using System;
using System.Globalization;
using Jockusch.Common.Debugging;
using numericType = System.Int64;

namespace Jockusch.Common
{
  public static class LongAdditions {
     
    public static numericType LongSqrt(this numericType n)
    {
      numericType theByte = 1 << 62;
      while (theByte > n)
      {
        theByte = theByte >> 2;
      }
      long r = 0;
      while (theByte!=0)
      {
        if (n >= r + theByte)
        {
          n -= r + theByte;
          r = (r >> 1) + theByte;
        } else
        {
          r = r >> 1;
        }
        theByte >>= 2;
      }
      return r;
    }
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
    public static numericType LCM(this numericType x, numericType y)
    {
      numericType gcd = LongAdditions.GCD(x, y);
      numericType r = (x / gcd) * y;
      return r;
    }
    public static long IntegerPower(long theBase, int exponent) {
      CommonDebug.Assert(exponent >= 0);
      long r = 1;
      for (int i = 0; i < exponent; i++) {
        r *= theBase;
      }
      return r;
    }
    public static int ToInt(this long value) {
      int r;
      if (value > int.MaxValue) {
        CommonDebug.BreakPoint();
        r = int.MaxValue;
      } else if (value < int.MinValue) {
        CommonDebug.BreakPoint();
        r = int.MinValue;
      } else {
        r = Convert.ToInt32(value);
      }
      return r;
    }
    public static byte ToByte(this numericType value) {
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
    public static numericType? TryParseQ(string s)
    {
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
    public static numericType TryParse(string s, numericType defaultValue)
    {
      numericType parseResult;
      bool success = numericType.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out parseResult);
      numericType r = success ? parseResult : defaultValue;
      return r;
    }
  }
}

