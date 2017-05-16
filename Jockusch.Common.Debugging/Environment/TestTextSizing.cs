using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Jockusch.Common;
using Jockusch.Common.Debugging;

namespace JockuschTests.Graphics
{
  class TestTextSizing : TextSizing
  {
    public float AscentCoefficient { get; set; }
    public float DescentCoefficient { get; set; }
    public float WidthCoefficient { get; set; }
    public float LeadingCoefficient { get; set; }
    public TestTextSizing() : this(0.6f, 0.15f, 0.6f, 0.1f) {
    }
    public TestTextSizing(float ascentCoef, float descentCoef, float widthCoef, float leadingCoef) : base(false) {
      this.AscentCoefficient = ascentCoef;
      this.DescentCoefficient = descentCoef;
      this.WidthCoefficient = widthCoef;
      this.LeadingCoefficient = leadingCoef;
    }
    public override float RawAscent(CommonFont font) {
      float size = font.GetPointSize();
      float coef = this.AscentCoefficient;
      float r = size * coef;
      return r;
    }
    public override float RawDescent(CommonFont font) {
      float size = font.GetPointSize();
      float coef = this.DescentCoefficient;
      float r = size * coef;
      return r;
    }
    public float WidthPerChar(IAttributedString s) {
      CommonFont font = s.GetFont(true);
      float pointSize = font.GetPointSize();
      float coefficient = this.WidthCoefficient;
      float r = pointSize * coefficient;
      return r;
    }
    float Width(IAttributedString s) {
      bool empty = s.IsEmpty();
      if (empty) {
        return 0;
      }
      float perChar = this.WidthPerChar(s);
      int length = s.Length();
      float r = length * perChar;
      return r;
    }
    public float Leading(CommonFont font) {
      float size = font.GetPointSize();
      float coef = this.LeadingCoefficient;
      float r = size * coef;
      return r;
    }
    public override RectangleF RawBoundingRectangle(IAttributedString s, bool useDrawingFont, float maxWidth = float.NaN) {
      RectangleF r;
      if (s.IsEmpty()) {
        r = new RectangleF();
      } else {
        float width = this.Width(s);
        CommonFont font = s.GetFont(useDrawingFont);
        float ascent = this.Ascent(font);
        float descent = this.Descent(font);
        if (float.IsNaN(maxWidth)) {
          r = new RectangleF(0, -ascent, width, ascent + descent);
        } else if (maxWidth <= 0) {
          throw new BreakingException();
        } else {
          int lines;
          if (maxWidth >= int.MaxValue) {
            lines = 1;
          } else {
            float widthPerChar = this.WidthPerChar(s);
            float spaceForChars = maxWidth / widthPerChar;
            int charsPerLine = (int)spaceForChars + 1; //+1 because of roundoff error; we subtract shortly.
            float product = charsPerLine * widthPerChar;
            while (charsPerLine > 1 && product > maxWidth) {
              charsPerLine -= 1;
              product = charsPerLine * widthPerChar;
            }
            int ratio = s.Length() / charsPerLine;
            int modulus = s.Length() % charsPerLine;
            lines = (modulus == 0) ? ratio : ratio + 1;
          }
          if (lines == 1) {
            r = new RectangleF(0, -ascent, width, ascent + descent);
          } else {
            float leading = this.Leading(font);
            float y = -ascent;
            float height = lines * (ascent + descent) + (lines - 1) * leading;
            r = new RectangleF(0, y, maxWidth, height);
          }
        }
      }
      if (maxWidth > 0 && r.Width > maxWidth && s.Text?.Length > 1) {
        CommonDebug.BreakPoint();
      }
      return r;
    }
  }
}
