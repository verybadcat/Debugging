using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using numericType = System.Single;
using System.Globalization;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class FloatAdditions
  {
    ///<summary>Ignores any NaN in the list.</summary> 
    public static float Min(params float[] list) {
      float r = float.NaN;
      foreach (float x in list) {
        if (!(float.IsNaN(x))) {
          if (!(r < x)) { // we write it this way so that min or max of a float and NaN returns the float.
            r = x;
          }
        }
      }
      return r;
    }
    ///<summary>Ignores any NaN in the list.</summary> 
    public static float Max(params float[] list) {
      float r = float.NaN;
      foreach (float x in list) {
        if (!(float.IsNaN(x))) {
          if (!(r > x)) { // we write it this way so that min or max of a float and NaN returns the float.
            r = x;
          }
        }
      }
      return r;
    }
    /// <summary>Naming this IsFinite seems to lead to some confusion with the IsFinite()
    /// methods on other numeric types.</summary>
    public static bool IsFiniteFloat(this float x) {
      bool infinite = float.IsInfinity(x);
      bool nan = float.IsNaN(x);
      bool r = !(infinite || nan);
      return r;
    }
    public static bool AllFinite(params float[] list) {
      bool r = true;
      foreach (float x in list) {
        bool finite = x.IsFiniteFloat();
        if (!finite) {
          r = false;
          break;
        }
      }
      return r;
    }
    public static bool AllFinite(this float x, float[] additionalFloats) {
      bool xFinite = x.IsFiniteFloat();
      bool additionalFinite = FloatAdditions.AllFinite(additionalFloats);
      bool r = xFinite && additionalFinite;
      return r;
    }
    public static float FirstFinite(params float?[] floats) {
      float r = float.NaN;
      foreach (float? x in floats) {
        if (x.HasValue)
        if (FloatAdditions.IsFiniteFloat(x.Value)) {
          r = x.Value;
          break;
        }
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
    public static numericType RestrictToRange(this numericType f, numericType min, numericType max) {
      if (f < min) {
        return min;
      }
      if (f > max) {
        return max;
      }
      return f;
    }
    /// <summary>
    /// Same as Math.Round, but returns a float.
    /// </summary>
    public static float RoundToFloat(this float x) {
      float r = (float)Math.Round(x);
      return r;
    }
    public static float RoundDownToFloat(this float x) {
      float r = (int)x;
      if (r > x) { // can happen if x < 0
        r--;
      }
      return r;
    }
    /// <summary> If minimumOf1 is passed in as true, any result which is undefined or less than 1 will be converted to 1.</summary>
    public static float MultiplyAndRound(this float x, float y, bool minimumOf1 = false) {
      float product = x * y;
      float r = product.RoundToFloat();
      if (minimumOf1) {
        r = Math.Max(1f, r);
      }
      return r;
    }    
    /// <summary> If minimumOf1 is passed in as true, any result which is undefined or less than 1 will be converted to 1.</summary>
    public static float MultiplyAndRoundDown(this float x, float y, bool minimumOf1 = false) {
      float product = x * y;
      float r = product.RoundDownToFloat();
      if (minimumOf1) {
        if (!(r>1)) {
          r = 1;
        }
      }
      return r;
    }
    /// <summary> If minimumOf1 is passed in as true, any result which is undefined or less than 1 will be converted to 1.</summary>
    public static float DivideAndRound(this float x, float y, bool minimumOf1 = false) {
      float ratio = x / y;
      if (minimumOf1 && !(ratio>=1)) {
        ratio = 1;
      }
      float r = ratio.RoundToFloat();
      return r;
    }
    public static numericType IntegerPower(this numericType f, int exponent) {
      numericType r;
      if (exponent < 0) {
        numericType denominator = f.IntegerPower(-exponent);
        r = 1 / denominator;
      } else if (exponent == 0) {
        r = 1;
      } else {
        int halfExponent = (exponent / 2);
        numericType halfPower = FloatAdditions.IntegerPower(f, halfExponent);
        bool even = (exponent % 2 == 0);
        if (even) {
          r = halfPower * halfPower;
        } else {
          r = halfPower * halfPower * f;
        }
      }
      return r;
    }
    /// <summary> If minimumOf1 is passed in as true, any result which is undefined or less than 1 will be converted to 1.
    /// This method can produce some unexpected results, i.e. (5f/3).DivideRoundDown(1f/3)=4.</summary>
    public static numericType DivideRoundDown(this numericType x, numericType y, bool minimumOf1 = false) {
      float ratio = x / y;
      float r;
      if (minimumOf1 && !(ratio > 1)) {
        r = 1;
      } else {
        r = (int)ratio;
      }
      return r;
    }
    public static int RoundToInt(this numericType x) {
      int r;
      if (x > int.MaxValue) {
        r = int.MaxValue;
      } else {
        r = (int)Math.Round(x);
      }
      return r;
    }
    public static bool IsNullOrNan(float? x) {
      if (x == null) {
        return true;
      }
      if (float.IsNaN(x.Value)) {
        return true;
      }
      return false;
    }
    public static float Power(this float x, double exponent) {
      double dbl = x;
      double dR = Math.Pow(dbl, exponent);
      float r = (float)dR;
      return r;
    }
    public static IEnumerable<numericType> Minus(this IEnumerable<numericType> input, numericType subtractMe) {
      numericType addMe = -subtractMe;
      return input.Plus(addMe);
    }
    public static IEnumerable<numericType> Plus(this IEnumerable<numericType> input, numericType addMe) {
      foreach (numericType x in input) {
        yield return x + addMe;
      }
    }
    /// <summary>Empty or whitespace string parses to zero.  Returns NaN on failure.</summary>
    public static numericType TryParse(string s) {
      numericType r = numericType.NaN;
      numericType parseResult;
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
    public static string ToString(this numericType number, bool lossless) {
      string r;
      if (lossless) {
        r = number.ToString("R");
      } else {
        r = number.ToString();
      }
      return r;
    }
    public static float UndefinedToZero(this numericType x) {
      numericType r = x;
      if (!FloatAdditions.IsFiniteFloat(x)) {
        r = 0;
      }
      return r;
    }
    public static bool IsInteger(this numericType x) {
      bool r = (x == (int)x);
      return r;
    }
    public static int RoundUpToInt(this float x) {
      int r = (int)Math.Ceiling(x);
      return r;
    }

    public static bool IsSaneDrawingAmount(this float x, bool zeroOK = true) {
      bool r = false;
      if (x < NumericalTolerances.ScreenWidthOrHeightTooBig) {
        if (x > 0 || (x == 0 && zeroOK)) {
          r = true;
        }
      }
      return r;
    }
    public static float ToFinite(this float f, float finiteValue = 0, bool adjustingIsABug = true) {
      if (float.IsInfinity(f)||float.IsNaN(f)) {
        if (adjustingIsABug) {
          CommonDebug.BreakPoint();
        }
        f = finiteValue;
      }
      return f;
    }
    public static numericType Quantum(this numericType x) {
      // The absolute value of the difference between x and the numericType that is as little as possible further from zero while not equalling x.
      numericType absX = Math.Abs(x);
      numericType r;
      if (absX == 0) {
        r = numericType.Epsilon;
      } else if (x == numericType.MaxValue || x == numericType.MinValue) {
        r = numericType.PositiveInfinity;
      } else if (!x.IsFiniteFloat()) {
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

    public static numericType ResolveNaN(this numericType x, numericType backup) {
      if (float.IsNaN(x)) {
        return backup;
      } else {
        return x;
      }
    }
    public static numericType AddQuantum(this numericType x) {
      return x + x.Quantum();
    }
    public static numericType MinusQuantum(this numericType x) {
      return x - x.Quantum();
    }
    public static numericType AtLeast(this numericType x, numericType minValue) {
      return Math.Max(x, minValue);
    }
  }
}
