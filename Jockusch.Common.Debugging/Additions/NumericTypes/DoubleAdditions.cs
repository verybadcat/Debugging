using System;
using Jockusch.Common.Debugging;
using numericType = System.Double;
using System.Globalization;
using System.Collections.Generic;

namespace Jockusch.Common
{
  public static class DoubleAdditions
  {
    public static bool IsInteger(this numericType x) {
      bool r = (x == x.ToInteger(0));
      return r;
    }
    public static double Increment(this double x) {
      double y = x*2;
      double z = x + y / 2;
      while (z!=x && z!=y) {
        y = z;
        z = x + y / 2;
      }
      return y;
    }
    public static int WantsSpaceAfterEncoding(this numericType x) {
      // name has to be different from the corresponding DoubleWrapper.EncodingWantsSpaceAfter() because
      // we are using an implicit type conversion
      if (x.IsFinite()) {
        return 0;
      } else {
        return 1;
      }
    }
    public const numericType SMALL_NUMBER = NumericalTolerances.DoubleTypicalRoundoff;

    public static numericType Min(params numericType[] list) {
      numericType r = numericType.NaN;
      foreach (numericType x in list) {
        if (!(numericType.IsNaN(x))) {
          if (!(r < x)) { // we write it this way so that min or max of a numericType and NaN returns the numericType.
            r = x;
          }
        }
      }
      return r;
    }
    public static float ToFloat(this numericType x) {
      float r;
      if (double.IsNaN(x)) {
        r = float.NaN;
      } else if (x > float.MaxValue) {
        r = float.PositiveInfinity;
      } else if (x < float.MinValue) {
        r = float.NegativeInfinity;
      } else {
        r = (float)x;
      }
      return r;
    }
    public static numericType Min(List<numericType> list) {
      // keep the input a list, not IEnumerable, or it will conflict with the params version.
      return DoubleAdditions.Min(list.ToArray());
    }
    public static numericType Max(List<numericType> list) {
      return DoubleAdditions.Max(list.ToArray());
    }
    public static numericType Max(params numericType[] list) {
      numericType r = numericType.NaN;
      foreach (numericType x in list) {
        if (!(numericType.IsNaN(x))) {
          if (!(r > x)) { // we write it this way so that min or max of a numericType and NaN returns the numericType.
            r = x;
          }
        }
      }
      return r;
    }

    public static bool AnyNaN(params numericType[] list) {
      foreach (numericType x in list) {
        if (numericType.IsNaN(x)) {
          return true;
        }
      }
      return false;
    }

    public static bool IsSmall(this numericType d, numericType tolerance = SMALL_NUMBER) {
      numericType absD = Math.Abs(d);
      bool r = (absD < tolerance);
      return r;
    }
    public static numericType RoundIfCloseToInteger(this numericType x, numericType maxAmountToRound) {
      numericType roundX = Math.Round(x);
      double error = Math.Abs(x - roundX);
      numericType r = (error <= maxAmountToRound) ? roundX : x;
      return r;
    }
    public static bool IsBetween(this numericType f, numericType min, numericType max) {
      bool r = false;
      if (f >= min) {
        if (f <= max) {
          r = true;
        }
      }
      return r;
    }
    public static numericType RestrictToRange(this numericType x, numericType min, numericType max)
    {
      if (x < min) {
        return min;
      }
      if (x > max) {
        return max;
      }
      return x;
    }
    public static int RoundToInt(this numericType x) {
      if (double.IsNaN(x)) {
        return int.MinValue;
      } else if (x < int.MinValue) {
        return int.MinValue;
      } else if (x > int.MaxValue) {
        return int.MaxValue;
      } else {
        int r = (int)Math.Round(x);
        return r;
      }
    }
    public static int RoundDownToInt (this numericType x) {
      double smaller = x - 0.5;
      double roundMe = smaller.Increment();
      int r = roundMe.RoundToInt();
      return r;
    }
    public static numericType RoundSmallToZero(this numericType d, numericType tolerance = SMALL_NUMBER) {
      if (numericType.IsNaN(d)) {
        return numericType.NaN;
      }
      bool small = d.IsSmall(tolerance);
      if (small) {
        return 0;
      }
      return d;
    }
    /// returns true if the difference is less than roundoff tolerance, OR if the ratio is within roundoff tolerance of 1.
    public static bool ApproximatelyEquals(this numericType This, numericType x) {
      numericType small = NumericalTolerances.DoubleTypicalRoundoff;
      if (Math.Abs(x) <= 1) {
        if (Math.Abs(This - x) < small) {
          return true;
        }
      } else {
        if (Math.Abs(This / x - 1) < small) {
          return true;
        }
      }
      return false;
    }
    public static bool LessThanOrApproximatelyEqual(this numericType This, numericType x) {
      bool less = This < x;
      bool approx = This.ApproximatelyEquals(x);
      bool r = (less || approx);
      return r;
    }
    public static bool LessThanAndNotApproximatelyEqual(this numericType This, numericType x) {
      bool less = This < x;
      bool approx = This.ApproximatelyEquals(x);
      bool r = (less && !approx);
      return r;
    }
    public static numericType ModulusPositive(numericType m, numericType n) {
      // e.g. we want -5 mod 3 to be 1, not -2.
      numericType absN = Math.Abs(n);
      numericType r1 = (m % absN) + absN;
      numericType r = (r1 % absN);
      return r;
    }
    public static bool IsFinite(this numericType x) {
      if (numericType.IsNaN(x)) {
        return false;
      }
      if (numericType.IsInfinity(x)) {
        return false;
      }
      return true;
    }
    public static numericType Quantum(this numericType x) {
      // The absolute value of the difference between x and the numericType that is as little as possible further from zero while not equalling x.
      numericType absX = Math.Abs(x);
      numericType r;
      if (absX == 0) {
        r = numericType.Epsilon;
      } else if (x == numericType.MaxValue || x == numericType.MinValue) {
        r = numericType.PositiveInfinity;
      } else if (!x.IsFinite()) {
        r = numericType.NaN;
      } else {
        numericType testDelta = absX;
        r = testDelta;
        while (absX + testDelta != absX) {
          r = (absX + testDelta) - absX;
          testDelta /= 2;
        }
      }
      return r;
    }
    public static numericType AddQuantum(this numericType x) {
      return x + x.Quantum();
    }
    public static numericType MinusQuantum(this numericType x) {
      return x - x.Quantum();
    }
    public static int? ToInteger(this numericType x, numericType allowedRoundoffError = NumericalTolerances.DoubleCloseToInteger) {
      numericType roundX = Math.Round(x);
      numericType restrictRoundX = roundX.RestrictToRange(int.MinValue, int.MaxValue);
      numericType difference = x - restrictRoundX;
      numericType absDifference = Math.Abs(difference);
      if (absDifference <= allowedRoundoffError) {
        return (int)restrictRoundX;
      } else {
        return null;
      }
    }
    public static numericType IntegerPower(this numericType x, int exponent) {
      numericType r;
      if (exponent < 0) {
        numericType denominator = x.IntegerPower(-exponent);
        r = 1 / denominator;
      } else if (exponent == 0) {
        r = 1;
      } else {
        int halfExponent = (exponent / 2);
        numericType halfPower = DoubleAdditions.IntegerPower(x, halfExponent);
        bool even = (exponent % 2 == 0);
        if (even) {
          r = halfPower * halfPower;
        } else {
          r = halfPower * halfPower * x;
        }
      }
      return r;
    }
    public static int OrderOfMagnitude(this numericType x, double toBase=10) {
      numericType absX = Math.Abs(x);
      if (absX == 0) {
        return int.MinValue;
      } else {
        numericType log = Math.Log(absX, toBase);
        return (int)log;
      }
    }
    /// <summary>Empty or whitespace string parses to zero.  Returns NaN on failure.</summary>
    public static numericType TryParse(string s) {
      numericType r = DoubleQAdditions.TryParse(s) ?? numericType.NaN;
      return r;
    }

