using System;
using System.Drawing;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public class CommonFont 
  {
    public CommonFont() : this(-1) {
    }
    public CommonFont(float pointSize, bool bold = false, bool italic = false)
      : base() {
      _PointSize = pointSize;
      _Bold = bold;
      _Italic = italic;
    }
    protected bool _Bold { get; set; }
    protected bool _Italic { get; set; }
    protected float _PointSize { get; set; }
    public bool GetBold() {
      return _Bold;
    }
    public bool GetItalic() {
      return _Italic;
    }
    public float GetPointSize() {
      return _PointSize;
    }
    public override string ToString() {
      bool bold = this._Bold;
      bool italic = this._Italic;
      float size = this._PointSize;
      if (bold || italic) {
        string r = size.ToString();
        r = r.AppendIf(bold, " bold");
        r = r.AppendIf(italic, " italic");
        return r;
      } else {
        string name = ObjectNamer.NameForObject(this);
        return name + " " + size;
      }
    }
    public int CacheHash {
      get {
        int roundedPointSize = (int)(this._PointSize + 0.99999f);
        int r = roundedPointSize;
        if (this._Bold) {
          r += 1 << 16;
        }
        if (this._Italic) {
          r += 1 << 17;
        }
        return r;
      }
    }

    /// <summary>Determines whether or not the two fonts are equal, considered as fonts. Ignores mutablility.
    /// In other words, a mutable font could be equal to an immutable one if they are otherwise the same.</summary>
    public bool EqualsFont(CommonFont otherFont) {
      if (this._PointSize == otherFont._PointSize) {
        if (this._Italic == otherFont._Italic) {
          if (this._Bold == otherFont._Bold) {
            return true;
          }
        }
      }
      return false;
    }
    protected void DeepCopyIvarsFrom(CommonFont otherFont) {
      this._Bold = otherFont._Bold;
      this._PointSize = otherFont._PointSize;
      this._Italic = otherFont._Italic;
    }
    public CommonFont(CommonFont cloneMe) : base() {
      this.DeepCopyIvarsFrom(cloneMe);
    }


    public CommonFont Times(float factor, float minSize = float.MinValue) {
      CommonFont r = new CommonFont(this);
      float pointSize = this._PointSize * factor;
      r._PointSize = Math.Max(pointSize, minSize).RoundToFloat();
      return r;
    }
    public CommonFont FontWithSize(float size) {
      CommonFont r = null;
      {
        if (size == this.GetPointSize()) {
          r = this;
        } else {
          r = new CommonFont(size, this.GetBold(), this.GetItalic());
        }
      }
      return r;
    }
    public bool IsSane() {
      float size = this.GetPointSize();
      bool r = (size > 0 && size < 5000);
      return r;
    }
  }

  public static class CommonFontAdditions

  {
    /// <summary>If we are drawing lines near something with the font, this gives a sane line width.</summary>
    public static float GetProportionalLineStrokeWidth(this CommonFont font, float proportion = 1) {
      float pointSize = font.GetPointSize();
      float r = ((pointSize * 0.11f) + 0.5f).RoundToFloat();
      return r;
    }
    /// <summary>If passed-in height is less than or equal to zero, will not shrink.</summary>
    public static CommonFont ShrinkToFitHeight(this CommonFont font, float height) {
      TextSizing ts = TextSizing.Current;
      float fontHeight = ts.Ascent(font) + ts.Descent(font);
      CommonFont r = font;
      if (fontHeight > height && height > 0) {
        int adjustedSize = (int)(font.GetPointSize() * height / fontHeight);
        r = font.FontWithSize(adjustedSize);
#if DEBUG
        if (ts.Ascent(r) + ts.Descent(r) > height) {
          r = r.FontWithSize(adjustedSize - 1);
          CommonDebug.Assert(ts.Ascent(r) + ts.Descent(r) <= height);
        }
#endif

      }
      return r;
    }
    public static MutableCoreFont FontThatFitsHeight(this CommonFont font, float singleLineHeight, float expandToSize) {
      TextSizing ts = WJOperatingSystem.TS;
      float initialPointSize = font.GetPointSize();
      float expandedPointSize = FloatAdditions.Max(initialPointSize, expandToSize);
      MutableCoreFont r = new MutableCoreFont(font);
      float testSize = 1000;
      r.SetPointSize(testSize);
      AttributedString testString = AttributedStrings.CreateOrUpdate("Xq", r, Color.Black);
      float testHeight = ts.BoundingHeight(testString, false);
      float ratio = singleLineHeight / testHeight;
      float guess = (int)(testSize * ratio);

      if (guess < expandedPointSize) {
        r.SetPointSize(guess);
      } else {
        r.SetPointSize(expandedPointSize);
      }
      bool didIncrease = false;
      while ((r.GetPointSize() < expandedPointSize) &&
             (ts.BoundingHeight(testString, false) < singleLineHeight)) {
        {
          r.SetPointSize(r.GetPointSize() + 1);
          didIncrease = true;
        }
      }
      if (didIncrease) {
        // BoundingHeight call is expensive; didIncrease lets us avoid a call in this case.
        r.SetPointSize(r.GetPointSize() - 1);
      } else {
        while (ts.BoundingHeight(testString, false) > singleLineHeight) {
          r.SetPointSize(r.GetPointSize() - 1);
        }
      }
      return r;
    }
  }
}

