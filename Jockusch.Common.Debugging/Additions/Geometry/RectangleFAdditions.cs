using System;
using System.Drawing;
using RectangleClass = System.Drawing.RectangleF;
using PointClass = System.Drawing.PointF;
using ThicknessClass = Jockusch.Common.ThicknessF;
using ScalarClass = System.Single;
using System.Collections.Generic;
using System.Linq;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class RectangleFAdditions
  {
    /* If small is null, returns TRUE.  If big is null and small is not null, returns FALSE.
     * Additionally, As of January, 2014, RectangleF.Contains gives some incorrect answers.  See https://bugzilla.xamarin.com/show_bug.cgi?id=15518
    */
    public static ScalarClass MinSide(this RectangleClass rect) {
      return rect.Size.MinSide();
    }
    /// <summary>Not rounded.</summary>
    public static ScalarClass ProportionalX(this RectangleF rect, ScalarClass proportion) {
      ScalarClass x = rect.X;
      ScalarClass width = rect.Width;
      ScalarClass r = x + width * proportion;
      return r;
    }
    /// <summary>Not rounded.</summary>
    public static ScalarClass ProportionalY(this RectangleF rect, ScalarClass proportion) {
      ScalarClass y = rect.Y;
      ScalarClass height = rect.Height;
      ScalarClass r = y + height * proportion;
      return r;
    }
    public static ScalarClass XMid(this RectangleF rect) {
      return rect.ProportionalX(0.5f);
    }
    public static ScalarClass YMid(this RectangleF rect) {
      return rect.ProportionalY(0.5f);
    }
    /// <summary>Inner proportions tell us where within the inner rectangle the nail goes.  Outer proportions tell us where within the outer rectangle.
    /// Returned rectangle is the resulting location of the inner rectangle.</summary>
    public static RectangleF WithNail(this RectangleF outerRect, SizeF innerSize, ScalarClass nailOuterX, ScalarClass nailOuterY,
      ScalarClass nailInnerX, ScalarClass nailInnerY) {
      ScalarClass x = outerRect.ProportionalX(nailOuterX);
      ScalarClass y = outerRect.ProportionalY(nailOuterY);
      RectangleF r = RectangleFAdditions.RectangleRelativeToPoint(x, y, innerSize.Width, innerSize.Height, nailInnerX, nailInnerY);
      return r;
    }
    /// <summary>Nails the point in the size at proportions (nailMyX, nailMyY) to the external point
    /// (nailExternalX, nailExternalY). Therefore, the "my" coords are generally in the range 0-1, while the "external"
    /// coords are whatever point.</summary>
    public static RectangleF Nailed(this SizeF size, ScalarClass nailMyX, ScalarClass nailMyY, ScalarClass nailExternalX, ScalarClass nailExternalY) {
      ScalarClass width = size.Width;
      ScalarClass height = size.Height;
      ScalarClass dx = width * nailMyX;
      ScalarClass dy = height * nailMyY;
      RectangleF r = new RectangleF(nailExternalX - dx, nailExternalY - dy, width, height);
      return r;
    }


    public static RectangleClass FromPoints(ScalarClass x, ScalarClass y, ScalarClass x1, ScalarClass y1) {
      ScalarClass xMin = Math.Min(x, x1);
      ScalarClass yMin = Math.Min(y, y1);
      ScalarClass xMax = Math.Max(x, x1);
      ScalarClass yMax = Math.Max(y, y1);
      RectangleClass r = new RectangleClass(xMin, yMin, xMax - xMin, yMax - yMin);
      return r;
    }

    public static RectangleClass FromPoints(PointClass p1, PointClass p2) {
      return RectangleFAdditions.FromPoints(p1.X, p1.Y, p2.X, p2.Y);
    }

    public static RectangleClass FromCenterAndSize(PointClass center, SizeF size) {
      PointClass location = center.Minus(size.Times(0.5f).ToPointF());
      RectangleClass r = new RectangleF(location, size);
      return r;
    }
    /// <summary>If the small rect is NaN, we return that the big one DOES contain it.
    /// In other words, we treat NaN as an empty rectangle that is contained within everything.
    /// This is different from how System.RectangleF.Contains works, and the code does use the difference.</summary>
    public static bool ContainsNaNAware(this RectangleClass big, RectangleClass small) {
      bool r;
      if (small.IsNaN()) {
        r = true;
      } else {
        r = false;
        if (big.Left <= small.Left) {
          if (big.Top <= small.Top) {
            if (big.Bottom >= small.Bottom) {
              if (big.Right >= small.Right) {
                r = true;
              }
            }
          }
        }
      }
      return r;
    }

    public static bool Contains(this RectangleClass big, RectangleClass small, float slop) {
      RectangleClass bigger = big.Inset(-slop);
      bool r = bigger.Contains(small);
      return r;
    }

    public static RectangleClass RectangleRelativeToPoint(ScalarClass x, ScalarClass y, ScalarClass width, ScalarClass height, ScalarClass xProportion, ScalarClass yProportion) {
      // The proportions show where within our rect to put the point (x, y).  For example, if xProportion and yProportion are both zero, this
      // becomes the usual constructor where (x,y) is the top-left corner.  If they are both 1, then (x, y) is the bottom-right corner, and
      // if they are 1/2, (x, y) becomes the center.
      ScalarClass rectX = x - width * xProportion;
      ScalarClass rectY = y - height * yProportion;
      RectangleClass r = new RectangleClass(rectX, rectY, width, height);
      return r;
    }

    public static RectangleClass UnionNanAware(params RectangleClass[] rects) {
      ScalarClass left = ScalarClass.NaN;
      ScalarClass top = ScalarClass.NaN;
      ScalarClass right = ScalarClass.NaN;
      ScalarClass bottom = ScalarClass.NaN;
      foreach (RectangleClass rect in rects) {
        left = FloatAdditions.Min(left, rect.Left);
        right = FloatAdditions.Max(right, rect.Right);
        top = FloatAdditions.Min(top, rect.Top);
        bottom = FloatAdditions.Max(bottom, rect.Bottom);
      }
      bool anyNan = ScalarClass.IsNaN(left) || ScalarClass.IsNaN(top) || ScalarClass.IsNaN(right) || ScalarClass.IsNaN(bottom);
      RectangleClass r;
      if (anyNan) {
        r = RectangleFAdditions.NaN;
      } else {
        ScalarClass width = right - left;
        ScalarClass height = bottom - top;
        r = new RectangleClass(left, top, width, height);
      }
      return r;
    }

    public static RectangleClass UnionNanAware(IEnumerable<RectangleClass> childRects) {
      return RectangleFAdditions.UnionNanAware(childRects.ToArray());
    }

    public static RectangleClass NaN {
      get {
        ScalarClass nan = ScalarClass.NaN;
        RectangleClass r = new RectangleClass(nan, nan, nan, nan);
        return r;
      }
    }
    public static bool IsNaN(this RectangleClass rect) {
      if (ScalarClass.IsNaN(rect.X)
        || ScalarClass.IsNaN(rect.Y)
        || ScalarClass.IsNaN(rect.Width)
        || ScalarClass.IsNaN(rect.Height)) {
        return true;
      }

      return false;
    }
    public static bool IsFinite(this RectangleF rect) {
      bool loc = rect.Location.IsFinite();
      bool size = rect.Size.IsFinite();
      bool r = loc && size;
      return r;
    }
    public static bool IsNaNOrEmpty(this RectangleClass rect) {
      bool r = rect.IsNaN() || rect.IsEmpty;
      return r;
    }
    public static ScalarClass MinDimension(this RectangleClass rect) {
      ScalarClass width = rect.Width;
      ScalarClass height = rect.Height;
      ScalarClass r = Math.Min(width, height);
      return r;
    }

    public static ScalarClass MaxDimension(this RectangleClass rect) {
      ScalarClass width = rect.Width;
      ScalarClass height = rect.Height;
      ScalarClass r = Math.Max(width, height);
      return r;
    }
    public static string ToShortString(this RectangleClass rect) {
      ScalarClass x = rect.X;
      ScalarClass y = rect.Y;
      ScalarClass width = rect.Width;
      ScalarClass height = rect.Height;
      string r = x.ToString() + " " + y.ToString() + " " + width.ToString() + " " + height.ToString();
      return r;
    }
    public static bool AllFinite(this RectangleClass rect) {
      bool r = FloatAdditions.AllFinite(rect.X, rect.Y, rect.Width, rect.Height);
      return r;
    }
    public static Rectangle IntegralRect(this RectangleClass rect) {
      return rect.IntegralRectangle().ToRect();
    }
    public static RectangleClass IntegralRectangle(this RectangleClass rect) {
      /* Smallest rectangle that wraps this and has all integer coordinates.  Exceptions:
       * 1.  If width or height is zero, it remains zero.
       * 2.  If any measurement is not a finite ScalarClass, the whole rect is left unchanged.
       */
      ScalarClass x = rect.X;
      ScalarClass y = rect.Y;
      ScalarClass width = rect.Width;
      ScalarClass height = rect.Height;
      bool allFinite = FloatAdditions.AllFinite(x, y, width, height);
      if (!allFinite) {
        return rect;
      }
      if (width == 0 || height == 0) {
        return rect;
      }
      ScalarClass right = rect.Right;
      ScalarClass bottom = rect.Bottom;
      ScalarClass intX = (ScalarClass)Math.Floor(x);
      ScalarClass intY = (ScalarClass)Math.Floor(y);
      ScalarClass intBottom = (ScalarClass)Math.Ceiling(bottom);
      ScalarClass intRight = (ScalarClass)Math.Ceiling(right);
      ScalarClass intWidth = intRight - intX;
      ScalarClass intHeight = intBottom - intY;
      RectangleClass r = new RectangleClass(intX, intY, intWidth, intHeight);
      return r;
    }
    public static Rectangle ToRect(this RectangleClass rect) {
      RectangleClass integral = rect.IntegralRectangle();
      int x = (int)integral.X;
      int y = (int)integral.Y;
      int width = (int)integral.Width;
      int height = (int)integral.Height;
      Rectangle r = new Rectangle(x, y, width, height);
      return r;
    }
    /// <summary>Handling of special cases:
    /// If the rect has width or height less than or equal to zero, or not finite, this method simply returns
    /// the original rect. However, if the rect is initially positive in both width and height, and the thickness
    /// reduces either or both of these numbers to a negative number, it is allowed to happen.</summary>
    public static RectangleClass Inset(this RectangleClass rect, ThicknessClass thickness) {
      RectangleClass r;
      if (rect.AllFinite() && rect.Width != 0 && rect.Height != 0) {
        ScalarClass x = rect.X + thickness.Left;
        ScalarClass y = rect.Y + thickness.Top;
        ScalarClass width = Math.Max(0, rect.Width - thickness.LinearWidth());
        ScalarClass height = Math.Max(0, rect.Height - thickness.LinearHeight());
        r = new RectangleClass(x, y, width, height);
      } else {
        r = rect;
      }
      return r;
    }
    public static RectangleClass Inset(this RectangleClass rect, ScalarClass delta) {
      RectangleClass r = rect.Inset(new ThicknessClass(delta));
      return r;
    }
    public static RectangleClass Inset(this RectangleClass rect, ScalarClass dx, ScalarClass dy) {
      ThicknessClass thickness = new ThicknessClass(dx, dy);
      RectangleClass r = rect.Inset(thickness);
      return r;
    }
    /// <summary> If we call the returned thickness thick, then rect.Inset(thick) == otherRect. </summary>
    public static ThicknessClass InsetToRect(this RectangleClass rect, RectangleClass otherRect) {
      ScalarClass dLeft = otherRect.Left - rect.Left;
      ScalarClass dTop = otherRect.Top - rect.Top;
      ScalarClass dRight = rect.Right - otherRect.Right;
      ScalarClass dBottom = rect.Bottom - otherRect.Bottom;
      ThicknessClass r = new ThicknessClass(dLeft, dTop, dRight, dBottom);
      return r;
    }
    public static RectangleClass Plus(this RectangleClass rect, PointClass vector) {
      PointClass translatedLocation = rect.Location.Plus(vector);
      RectangleClass r = new RectangleClass(translatedLocation, rect.Size);
      return r;
    }
    public static RectangleClass Minus(this RectangleClass rect, PointClass vector) {
      RectangleClass r = rect.Plus(vector.Times(-1));
      return r;
    }
    public static RectangleClass Times(this RectangleClass rect, ScalarClass scalar) {
      RectangleClass r = new RectangleClass(scalar * rect.X, scalar * rect.Y, scalar * rect.Width, scalar * rect.Height);
      return r;
    }
    public static RectangleClass TimesXY(this RectangleClass rect, ScalarClass xScalar,
      ScalarClass yScalar, bool roundToIntegers) {
      PointClass location = rect.Location;
      PointClass bottomRight = rect.BottomRight();
      PointClass locationOut = location.TimesXY(xScalar, yScalar, roundToIntegers);
      PointClass bottomRightOut = bottomRight.TimesXY(xScalar, yScalar, roundToIntegers);
      RectangleClass r = RectangleFAdditions.FromPoints(locationOut, bottomRightOut);
      return r;
    }
    public static PointClass BottomRight(this RectangleClass rect) {
      return new PointClass(rect.Right, rect.Bottom);
    }
    public static PointClass TopRight(this RectangleClass rect) {
      return new PointClass(rect.Right, rect.Top);
    }
    public static PointClass BottomLeft(this RectangleClass rect) {
      return new PointClass(rect.Left, rect.Bottom);
    }
    public static RectangleClass ToOrigin(this RectangleClass rect) {
      rect.X = 0;
      rect.Y = 0;
      return rect;
    }
    public static IEnumerable<PointClass> Corners(this RectangleClass rect) {
      ScalarClass x = rect.X;
      ScalarClass y = rect.Y;
      ScalarClass width = rect.Width;
      ScalarClass height = rect.Height;
      yield return new PointClass(x, y);
      yield return new PointClass(x + width, y);
      yield return new PointClass(x, y + height);
      yield return new PointClass(x + width, y + height);
    }
    public static RectangleF Intersection(RectangleF firstRect, params RectangleF[] additionalRects) {
      RectangleF r = firstRect;
      foreach (RectangleF rect in additionalRects) {
        r.Intersect(rect);
      }
      return r;
    }
    public static RectangleF IntersectY(this RectangleF rect, float minY, float maxY) {
      float y = Math.Max(minY, rect.Y);
      float bottom = Math.Min(rect.Bottom, maxY);
      rect.Y = y;
      rect.Height = bottom - y;
      return rect;
    }
    public static RectangleF FlipY(this RectangleF parent, RectangleF child) {
      RectangleF r;
      if (parent.IsSane()) {
        float y = parent.Height - child.Bottom;
        r = new RectangleF(child.X, y, child.Width, child.Height);
      } else {
        r = child;
      }
      return r;
    }
    public static PointClass FlipY(this RectangleF parent, PointClass child) {
      PointClass r;
      if (parent.IsSane()) {
        float y = parent.Height - child.Y;
        r = new PointClass(child.X, y);
      } else {
        r = child;
      }
      return r;
    }
    /// <summary>Returns zero if the point is inside the rectangle. Otherwise, returns the square of the distance
    /// from the point to the nearest point on the rectangle.</summary>
    public static ScalarClass DistanceSquaredFromPoint(this RectangleClass rect, PointClass point) {
      ScalarClass r;
      if (rect.IsNaN() || point.IsNaN()) {
        r = ScalarClass.NaN;
      } else {
        ScalarClass dx = FloatAdditions.Max(0, point.X - rect.Right, rect.Left - point.X);
        ScalarClass dy = FloatAdditions.Max(0, point.Y - rect.Y - rect.Height, rect.Y - point.Y);
        r = dx * dx + dy * dy;
      }
      return r;
    }
    public static float FlipY(this RectangleF parent, float y) {
      float r;
      if (parent.IsSane()) {
        r = parent.Bottom - y;
      } else {
        r = y;
      }
      return r;
    }
    public static bool ReallyIntersectsWith(this RectangleF rect1, RectangleF rect2) {
      bool r = false;
      if (!(rect1.IsNaNOrEmpty() || rect2.IsNaNOrEmpty())) {
        r = rect1.IntersectsWith(rect2);
      }
      return r;
    }
    public static RectangleClass TranslateToBeInsideOf(this RectangleClass rect1, SizeF size) {
      RectangleClass rect2 = size.ToRectangleF();
      RectangleClass r = rect1.TranslateToBeInsideOf(rect2);
      return r;
    }
    public static RectangleClass TranslateToBeInsideOf(this RectangleClass rect1, RectangleClass rect2) {
      rect1.Width = Math.Min(rect1.Width, rect2.Width);
      rect1.Height = Math.Min(rect1.Height, rect2.Height);
      rect1.X = rect1.X.RestrictToRange(rect2.X, rect2.Right - rect1.Width);
      rect1.Y = rect1.Y.RestrictToRange(rect2.Y, rect2.Bottom - rect1.Height);
      return rect1;
    }
    public static ScalarClass Area(this RectangleClass rect) {
      return rect.Width * rect.Height;
    }
    #region debugging
    /// <summary>x or y beyond the tooBig value does not raise any alarm with this method.</summary>
    public static bool IsSane(this RectangleClass rect, bool zeroWidthHeightOK = false, ScalarClass widthHeightTooBig = NumericalTolerances.ScreenWidthOrHeightTooBig) {
      bool sizeSane = rect.Size.IsSane(zeroWidthHeightOK, widthHeightTooBig);
      bool locationSane = rect.Location.IsSane();
      bool r = sizeSane && locationSane;
      return r;
    }
    #endregion
  }
}
