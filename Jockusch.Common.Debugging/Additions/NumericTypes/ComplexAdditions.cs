using System;
using System.Numerics;
using complexNumericType = System.Numerics.Complex;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class ComplexAdditions
  {
    private static string ImaginaryTermDisplayString(this complexNumericType z, bool showPlusSign) {
      double imaginary = z.Imaginary;
      if (double.IsPositiveInfinity(imaginary)) {
        if (showPlusSign) {
          return StringConstants.PlusInfinity + " " + StringConstants.ImaginaryI;
        } else {
          return StringConstants.Infinity + " " + StringConstants.ImaginaryI;
        }
      } else if (double.IsNegativeInfinity(imaginary)) {
        return StringConstants.MinusInfinity + " " + StringConstants.ImaginaryI;
      } else {
        return DoubleAdditions.TermDisplayString(imaginary, "i", showPlusSign, true);
      }
    }

    private static string ToStringLossless(this complexNumericType z) {
      double real = z.Real;
      double imaginary = z.Imaginary;
      string realString = DoubleAdditions.ToStringLossless(real);
      string r;
      if (z.IsReal()) {
        r = realString;
      } else if (z.IsNaN()) {
        r = StringConstants.ComplexNaN;
      } else if (z.IsImaginary()) {
        // In particular, in this case, the imaginary part is never zero.
        r = z.ImaginaryTermDisplayString(false);
      } else {
        string imaginaryString = z.ImaginaryTermDisplayString(true);
        r = realString + imaginaryString;
      }
      return r;
    }
    /// <summary>For now, we actually always use a lossless conversion.  IDK if we will ever want a lossy one or not.</summary>
    public static string ToString(this complexNumericType z, bool lossless) {
      string r = ComplexAdditions.ToStringLossless(z);
      return r;
    }
    public static Complex NaN {
      get {
        return new Complex (double.NaN, double.NaN);
      }
    }
		public static bool IsFinite(this complexNumericType z) {
			bool realFinite = z.Real.IsFinite();
			bool imagFinite = z.Imaginary.IsFinite();
			bool r = realFinite && imagFinite;
			return r;
		}
    public static bool IsNotNaN(this complexNumericType z) {
      bool r = !(z.IsNaN());
      return r;
    }
    public static bool IsRealInteger(this complexNumericType z) {
      bool r = (z.Real.IsInteger() && z.Imaginary == 0);
      return r;
    }
    /// <summary>If the term is not either zero or pure imaginary, returns NaN.
    /// Does not call TryParse.</summary>
    public static complexNumericType TryParsePureImaginary(string parseMe) {
      string trimmed = parseMe.Trim();
      int sign = 1;
      if (trimmed.StartsWithInvariant("-")) {
        sign = -1;
        trimmed = trimmed.Substring(1);
      } else if (trimmed.StartsWithInvariant("+")) {
        trimmed = trimmed.Substring(1);
      }
      int iIndex = trimmed.IndexOfInvariant("i");
      complexNumericType r = ComplexAdditions.NaN;
      string rest = null;
      if (iIndex == -1) {
        double x = DoubleAdditions.TryParse(parseMe);
        if (x == 0) {
          r = 0;
        }
      } else if (iIndex == 0) {
        rest = trimmed.Substring(1);
      } else if (trimmed[trimmed.Length - 1] == 'i') {
        rest = trimmed.Substring(0, trimmed.Length - 1);
      }
      if (rest!=null) {
        double val = DoubleAdditions.TryParse(rest, NumberParseHandling.Coefficient);
        if (!double.IsNaN(val)) {
          r = new Complex(0, val * sign);
        }
      }
      return r;
    }
    /// <summary>Returns NaN if the number is not pure real or imaginary.</summary>
    public static complexNumericType TryParsePureRealOrImaginary(string parseMe) {
      complexNumericType r = ComplexAdditions.NaN;
      if (parseMe.ContainsInvariant("i")) {
        r = TryParsePureImaginary(parseMe);
      }
      if (r.IsNaN()) {
        // Writing it this way, without using "else", caters to the case "Infinity."
        r = DoubleAdditions.TryParse(parseMe);
      }
      return r;
    }
    public static complexNumericType TryParse(string parseMe) {
      string s = parseMe.Trim();
      complexNumericType r = ComplexAdditions.NaN;
      if (s.ContainsInvariant("i")) {
        char[] sign = "+-".ToCharArray();
        int signIndex = s.LastIndexOfAny(sign);
        if (signIndex == -1 || signIndex == 0) {
          r = ComplexAdditions.TryParsePureRealOrImaginary(parseMe);
        } else {
          string str1 = s.Substring(0, signIndex);
          string str2 = s.Substring(signIndex);
          complexNumericType r1 = TryParse(str1);
          complexNumericType r2 = TryParse(str2);
          r = r1 + r2;
        }
      } else {
        r = DoubleAdditions.TryParse(parseMe, NumberParseHandling.EmptyToZero);
      }
      return r;
    }
    public static bool IsNaN(this complexNumericType number) {
      double real = number.Real;
      double imag = number.Imaginary;
      bool r = double.IsNaN(real) || double.IsNaN(imag);
      return r;
    }
    /// <summary> A complex number is considered to be real iff its imaginary part
    /// is zero and its real part is not NaN.  At least for now -- not sure if the 
    /// NaN part is really what we will end up wanting.</summary>
    public static bool IsReal(this complexNumericType number) {
      bool r = false;
      double imag = number.Imaginary;
      if (imag == 0) {
        double real = number.Real;
        if (!IsNaN(real)) {
          r = true;
        }
      }
      return r;
    }
    /// <summary>We include 0 as an imaginary number.</summary>
    public static bool IsImaginary (this complexNumericType number) {
      bool r = false;
      double re = number.Real;
      if (re == 0) {
        double imag = number.Imaginary;
        if (!IsNaN(imag)) {
          r = true;
        }
      }
      return r;
    }
		public static bool IsAlmostReal(this complexNumericType z, double roundoffTolerance) {
			bool r = false;
			double imag = z.Imaginary;
			if (Math.Abs(imag) <= roundoffTolerance) {
				double real = z.Real;
				if (!IsNaN(real)) {
					r = true;
				}
			}
			return r;
		}
		public static bool IsAlmostImaginary(this complexNumericType z, double roundoffTolerance) {
			bool r = false;
			double real = z.Real;
			if (Math.Abs(real) <= roundoffTolerance) {
				double imag = z.Imaginary;
				if (!IsNaN(real)) {
					r = true;
				}
			}
			return r;
		}
    /// <summary>e.g. factorString might be "x."  In such a case, this method will correctly format things like:
    /// "-x" for -1 times x, the empty string for 0 times x, "x" for 1 times x, and so on.</summary>
    public static string TermDisplayString(this complexNumericType z, string factorString, bool showPlusSign = false) {
      string r;
      double imaginary = z.Imaginary;
      double real = z.Real;
      if (imaginary == 0) {
        r = DoubleAdditions.TermDisplayString(real, factorString, showPlusSign);
      } else if (real == 0) {
        r = z.ImaginaryTermDisplayString(showPlusSign) + factorString;
      } else {
        string rWithoutPlus = z.ToString(false);  // the "false" part matters as it forces the correct ToString method
        r = "(" + rWithoutPlus + ")" + factorString;
        if (showPlusSign) {
          r = "+" + r;
        }
      }
      return r;
    }
    public static complexNumericType RoundSmallPartsToIntegers(this Complex z, double tolerance = NumericalTolerances.DoubleTypicalRoundoff) {
      double real = z.Real;
      double imag = z.Imaginary;
      int? roundedReal = real.ToInteger(tolerance);
      int? roundedImag = imag.ToInteger(tolerance);
      double realOut = roundedReal ?? real;
      double imagOut = roundedImag ?? imag;
      complexNumericType r = new complexNumericType(realOut, imagOut);
      return r;
    }
		/// <summary>Intended for polynomial stuff, NOT for display string</summary>
    public static Complex RoundSmallPartsForPolynomialCalculations(this Complex z) {
      double real = z.Real;
      double imaginary = z.Imaginary;
      double absReal = Math.Abs(real);
      double absImaginary = Math.Abs(imaginary);
      bool realCloseToZero = (absReal < NumericalTolerances.DoubleTypicalRoundoff);
      bool imaginaryCloseToZero = (absImaginary < NumericalTolerances.DoubleTypicalRoundoff);
      if (realCloseToZero && !imaginaryCloseToZero) {
        real = 0;
      }
      if (imaginaryCloseToZero && !realCloseToZero) {
        imaginary = 0;
      }
      Complex r = new Complex (real, imaginary);
      return r;
    }
    /// <summary>Takes the power via multiplication, thereby avoiding roundoff error in cases like (1+i)^8.</summary> 
    public static complexNumericType IntegerPower(this complexNumericType z, int n) {
      if (n == -1) {
        return 1 / z;
      }
      else if (n < 0) {
        if (n == int.MinValue) {
          complexNumericType value = (z.IntegerPower(n / 2));
          return value * value;
        } else {
          return 1 / z.IntegerPower(-n);
        }
      } else {
        switch (n) {
          case 0: {
              if (z == 0) {
                return ComplexAdditions.NaN;
              } else {
                return 1;
              }
            }
          case 1: {
              return z;
            }
          default: {
              int halfN = n / 2;
              complexNumericType input = z.IntegerPower(halfN);
              complexNumericType r = input * input;
              if (n%2==1) {
                r = r * z;
              }
              return r;
            }
        }
      }
    }
    /// <summary> Returns null if z is not an integer.  Does not allow any roundoff error.</summary>
    public static int? ToInteger(this complexNumericType z) {
      int? r = null;
      if (z.IsReal()) {
        double real = z.Real;
        r = real.ToInteger(0);
      }
      return r;
    }
    /// <summary> If the exponent is an integer, uses multiplication instead of logs.</summary>
    public static complexNumericType ComplexPower(this complexNumericType z, complexNumericType exponent) {
      int? integerExponent = exponent.ToInteger();
      if (integerExponent.HasValue) {
        complexNumericType r = z.IntegerPower(integerExponent.Value);
        return r;
      } else {
        return Complex.Pow(z, exponent);
      }
    }
    ///<summary> Returns null if the complex number has a nonzero imaginary part.</summary>
    public static double? ToDoubleQ(this Complex z) {
      double? r = null;
      bool real = (z.Imaginary == 0);
      if (real) {
        r = z.Real;
      }
      return r;
    }
    ///<summary> Returns NaN if the complex number is not real</summary>
    public static double ToDouble(this Complex z) {
      double r = Double.NaN;
      bool real = z.IsReal();
      if (real) {
        r = z.Real;
      }
      return r;
    }

    /// <summary>Purpose is to allow one to write code where the type can be defined
    /// as Complex or Double. See also DoubleAdditions.RealPart.</summary>
    public static double RealPart(this Complex z) {
      return z.Real;
    }

    #region naming
    public static string GetNoun(this Complex z) {
      string r;
      bool nan = z.IsNaN();
      if (nan) {
        r = "not a number";
      }
      else if (z.Imaginary == 0) {
        r = "real number";
      } else {
        r = "complex number";
      }
      return r;
    }
    #endregion
  }
}

