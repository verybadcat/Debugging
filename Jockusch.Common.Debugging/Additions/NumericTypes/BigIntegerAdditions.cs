using Jockusch.Common.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using numericType = System.Numerics.BigInteger;

namespace Jockusch.Common
{
  public static class BigIntegerAdditions
  {
    private static BigInteger NewtonRootGuess(BigInteger a, int root, BigInteger prevGuess) {
      return ((root - 1) * prevGuess + a / BigInteger.Pow(prevGuess, root - 1)) / root;
    }
    private static BigInteger _TooBig { get; set; }
    private static BigInteger _TooNegative { get; set; }
    private static BigInteger _WillTakeAWhileToGetString { get; set; }
    private static BigInteger _NegativeWillTakeAWhileToGetString { get; set; }
    public static BigInteger WillTakeAWhileToGetString {
      get {
        if (_WillTakeAWhileToGetString == 0) {
          _WillTakeAWhileToGetString = BigInteger.One << 4000;
        }
        return _WillTakeAWhileToGetString;
      }
    }
    public static BigInteger NegativeWillTakeAWhileToGetString {
      get {
        if (_NegativeWillTakeAWhileToGetString == 0) {
          _NegativeWillTakeAWhileToGetString = -WillTakeAWhileToGetString;
        }
        return _NegativeWillTakeAWhileToGetString;
      }
    }
    public static BigInteger TooBig {
      get {
        if (_TooBig == 0) {
          _TooBig = (BigInteger.One << NumericalTolerances.TooManyDigits);
        }
        return _TooBig;
      }
    }

