using System;
using System.Drawing;
using numericType = System.Single;
using PointType = System.Drawing.PointF;
using System.Linq;
using System.Collections.Generic;

namespace Jockusch.Common
{
  public static class PointFAdditions
  {
    public static PointType NaN {
      get {
        numericType nan = numericType.NaN;
        PointType r = new PointType (nan, nan);
        return r;
      }
    }
    public static bool IsNaN(this PointType point) {
      numericType x = point.X;
      numericType y = point.Y;
      bool r = false;
      if (numericType.IsNaN(x)) {
        if (numericType.IsNaN (y)) {
          r = true;
        }
      }
      return r;
    }
    public static bool IsFinite(this PointType point) {
      bool r = (point.X.IsFiniteFloat() && point.Y.IsFiniteFloat());
      return r;
    }
    public static bool IsSane(this PointType point) {
      return !point.IsNaN();
    }
    public static PointType Normalized (this PointType point)
    {
      numericType length = point.Length ();
      if (length == 0) {
        return new PointType(0, 0);
      }
      numericType inverseLength = 1/length;
      PointType r = point.Times(inverseLength);
      return r;
    }
    public static PointType Plus (this PointType point1, PointType point2) {
      numericType x1 = point1.X;
      numericType x2 = point2.X;
      numericType y1 = point1.Y;
      numericType y2 = point2.Y;
      PointType r = new PointType(x1+x2, y1+y2);
      return r;
    }
    public static PointType Minus (this PointType point1, PointType point2) {
      numericType x1 = point1.X;
      numericType x2 = point2.X;
      numericType y1 = point1.Y;
      numericType y2 = point2.Y;
      PointType r = new PointType(x1-x2, y1-y2);
      return r;
    }
    public static PointType Times (this PointType point, numericType scalar)
    {
      numericType xIn = point.X;
      numericType yIn = point.Y;
      numericType x = xIn*scalar;
      numericType y = yIn*scalar;
      PointType r = new PointType(x, y);
      return r;
    }
    public static PointType TimesXY(this PointType point, numericType xScalar,
      numericType yScalar, bool roundToIntegers)
    {
      numericType xIn = point.X;
      numericType yIn = point.Y;
      numericType x = xIn * xScalar;
      numericType y = yIn * yScalar;
      if (roundToIntegers)
      {
        x = x.RoundToFloat();
        y = y.RoundToFloat();
      }
      return new PointType(x, y);
    }
    public static numericType Length(this PointType point) {
      numericType x = point.X;
      numericType y = point.Y;
      numericType rSquared = x * x + y * y;
      numericType r = (numericType)Math.Sqrt (rSquared);
      return r;
    }
    public static PointType Rotate(this PointType point, double counterclockwiseRadians) {
      numericType x = point.X;
      numericType y = point.Y;
      numericType x1 = x * (numericType)Math.Cos(counterclockwiseRadians) - y * (numericType)Math.Sin(counterclockwiseRadians);
      numericType y1 = x * (numericType)Math.Sin(counterclockwiseRadians) + y * (numericType)Math.Cos(counterclockwiseRadians);
      PointType r = new PointType(x1, y1);
      return r;
    }
    public static numericType Times(this PointType point1, PointType point2) {
      // dot product
      numericType r = point1.X * point2.X + point1.Y * point2.Y;
      return r;
    }
    public static PointType MultiplyAndRound(this PointType point, numericType scalar) {
      return new PointType(point.X.MultiplyAndRound(scalar), point.Y.MultiplyAndRound(scalar));
    }
    public static string ToShortString(this PointType This) {
      string r = (int)This.X + " " + (int)This.Y;
      return r;
    }
    public static string Encoding(this PointType point) {
      numericType x = point.X;
      numericType y = point.Y;
      string xString = x.ToString("R");
      string yString = y.ToString("R");
      string r = xString + " " + yString;
      return r;
    }
    public static PointType Decode (string encodedPoint) {
      char[] space = " ".ToCharArray();
      string[] split = encodedPoint.Split(space, StringSplitOptions.RemoveEmptyEntries);
      PointType r = new PointType ();
      int splitCount = split.Count();
      if (splitCount > 0) {
        r.X = FloatAdditions.TryParse(split[0]);
      }
      if (splitCount > 1) {
        r.Y = FloatAdditions.TryParse(split[1]);
      }
      return r;
    }
    public static numericType Distance(this PointType p1, PointType p2) {
      numericType r = p1.Minus (p2).Length ();
      return r;
    }
    public static bool IsZero(this PointType p) {
      bool r = (p.X == 0 && p.Y == 0);
      return r;
    }
    public static int CompareRowMajor(this PointType p, PointType q) {
      int r = p.Y.CompareTo(q.Y);
      if (r == 0) {
        r = p.X.CompareTo(q.X);
      }
      return r;
    }
    public static IEnumerable<PointType> Plus(this IEnumerable<PointType> points, PointType other) {
      foreach(PointType point in points) {
        yield return point.Plus(other);
      }
    }
    public static IEnumerable<PointType> Minus(this IEnumerable<PointType> points, PointType other) {
      foreach (PointType point in points) {
        yield return point.Minus(other);
      }
    }
    public static bool EqualsPoint(this PointType point, PointType other)
    {
      bool r = point.X == other.X && point.Y == other.Y;
      return r;
    }
    public static IEnumerable<float> ToFloatEnumerable(this PointType point) {
      yield return point.X;
      yield return point.Y;
    }
    public static IEnumerable<double> ToDoubleEnumerable(this PointType point) {
      yield return (double)point.X;
      yield return (double)point.Y;
    }
    public static PointType FromDoubleEnumerable(IEnumerable<double> enumerable) {
      if (enumerable.SafeCount() < 2) {
        return PointFAdditions.NaN;
      } else {
        List<double> list = enumerable.ToSafeList(null, 2);
        return new PointType((numericType)list[0], (numericType)list[1]);
      }
    }
    public static PointType FromFloatEnumerable(IEnumerable<float> enumerable) {
      if (enumerable.SafeCount() < 2) {
        return PointFAdditions.NaN;
      } else {
        List<float> list = enumerable.ToSafeList(null, 2);
        return new PointType(list[0], list[1]);
      }
    }
  }
}
