using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jockusch.Common.Debugging;
using Jockusch.Common.Notifications;

namespace Jockusch.Common
{

  public abstract class TextSizing 
  {
    /* "Raw" methods return unrounded values.  Methods without "Raw" in the name
     * return values rounded to an integer, typically rounded up.
     * The intention is that front ends should only need to implement raw
     * methods; the rest should be handled in the core. One quirk:
     * To get the bounding height, we round up the ascent and descent, then
     * add.  So the increase could be any amount between 0 and 2.*/
    /// <summary>If we draw the string with the baseline at y=0 and starting at x=0, this is the bounding rectangle, based on the font, ascent, and descent.  (Not the image bounds).</summary>
    public abstract RectangleF RawBoundingRectangle(IAttributedString s, bool useDrawingFont, float maxWidth = float.NaN);
    public abstract float RawAscent(CommonFont font);
    public abstract float RawDescent(CommonFont font);
    public virtual float CursorAdvance(IAttributedString aString) {
      float r = this.BoundingWidth(aString, true);
      return r;
    }
    protected TextSizing(bool clearCacheOnFontSizeChange) {
      if (clearCacheOnFontSizeChange) {
        WJNotificationCenter center = WJNotificationCenter.DefaultCenter;
        center.AddObserver(this, NotificationKey.FontSizesChanged, () => this.ClearCache());
      }
    }
    public virtual void ClearCache() { }
    public static TextSizing Current {
      get {
        return WJOperatingSystem.TS;
      }
    }
  }

  public static class TextSizingAdditions
  {
    /// <summary>This method either shrinks for width or it doesn't. There is no middle ground. IDK
    /// why I made that choice.  The out isSingleLine parameter gets set to whether or not the text,
    /// drawn at the returned font in the passed-in size, will be on a single line. This method also
    /// USUALLY CLEARS THE DRAWINGFONTSIZE OF THE PASSED-IN IATTRIBUTEDSTRING OBJECT.</summary>
    public static SizeF BoundingSizeWithSingleLineShrinkage(this TextSizing textSizing, IAttributedString aText,
                                                            SizeF size, float ratio, out bool isSingleLine) {
      SizeF r;
      isSingleLine = true;
      if (aText.IsEmpty()) {
        r = new SizeF();
      } else {
        float width = size.Width;
        float height = size.Height;
        RectangleF rawSingleLineRect = textSizing.BoundingRectangle(aText, false);
        float rawWidth = rawSingleLineRect.Width;
        float rawHeight = rawSingleLineRect.Height;
        bool fits = (rawWidth <= width && rawHeight <= height);
        if (fits) {
          r = rawSingleLineRect.Size;
        } else {
          CommonFont font = aText.GetFont(false);
          if (font == null) {
            r = new SizeF();
            CommonDebug.BreakPoint();
          } else {
            float baseFontSize = font.GetPointSize();
            int widthFontSize = (baseFontSize * ratio).RoundToInt();
            float safeHeight = (rawHeight > 0) ? rawHeight : 1;
            int minFontSize = widthFontSize;
            if (height < safeHeight) {  // i.e, if the available height is less than the height of the font
              int heightFontSize = (int)(baseFontSize * height / safeHeight);
              minFontSize = Math.Min(widthFontSize, heightFontSize);
            }
            aText.SetDrawingFontSize(minFontSize);
            isSingleLine = aText.MightFitOnSingleLine(width, true);
            r = new SizeF(); // dummy value to shut compiler up.
            bool handled = false;
            if (isSingleLine) {
              SizeF singleLineSize = textSizing.RawBoundingRectangle(aText, true).Size;
              float singleLineWidth = singleLineSize.Width;
              isSingleLine = (singleLineWidth <= width); // i.e., at the shrunk size, we can do it on a single line.
              if (isSingleLine) {
                //      private static int FontSizeThatFits(this AttributedString aString, SizeF available, int maxDepth, 
                //        int badFontSize, int goodFontSize, int testFontSize, bool singleLine);
                aText.ComputeSingleLineDrawingFontSize(size, textSizing);
                SizeF outputSize = textSizing.BoundingSize(aText, true);
                float outputHeight = Math.Min(outputSize.Height, height);
                r = new SizeF(outputSize.Width, outputHeight);
                handled = true;
              }
            }
            if (!handled) {
              aText.ClearDrawingFontSize();
              RectangleF rect = textSizing.RawBoundingRectangle(aText, false, width);
              rect.Height = Math.Min(rect.Height, size.Height);
              r = rect.Size;
            }
            aText.ClearDrawingFontSize();
          }
        }
      }
      return r;
    }
    /// <summary>AttributedStrings have two fonts -- the base font (set by the core) and the drawing font (which usually equals
    /// the base font, but could be smaller if the string won't fit at the base font size. The useDrawingFont parameter tells us
    /// which font to use. =Typically, drawing code will want to use the drawing font, while measuring code will not.</summary>
    public static RectangleF BoundingRectangle(this TextSizing sizing, IAttributedString s, bool useDrawingFont, float maxWidth = float.NaN) {
      RectangleF rawRect;
      try {
        rawRect = sizing.RawBoundingRectangle(s, useDrawingFont, maxWidth);
      }
      catch (Exception e) {
        e.LogAndIgnore();
        rawRect = sizing.RawBoundingRectangle(s, useDrawingFont, maxWidth);
      }
      RectangleF r = rawRect.IntegralRectangle();
      return r;
    }
    public static float Ascent(this TextSizing sizing, CommonFont font) {
      float ascent = sizing.RawAscent(font);
      float r = (float)Math.Ceiling(ascent);
      return r;
    }
    public static float Descent(this TextSizing sizing, CommonFont font) {
      float descent = sizing.RawDescent(font);
      float r = (float)Math.Ceiling(descent);
      return r;
    }
    public static RectangleF BoundingRectangle(this TextSizing sizing, IAttributedString s,
      bool useDrawingFont, float x, float y, float maxWidth = float.NaN) {
      RectangleF rect = sizing.BoundingRectangle(s, useDrawingFont, maxWidth);
      float newX = x + rect.X;
      float newY = y + rect.Y;
      rect.X = newX;
      rect.Y = newY;
      return rect;
    }
    public static float BoundingHeight(this TextSizing sizing, IAttributedString s, bool useDrawingFont, float maxWidth = float.NaN) {
      SizeF size = sizing.BoundingSize(s, useDrawingFont, maxWidth);
      float r = size.Height;
      return r;
    }
    public static float BoundingWidth(this TextSizing sizing, IAttributedString s, bool useDrawingFont, float maxWidth = float.NaN) {
      SizeF size = sizing.BoundingSize(s, useDrawingFont, maxWidth);
      float r = size.Width;
      return r;
    }
    public static SizeF BoundingSize(this TextSizing sizing, IAttributedString s, bool useDrawingFont, float maxWidth = float.NaN) {
      RectangleF rect = sizing.RawBoundingRectangle(s, useDrawingFont, maxWidth);
      RectangleF intRect = rect.IntegralRectangle();
      SizeF r = intRect.Size;
      return r;
    }
    public static string Longest(this TextSizing sizing, params string[] strings) {
      string r = "";
      CommonFont font = new CommonFont(50);
      float maxWidth = 0;
      foreach (string s in strings) {

        AttributedString aString = new AttributedString(s, font, Color.Black);
        float width = sizing.BoundingWidth(aString, false);
        if (width > maxWidth) {
          maxWidth = width;
          r = s;
        }
      }
      return r;
    }
  }
}