    public static BigInteger TooNegative {
      get {
        if (_TooNegative == 0) {
          _TooNegative = -1 * TooBig;
        }
        return _TooNegative;
      }
    }
    public static bool IsTooBigOrTooNegative(this BigInteger b) {
      bool r = false;
      if (b > TooBig) {
        r = true;
      } else if (b < TooNegative) {
        r = true;
      }
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
    /// <summary>If n and root are both positive, returns the largest BigInteger
    /// r such that r^root is less than or equal to n.
    /// Otherwise, returns -1.</summary>
    public static BigInteger NRootLowerBoundPos(BigInteger n, BigInteger root) {
      //http://stackoverflow.com/questions/8826822/calculate-nth-root-with-integer-arithmetic?rq=1
      if (CommonDebug.ReturningAssert(n > 0 && root > 0)) {
        if (root > int.MaxValue) {
          return 1;
        } else {
          int iRoot = (int)BigInteger.Max(root, 1);
          double logN = BigInteger.Log10(n);
          BigInteger guess = BigInteger.Pow(10, (int)(logN / iRoot));
          BigInteger guess2 = NewtonRootGuess(n, iRoot, guess);
          BigInteger next = NewtonRootGuess(n, iRoot, guess2);
          while (next < guess2) {
            guess2 = next;
            next = NewtonRootGuess(n, iRoot, guess2);
          }
          return guess2;
        }
      } else {
        return -1;
      }
    }
    /// <summary>Returns null if there is no exact root.</summary>
    public static BigInteger? NRootExact(this BigInteger x, BigInteger root) {
      if ((x == 0 || x == 1) && root >= 1) {
        return x;
      }
      if (root > 0 && root < int.MaxValue) {
        int iRoot = (int)root;
        if (x > 0) {
          BigInteger r = BigIntegerAdditions.NRootLowerBoundPos(x, iRoot);
          if (BigInteger.Pow(r, iRoot) == x) {
            return r;
          }
        } else if (iRoot % 2 == 1) {
          BigInteger rPos = BigIntegerAdditions.NRootLowerBoundPos(-x, iRoot);
          if (BigInteger.Pow(rPos, iRoot) == -x) {
            return -rPos;
          }
        }
      }
      return null;
    }
    /// <summary>e.g. factorString might be "x."  In such a case, this method will correctly format things like:
    /// "-x" for -1 times x, the empty string for 0 times x, "x" for 1 times x, and so on.</summary>
    public static string TermDisplayString(this numericType n, string factorString, bool showPlusSign = false, bool showZero = false) {
      string r;
      if (n.Equals(numericType.Zero)) {
        if (showZero) {
          r = showPlusSign ? "+0" : "0";
        } else {
          r = "";
        }
      } else {
        if (n < 0) {
          numericType absThis = n * -1;
          r = "-" + TermDisplayString(absThis, factorString);
        } else {
          string rWithoutPlus;
          if (n.Equals(numericType.One)) {
            if (factorString.Trim() == "") {
              rWithoutPlus = "1";
            } else {
              rWithoutPlus = factorString;
            }
          } else {
            rWithoutPlus = n.ToString() + factorString;
          }
          string plusString = showPlusSign ? "+" : "";
          r = plusString + rWithoutPlus;
        }
      }
      return r;
    }
    private static string SciOrEngDisplayString(this numericType n, string raw, int powerOfTen,
                                               int decimalPlaces) {
      string r;
      int length = raw.Length;
      if (powerOfTen == 0 || length <= decimalPlaces || length < powerOfTen) {
        r = raw;
      } else {
        int digitsBeforeDecimal = length - powerOfTen;
        if (digitsBeforeDecimal + decimalPlaces > length) {
          r = raw;
        } else {
          string suffix = "E" + powerOfTen.ToString();
          string beforeDecimal;
          if (digitsBeforeDecimal == 0) {
            beforeDecimal = "0";
          } else {
            beforeDecimal = raw.Substring(0, digitsBeforeDecimal);
          }
          string afterDecimal;
          if (decimalPlaces == -1) {
            afterDecimal = raw.Substring(digitsBeforeDecimal);
          } else {
            afterDecimal = raw.Substring(digitsBeforeDecimal, decimalPlaces);
          }
          r = beforeDecimal + "." + afterDecimal + suffix;
        }
      }
      return r;
    }
    //public static string DisplayString(this numericType n, DecimalDisplayOptions options) {
    //  string raw = n.ToString(); // time taken by this method pretty much all comes from this call.
    //  int digits = options.MaxDigitsForExact;
    //  bool sciOrEng = (raw.Length > digits);
    //  string r;
    //  if (sciOrEng) {
    //    bool eng = options.EngineeringNotationWhenTooBig;
    //    int powerOfTen = raw.Length - 1;
    //    if (eng) {
    //      powerOfTen = powerOfTen / 3 * 3;
    //    }
    //    r = BigIntegerAdditions.SciOrEngDisplayString(n, raw, powerOfTen, options.DecimalPlaces);
    //  } else {
    //    bool commas = options.Commas;
    //    if (commas) {
    //      int nDigits = raw.Length;
    //      int modulus = nDigits % 3;
    //      if (modulus == 0) {
    //        modulus = 3;
    //      }
    //      int nInsert = (nDigits - modulus) / 3;
    //      r = raw.InsertSpaced(modulus, nInsert, 3, ',');
    //    } else {
    //      r = raw;
    //    }
    //  }
    //  return r;
    //}
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
    public static numericType LCM(this numericType x, numericType y) {
      numericType gcd = BigIntegerAdditions.GCD(x, y);
      numericType r = (x / gcd) * y;
      return r;
    }
    public static numericType PowerNonNegativeExponent(this numericType big, long exponent) {
      numericType r;
      if (exponent < 0) {
        CommonDebug.BreakPoint();
        return 0;
      }
      switch (exponent) {
      case 0:
        r = 1;
        break;
      case 1:
        r = big;
        break;
      case 2:
        r = big * big;
        break;
      case 3:
        r = big * big * big;
        break;
      default: {
          r = 1;
          numericType factor = big;
          long currentExponent = 1;
          while (true) {
            if ((exponent & currentExponent) != 0) {
              r *= factor;
            }
            currentExponent <<= 1;
            if (currentExponent > exponent || currentExponent == 0) {
              break;
            }
            factor = factor * factor;
          }
          break;
        }
      }
      return r;
    }
    /// <summary>Returns true if the conversion is the 
    /// same number as the input. Otherwise, returns
    /// int.MinValue or int.MaxValue and false.</summary> 
    public static bool TryToInteger(this BigInteger big, out int result) {
      bool r = false;
      if (big < int.MinValue) {
        result = int.MinValue;
      } else if (big > int.MaxValue) {
        result = int.MaxValue;
      } else {
        result = (int)big;
        r = true;
      }
      return r;
    }
    public static numericType SquareRoot(this numericType n, out bool isExact) {
      numericType r = 0;
      isExact = false;
      CommonDebug.BreakPoint();
      return r;
    }
    public static string ToByteString(this numericType n) {
      byte [] bytes = n.ToByteArray();
      string r = "";
      foreach (byte b in bytes) {
        string byteString = b.ToString();
        r = r + byteString + " ";
      }
      return r;
    }
    /// <summary>Always returns false unless power > 0 and theBase > 1.
    /// When returning false, exponent is set to -1.</summary>
    public static bool IsPowerOfPositive(this BigInteger power, BigInteger theBase, out int exponent) {
      bool r = false;
      exponent = -1;
      if (power > 0 && theBase > 1) {
        if (power == 1) {
          exponent = 0;
          r = true;
        } else {
          BigInteger work = theBase;
          List<BigInteger> powers = new List<BigInteger>();
          int maxDigits = NumericalTolerances.TooManyDigits;
          double logBase = BigInteger.Log10(theBase);
          int maxPowers = (int)Math.Log(maxDigits / logBase, 2);
          while (power % work == 0) {
            if (power == work) {
              exponent = 1 << powers.Count;
              return true;
            }
            powers.Add(work);
            if (powers.Count == maxPowers) {
              break;
            }
            work = work * work;
          }
          int nShift = powers.Count;
          if (work > power && powers.Count > 0) {
            work = powers.Last();
            nShift--;
          }
          exponent = 0;
          while (true) {
            if (power == 1) {
              r = true;
              break;
            } else if (power % work == 0) {
              exponent += 1 << nShift;
              power = power / work;
              nShift--;
              if (nShift >= 0) {
                work = powers [nShift];
              }
            } else if (work > power) {
              nShift--;
              if (nShift > 0) {
                work = powers [nShift];
              } else {
                break;
              }
            } else {
              exponent = -1;
              break;
            }
          }
        }
      }
      return r;
    }
    /// <summary>returned value * returned value + remainder == n</summary>
    public static numericType BigSqrt(this numericType n, out numericType remainder) {
      if (n >= 0) {
        numericType theByte = 1;
        while (theByte <= n) {
          theByte = theByte << 2;
        }
        theByte = theByte >> 2;
        numericType r = 0;
        while (theByte != 0) {
          if (n >= r + theByte) {
            n -= r + theByte;
            r = (r >> 1) + theByte;
          } else {
            r = r >> 1;
          }
          theByte >>= 2;
        }
        remainder = n;
        return r;
      } else {
        throw new ArithmeticException();
      }
    }
    #region specific values
    private static BigInteger _MinDouble = 0;
    private static BigInteger _MaxDouble = 0;
    public static BigInteger MinDouble {
      get {
        if (_MinDouble == 0) {
          // no locks; it doesn't matter if this fires more than once.
          _MinDouble = new BigInteger(double.MinValue);
        }
        return _MinDouble;
      }
    }
    /// <summary>The system rounds too big to double.PositiveInfinity and
    /// too small to double.NegativeInfinity, which is exactly what I want.
    /// Putting this in as a level of indirection in case something changes.</summary>
    public static double ToDouble(this BigInteger b) {
      return (double)b;
    }
    public static BigInteger MaxDouble {
      get {
        if (_MaxDouble == 0) {
          // no locks; it doesn't matter if this fires more than once.
          _MaxDouble = new BigInteger(double.MaxValue);
        }
        return _MaxDouble;
      }
    }
    public static bool IsInDoubleRange(this BigInteger b) {
      bool r = (b >= BigIntegerAdditions.MinDouble && b <= BigIntegerAdditions.MaxDouble);
      return r;
    }
    #endregion
  }
}

