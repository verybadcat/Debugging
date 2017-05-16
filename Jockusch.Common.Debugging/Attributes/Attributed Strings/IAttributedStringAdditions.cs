using System;
using Jockusch.Common;
using System.Drawing;
using Jockusch.Common.Debugging;
using System.Collections.Generic;


namespace Jockusch.Common
{
  public static class IAttributedStringAdditions
  {
    #region methods which have been moved up to the interface
    /// <summary></summary> quick-and-dirty call that avoids expensive Measure methods.</summary>
    public static bool MightFitOnSingleLine(this IAttributedString aString, float width, bool useDrawingFont) {
      bool r = true;
      if (width.IsFiniteFloat()) {
        string text = aString.Text;
        float pointSize = aString.GetFont(useDrawingFont).GetPointSize();
        float product = text.Length * pointSize;
        r = (product < 3 * width);
      }
      return r;
    }
    public static bool IsSingleLine(this IAttributedString aString, float maxWidth, bool useDrawingFont) {
      bool r = false;
      if (aString.HasDrawableText()) {
        r = aString.MightFitOnSingleLine(maxWidth, useDrawingFont);
        if (r) {
          TextSizing ts = TextSizing.Current;
          RectangleF rect = ts.RawBoundingRectangle(aString, useDrawingFont);
          r = (rect.Right.RoundUpToInt() - rect.Left.RoundDownToFloat() <= maxWidth);
        }
      }
      return r;
    }

    public static bool HasDrawableText(this IAttributedString aString) {
      bool r = false;
      if (aString != null) {
        if (!(string.IsNullOrWhiteSpace(aString.Text))) {
          if (aString.GetDrawingFontSize(true) > 0) {
            if (aString.GetTextColor().A > 0) {
              r = true;
            }
          }
        }
        return r;
      }
      return r;
    }
    public static bool IsEmpty(this IAttributedString aString) {
      int myLength = aString.Length();
      bool r = (myLength == 0);
      return r;
    }
    public static int Length(this IAttributedString aString) {
      string text = aString?.Text;
      int r = 0;
      if (text != null) {
        r = text.Length;
      }
      return r;
    }
    public static void ClearDrawingFontSize(this IAttributedString aString) {
      aString.SetDrawingFontSize(float.NaN);
    }
    public static string TextNotNull(this IAttributedString aString) {
      string myText = aString.Text;
      if (myText == null) {
        return "";
      } else {
        return myText;
      }
    }

    #endregion

    public static void SetTextWithoutClearingDrawingFontSize(this IAttributedString aString, string text) {
      float size = aString.GetDrawingFontSize(false);
      aString.Text = text;
      aString.SetDrawingFontSize(size);
    }


    public static IAttributedString TruncateForHeight(this IAttributedString s, RectangleF outerRect, bool useDrawingFont) {
      IAttributedString r;
      TextSizing ts = WJOperatingSystem.Current.TextSizing;
      SizeF size = ts.RawBoundingRectangle(s, useDrawingFont, outerRect.Width).Size;
      float height = size.Height;
      if (height <= outerRect.Height) {
        r = s;
      } else {
        // have to truncate
        int length = s.Length();
        r = s.TruncateForHeight(outerRect, useDrawingFont, 0, length, length / 2);
      }
      return r;
    }
    private static IAttributedString TruncateForHeight(this IAttributedString s, RectangleF outerRect, bool useDrawingFont,
                                                      int goodLength, int badLength, int testLength, AttributedString r = null) {
      if (r == null) {
        r = new AttributedString(s);
      }
      if (goodLength + 1 >= badLength) {
        string targetText = s.Text.Substring(0, goodLength);
        r.SetTextWithoutClearingDrawingFontSize(targetText);
      } else {
        string targetText = s.Text.Substring(0, testLength);
        r.SetTextWithoutClearingDrawingFontSize(targetText);
        TextSizing ts = WJOperatingSystem.Current.TextSizing;
        float rHeight = ts.BoundingHeight(r, useDrawingFont, outerRect.Width);
        if (rHeight <= outerRect.Height) { // testLength is good
          int goodTestLength = (testLength + badLength + 1) / 2;
          s.TruncateForHeight(outerRect, useDrawingFont, testLength, badLength, goodTestLength, r);
        } else {
          int badTestLength = (testLength + goodLength) / 2;
          s.TruncateForHeight(outerRect, useDrawingFont, goodLength, testLength, badTestLength, r);
        }
      }
      return r;
    }
    /// <summary>The attributed string here is relevant only for the horizontal and vertical alignments. The text
    /// plays no role; that's why we don't need a TextSizing object. The returned point is aligned as specified
    /// wrt the rect.</summary>
    public static PointF GetAlignmentPoint(this IAttributedString aText, RectangleF outerRect) {
      HorizontalAlignmentEnum horizontalAlignment = aText.GetHorizontalAlignment();
      VerticalAlignmentEnum verticalAlignment = aText.GetVerticalAlignment();
      float xFactor = horizontalAlignment.Multiplier();
      float yFactor = verticalAlignment.Multiplier();
      float x = outerRect.X;
      float y = outerRect.Y;
      float outerWidth = outerRect.Width;
      float outerHeight = outerRect.Height;
      float drawX = x + xFactor * outerWidth;
      float drawY = y + yFactor * outerHeight;
      PointF r = new PointF(drawX, drawY);
      return r;
    }
    public static IAttributedString MovedAttributedString(this IAttributedString aString, PointF dxy, bool forceNewObject = false) {
      IAttributedString r = null;
      if (!forceNewObject && (dxy == new PointF() || aString.Location.IsNaN())) {
        r = aString;
      }
      r = r ?? new AttributedString(aString);
      if (!(r.Location.IsNaN())) {
        r.Location = r.Location.Plus(dxy);
      }
      return r;
    }
    public static AttributedString AttributedSubstring(this IAttributedString aString, int fromIndex, int toIndex) {
      AttributedString r = new AttributedString(aString);
      r.SetTextWithoutClearingDrawingFontSize(aString.Text.Substring(fromIndex, toIndex));
      return r;
    }

    public static bool ChangeDrawingFontSize(this IAttributedString aString, float delta) {
      bool r = false;
      float oldSize = aString.GetDrawingFontSize(true);
      if (oldSize > 2 + delta) {
        aString.SetDrawingFontSize(oldSize + delta);
        r = true;
      }
      return r;
    }
  }
}
