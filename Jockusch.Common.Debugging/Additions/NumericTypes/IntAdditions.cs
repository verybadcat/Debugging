using System;
using System.Linq;
using System.Collections.Generic;
using Jockusch.Common.Debugging;
using numericType = System.Int32;
using System.Globalization;

namespace Jockusch.Common
{
  internal class IntWithRemainder {
    internal numericType X { get; set; }
    internal float Remainder { get; set; }
    internal IntWithRemainder (float input)
    {
      numericType x = (numericType)Math.Floor ((double)input);
      this.X = x;
      this.Remainder = input - x;
    }
    internal void RoundUp() {
      if (this.Remainder > 0) {
        this.X = this.X + 1;
        this.Remainder = 0;
      }
    }
  }

  public static class IntAdditions {
     
    public static numericType GCD(this numericType i1, numericType i2) {
      if (i1 < 0) {
        i1 *= -1;
      }
      if (i2 < 0) {
        i2 *= -1;
      }
      while (true) {
        if (i1 == 0) {
          return i2;
        }
        if (i2 == 0) {
          return i1;
        }
        if (i1 > i2) {
          i1 = i1 % i2;
        } else {
          i2 = i2 % i1;
        }
      }
    }

    public static numericType RestrictToRange(this numericType restrictMe, numericType min, numericType max, bool breakAtFailure = false) {
      if (restrictMe < min) {
        CommonDebug.BreakPointIf(breakAtFailure);
        return min;
      }
      if (restrictMe > max) {
        CommonDebug.BreakPointIf(breakAtFailure);
        return max;
      }
      return restrictMe;
    }

    /// <summary>Calling inList[index] will still fail if the list is null or empty.</summary>

    public static numericType RestrictToRange<T>(this numericType restrictMe, IList <T> inList, bool breakAtFailure) {
      int count = inList.Count();
      numericType r = restrictMe.RestrictToRange(0, count-1, breakAtFailure);
      return r;
    }

    public static numericType DivideRoundDown(this numericType numerator, numericType denominator) {
      numericType ratio = numerator / denominator;
      numericType modulus = numerator % denominator;
      numericType r;
      if (modulus == 0 || numerator >= 0) {
        r = ratio;
      } else {
        r = ratio - 1;
      }
      return r;
    }

    public static numericType ModulusPositive(this numericType m, numericType n) {
      numericType absN = Math.Abs (n);
      numericType r = (((m % absN) + absN) % absN);
      return r;
    }
    
