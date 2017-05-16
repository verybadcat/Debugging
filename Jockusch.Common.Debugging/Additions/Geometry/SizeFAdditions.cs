using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using SizeType = System.Drawing.SizeF;
using scalarType = System.Single;

namespace Jockusch.Common
{
  public static class SizeFAdditions
  {
    public static SizeF Union (this SizeF size1, SizeF size2) {
      float width1 = size1.Width;
      float width2 = size2.Width;
      float height1 = size1.Height;
      float height2 = size2.Height;
      float width = (width2 > width1) ? width2 : width1; // not the same as Math.Max if width2 is NaN.
      float height = (height2 > height1)?height2:height1;
      SizeF r = new SizeF (width, height);
      return r;
    }
    public static string ToDebugString (this SizeF size) {
      float width = size.Width;
      float height = size.Height;
      string r = width.ToString () + " " + height.ToString ();
      return r;
    }
    public static SizeF BigSize {
      get {
        float max = float.MaxValue;
        SizeF r = new SizeF (max, max);
        return r;
        
      }
    }
		public static RectangleF ToRectangleF(this SizeF size) {
      RectangleF r = new RectangleF(0, 0, size.Width, size.Height);
      return r;
    }
    public static RectangleF Inset(this SizeF size, float delta) {
      return size.ToRectangleF().Inset(delta);
    }
		public static SizeF Plus(this SizeF size, SizeF otherSize) {
			float width = size.Width + otherSize.Width;
			float height = size.Height + otherSize.Height;
			SizeF r = new SizeF (width, height);
			return r;
		}
    public static SizeF Plus(this SizeF size, ThicknessF thickness) {
      SizeF otherSize = thickness.LinearSize();
      SizeF r = size.Plus(otherSize);
      return r;
    }
    public static SizeF Plus(this SizeF size, float dx, float dy) {
      return new SizeF(size.Width + dx, size.Height + dy);
    }
		public static SizeF Minus (this SizeF size, SizeF otherSize) {
			float width = size.Width - otherSize.Width;
			float height = size.Height - otherSize.Height;
			SizeF r = new SizeF (width, height);
			return r;
		}
    public static SizeF Minus(this SizeF size, ThicknessF thickness) {
      SizeF otherSize = thickness.LinearSize();
      SizeF r = size.Minus(otherSize);
      return r;
    }
		public static SizeF Times (this SizeF size, float scalar) {
			float width = scalar * size.Width;
			float height = scalar * size.Height;
			SizeF r = new SizeF (width, height);
			return r;
		}
    public static SizeF MultiplyAndRound(this SizeF size, float scalar) {
      float width = size.Width.MultiplyAndRound(scalar);
      float height = size.Height.MultiplyAndRound(scalar);
      SizeF r = new SizeF(width, height);
      return r;
    }
    public static float MinSide(this SizeF size) {
      float width = size.Width;
      float height = size.Height;
      float r = Math.Min (width, height);
      return r;
    }
    public static float MaxSide(this SizeF size) {
      float width = size.Width;
      float height = size.Height;
      float r = Math.Max(width, height);
      return r;
    }
    public static SizeF Union (this SizeF size, params SizeF[] otherSizes) {
      float width = size.Width;
      float height = size.Height;
      foreach (SizeF otherSize in otherSizes) {
        float otherWidth = otherSize.Width;
        float otherHeight = otherSize.Height;
        if (otherWidth > width) {
          width = otherWidth;
        }
        if (otherHeight > height) {
          height = otherHeight;
        }
      }
      SizeF r = new SizeF (width, height);
      return r;
    }
    public static SizeF Intersection (this SizeF size, params SizeF[] otherSizes) {
      float width = size.Width;
      float height = size.Height;
      foreach (SizeF otherSize in otherSizes) {
        float otherWidth = otherSize.Width;
        float otherHeight = otherSize.Height;
        if (otherWidth < width) {
          width = otherWidth;
        }
        if (otherHeight < height) {
          height = otherHeight;
        }
      }
      SizeF r = new SizeF (width, height);
      return r;
    }
    public static SizeF CombineInRow (List<SizeF> sizes) {
      SizeF r = new SizeF();
      foreach (SizeF size in sizes) {
        if (!size.IsNaN()) {
          r.Width = r.Width + size.Width;
          r.Height = Math.Max(r.Height, size.Height);
        }
      }
      return r;
    }
    public static SizeF CombineInRow (params SizeF[] sizes) {
      SizeF r = SizeFAdditions.CombineInRow(sizes.ToList());
      return r;
    }
    public static SizeF CombineInColumn (List<SizeF> sizes) {
      SizeF r = new SizeF();
      foreach (SizeF size in sizes) {
        r.Height = r.Height + size.Height;
        r.Width = Math.Max(r.Width, size.Width);
      }
      return r;
    }
    public static SizeF CombineInColumn (params SizeF[] sizes) {
      SizeF r = SizeFAdditions.CombineInColumn(sizes.ToList());
      return r;
    }
    public static RectangleF RectangleWithNail (this SizeF outerSize, SizeF innerSize, float nailOuterX,
      float nailOuterY, float nailInnerX, float nailInnerY) {
      RectangleF outerRect = outerSize.ToRectangleF();
      RectangleF r = outerRect.WithNail(innerSize, nailOuterX, nailOuterY, nailInnerX, nailInnerY);
      return r;
    }
    /// <summary>Square is considered landscape.</summary>
    public static bool IsPortrait(this SizeF size) {
      return (size.Width < size.Height);
    }
    /// <summary> Returns true if width * ratio <= height. So a ratio of 0 would
    /// always be portrait.</summary>
    public static bool IsPortraitWithRatio(this SizeF size, float ratio) {
      return (size.Width * ratio <= size.Height);
    }
    /// <summary>Square is considered landscape.</summary>
    public static bool IsLandscape(this SizeF size) {
      return (size.Width >= size.Height);
    }
    public static SizeF NaN {
      get {
        return new SizeF(float.NaN, float.NaN);
      }
    }
    public static bool IsNaN(this SizeF size) {
      bool r = float.IsNaN(size.Width) || float.IsNaN(size.Height);
      return r;
    }
    public static bool IsFinite(this SizeF size) {
      bool r = (size.Width.IsFiniteFloat() && size.Height.IsFiniteFloat());
      return r;
    }
    public static SizeType Transpose(this SizeType size) {
      return new SizeF(size.Height, size.Width);
    }
    public static SizeF ToIntegralSizeF(this SizeF size) {
      float intWidth = size.Width.RoundToFloat();
      float intHeight = size.Height.RoundToFloat();
      return new SizeF(intWidth, intHeight);
    }