    public static numericType TryParse(string s, NumberParseHandling handling) {
      numericType r;
      switch (s) {
        case StringConstants.Infinity:
        case StringConstants.PlusInfinity:
          {
            r = numericType.PositiveInfinity;
            break;
          }
        case StringConstants.MinusInfinity:
          {
            r = numericType.NegativeInfinity;
            break;
          }
        default:
          {
            r = numericType.NaN;
            switch (handling) {
              case NumberParseHandling.Default:
                break;
              case NumberParseHandling.EmptyToZero:
                if (s == "") {
                  r = 0;
                }
                break;
              case NumberParseHandling.Coefficient:
                if (s == "" || s == "+") {
                  r = 1;
                } else if (s == "-") {
                  r = -1;
                } 
                break;
              default:
                CommonDebug.UnexpectedInputBreakpoint();
                throw new BreakingException ();
            }
            if (numericType.IsNaN(r)) {
              r = DoubleAdditions.TryParse(s);
            }
          }
          break;
      }


      return r;
    }
    /// <summary>For formatting something where the number is a coefficient in front of something like "x" or pi.</summary>
    public static string TermDisplayString(this numericType This, string factorString, bool showPlusSign = false, bool lossless = false) {
      string partWay;
      if (This.Equals(0)) {
        partWay = "";
      }
      if (This < 0) {
        numericType absThis = This * -1;
        partWay = "-" + TermDisplayString(absThis, factorString, false, lossless);
      } else {
        string rWithoutPlus;
        if (This.Equals(1)) {
          if (factorString.Trim() == "") {
            rWithoutPlus = "1";
          } else {
            rWithoutPlus = factorString;
          }
        } else {
          rWithoutPlus = This.ToString(lossless) + factorString;
        }
        string plusString = showPlusSign ? "+" : "";
        partWay = plusString + rWithoutPlus;  
      }
      string r = partWay.Replace("E+", "E");
      return r;
    }
    public static string ToStringLossless(this numericType number) {
      IFormatProvider provider = CultureInfo.InvariantCulture;    
      string partWay = number.ToString("R", provider);
      string r = partWay.Replace("E+", "E");
      return r;
    }
    public static string ToString(this numericType number, bool lossless) {
      string r;
      if (lossless) {
        r = number.ToString("R");
      } else {
        r = number.ToString();
      }
      return r;
    }
    #region complex #define compatibility
    /// <summary>Also defined on Complex. Allows one to write code that will work where the type may be complex
    /// or double, depending on a #define.</summary>
    public static double RealPart(this numericType x) {
      return x;
    }
    public static bool IsReal(this numericType x) {
      return true;
    }
    public static bool IsImaginary(this numericType x) {
      return (x == 0);
    }
    #endregion
  }
}