    public static numericType Max(params numericType[] ints) {
      numericType r = numericType.MinValue;
      foreach (numericType i in ints) {
        if (i > r) {
          r = i;
        }
      }
      return r;
    }
    /// <summary>
    /// e.g. SplitProportionally (100, 0.6, 0.299, 0.101) = {60, 30, 10}.
    /// </summary>
    public static List<numericType> SplitProportionally(this numericType splitMe, params float[] weights) {
      float weightSum = weights.Sum (x => x);
      if (weightSum == 0) {
        CommonDebug.BreakPointLog ("Weights can't add up to zero!");
      }
      numericType n = weights.Length;
      List<IntWithRemainder> pieces = new List<IntWithRemainder> ();
      foreach (float weight in weights) {
        float ratio = splitMe * weight / weightSum;
        IntWithRemainder iwr = new IntWithRemainder (ratio);
        pieces.Add (iwr);
      }
      List<IntWithRemainder> sortedPieces = pieces.OrderBy<IntWithRemainder, float> ((IntWithRemainder iwr) => iwr.Remainder).ToList ();
      float totalRemainder = pieces.Sum (iwr => iwr.Remainder);
      numericType roundedTotalRemainder = FloatAdditions.RoundToInt (totalRemainder);
      for (numericType i = n - roundedTotalRemainder; i < totalRemainder; i++) {
        IntWithRemainder piece = sortedPieces [i];
        piece.RoundUp ();
      }
      List<numericType> r = new List<numericType> ();
      foreach (IntWithRemainder piece in pieces) {
        r.Add (piece.X);
      }
      return r;
    }
    public static List<numericType> SplitProportionally(this numericType splitMe, List<float> weights) {
      List<numericType> r = splitMe.SplitProportionally(weights.ToArray());
      return r;
    }
    /// <summary>Splits splitMe into the given number of pieces.  Splitting is done as evenly as possible, with any leftovers
    /// spread uniformly throughout the list, i.e. 10 into 4 pieces would go to (2, 3, 2, 3). </summary>
    public static List<numericType> SplitProportionally(this numericType splitMe, numericType nPieces) {
      numericType quotient = splitMe / nPieces;
      numericType remainder = splitMe % nPieces;
      numericType leftOver = 0;
      List<numericType> r = new List<numericType> ();
      for (numericType i = 0; i < nPieces; i++) {
        numericType addMe = quotient;
        leftOver += remainder;
        if (leftOver >= quotient) {
          addMe++;
          leftOver -= remainder;
        }
        r.Add (addMe);
      }
      return r;
    }
    /// <summary>Any nulls are divided as equally as possible so that the sum is the total.  Any remainder is also divided as evenly as possible, e.g. if two are left in four nulls, the extra would go in the first and third.
    /// If there are no null entries in the list, the total is ignored, and the sum may or may not be the total.</summary>
    public static List<numericType> SplitProportionally(this numericType total, List<numericType?> knownPIeces) {
      numericType nullCount = 0;
      numericType nonNullSum = 0;
      foreach (numericType? entry in knownPIeces) {
        if (entry == null) {
          nullCount++;
        } else {
          nonNullSum += entry.Value;
        }
      }
      numericType nReplaced = 0;
      numericType combinedReplacedWidth = 0;
      numericType combinedWidthOfNulls = total - nonNullSum;
      numericType nWidths = knownPIeces.Count;
      List<numericType> r = new List<numericType> ();
      for (numericType i = 0; i < nWidths; i++) {
        numericType? entry = knownPIeces [i];
        if (entry == null) {
          nReplaced++;
          numericType entryWidth = (nullCount - 1 + combinedWidthOfNulls * nReplaced) / nullCount - combinedReplacedWidth;  
          // There is no division by zero problem with dividing by nullCount, as this line can only be hit if nullCount is positive.
          r.Add (entryWidth);
          combinedReplacedWidth += entryWidth;
        } else {
          r.Add (entry.Value);
        }
      }
      return r;
    }
    public static string NthSuffix(this numericType This) {
      numericType mod10 = This % 10;
      numericType mod100 = This % 100;
      string r;
      if (mod100 > 10 && mod100 < 20) {
        r = "th";
      } else {
        switch (mod10) {
        case 1:
          r = "st";
          break;
        case 2:
          r = "nd";
          break;
        case 3:
          r = "rd";
          break;
        default:
          r = "th";
          break;
        }
      }
      return r;
    }
    /// <summary> For making human-readable sigular or plural strings such as "1 day", "0 days", "2 days", etc.
    /// e.e. SingularOrPluralString(1, "day")= "1 day", but SingularOrPluralString(2, "day") = "2 days".</summary>
    public static string SingularOrPluralString (this int integer, string singularName, string pluralName = null, bool returnEmptyIfZero = false) {
      if (pluralName == null) {
        pluralName = singularName + "s";
      }
      switch (integer) {
        case 0:
          if (returnEmptyIfZero) {
            return "";
          } else {
            return "0 " + pluralName;
          }
        case 1:
          return "1 " + singularName;
        case -1:
          return "-1 " + singularName;
        default:
          return integer + " " + pluralName;
      }
    }
    public static numericType? TryParseQ(string s) {
      numericType parseResult;
      numericType? r = null;
      if (s?.Trim() == "") {
        r = 0;
      } else {
        bool success = numericType.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out parseResult);
        if (success) {
          r = parseResult;
        }
      }
      return r;
    }
    public static numericType TryParse(string s, numericType defaultValue) {
      numericType parseResult;
      bool success = numericType.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out parseResult);
      numericType r = success ? parseResult : defaultValue;
      return r;
    }
    public static numericType TryParse(object obj, numericType defaultValue = numericType.MinValue) {
      numericType r;
      if (obj is numericType) {
        r = (numericType)obj;
      } else if (obj is long) {
        r = ((long)obj).ToInt();
      } else {
        string str = obj.ToString();
        r = IntAdditions.TryParse(str, defaultValue);
      }
      return r;
    }
    public static bool ToBool(this int n) {
      return (n != 0);
    }
    public static int MultiplyAndRound(this int n, double factor, int minValueToReturn) {
      int product = (int)Math.Round(n * factor);
      return Math.Max(product, minValueToReturn);
    }
    public static byte ToByte(this numericType value)
    {
      byte r;
      if (value > byte.MaxValue) {
        CommonDebug.BreakPoint();
        r = byte.MaxValue;
      } else if (value < byte.MinValue) {
        CommonDebug.BreakPoint();
        r = byte.MinValue;
      } else {
        r = Convert.ToByte(value);
      }
      return r;
    }
  }
}