    public static SizeF FromDoubles(double width, double height) {
      return new SizeF(width.ToFloat(), height.ToFloat());
    }

    public static SizeF ResolveInsaneTo(this SizeF size, float widthIfInsane, float heightIfInsane, 
      bool zeroOK = false, float maxDimension = NumericalTolerances.ScreenWidthOrHeightTooBig) {
      bool sane = size.IsSane(zeroOK, maxDimension);
      SizeF r;
      if (sane) {
        r = size;
      } else {
        r = new SizeF(widthIfInsane, heightIfInsane);
      }
      return r;
    }
    public static bool IsSane(this SizeF size, bool zeroOK = false, float maxDimension = NumericalTolerances.ScreenWidthOrHeightTooBig) {
      float width = size.Width;
      float height = size.Height;
      bool bigEnough = (width >= 0 && height >= 0);
      if (!zeroOK) {
        if (width == 0 || height == 0) {
          bigEnough = false;
        }
      }
      bool r = (bigEnough && width < maxDimension && height < maxDimension);
      return r;
    }
    public static RectangleF RectangleAbove(this SizeF size, float y) {
      return new RectangleF(0, 0, size.Width, y);
    }
    public static RectangleF RectangleBelow(this SizeF size, float y) {
      return new RectangleF(0, y, size.Width, size.Height - y);
    }
    public static RectangleF RectangleToTheLeft(this SizeF size, float x) {
      return new RectangleF(0, 0, x, size.Height);
    }
    public static RectangleF RectangleToTheRight(this SizeF size, float x) {
      return new RectangleF(x, 0, size.Width - x, size.Height);
    }
    public static RectangleF LargestRectNotContaining(this SizeF size, RectangleF rect) {
      RectangleF left = size.RectangleToTheLeft(rect.X);
      RectangleF above = size.RectangleAbove(rect.Y);
      RectangleF right = size.RectangleToTheRight(rect.Right);
      RectangleF below = size.RectangleBelow(rect.Bottom);
      List<RectangleF> list = new List<RectangleF> { right, left, above, below};
      RectangleF r = RectangleF.Empty;
      float area = 0;
      foreach (RectangleF option in list) {
        if (option.Area() > area) {
          r = option;
          area = option.Area();
        }
      }
      return r;
    }
  }
}

