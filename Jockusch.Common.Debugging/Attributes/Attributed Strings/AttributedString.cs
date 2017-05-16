using System.Drawing;
using System.Collections.Generic;
using System;
using Jockusch.Common.Debugging;
using Jockusch.Common;
using System.Collections;


namespace Jockusch.Common
{
  public partial class AttributedString : AttributeCollectionOwner, IAttributedString
  {
    /* The superclass allows one to define additional attributes, such as alignment.  
     * If you don't need those, you can ignore it. */
    private string _Text { get; set; }
    public string Text {
      get {
        return _Text;
      }
      set {
        if (value != _Text) {
          _Text = value;
          this.ClearDrawingFontSize();
        }
      }
    }
    private Func<CommonFont> _FontGetter { get; set; }
    public Func<CommonFont> FontGetter {
      get {
        return _FontGetter;
      }
      set {
        _FontGetter = value;
        this.ClearDrawingFontSize();
      }
    }
    public override void ResetAttributes() {
      base.ResetAttributes();
      this._DrawingFontSize = float.NaN;
    }
    private float _DrawingFontSize { get; set; } = float.NaN;
    public void SetDrawingFontSize(float size) {
      if (size < 0) {
        CommonDebug.BreakPoint();
      }
      if (size == 0 && !string.IsNullOrWhiteSpace(this.Text)) {
        CommonDebug.BreakPoint();
      }
      if (size.IsFiniteFloat() && size != (int)size) {
        CommonDebug.BreakPoint();
      }
      if (size == float.MinValue) {
        CommonDebug.BreakPoint();
      }
      this._DrawingFontSize = size;
    }
    public bool HasDrawingFontSize(bool useBaseFontSizeIfUndefined) {
      return !float.IsNaN(this.GetDrawingFontSize(useBaseFontSizeIfUndefined));
    }
    /// <summary>Before any size shrinkage for drawing.</summary>
    public CommonFont BaseFont {
      get {
        CommonFont r = FontGetter();
        return r;
      }
      set {
        FontGetter = () => value;
        this._DrawingFontSize = float.NaN;
      }
    }
    public CommonFont GetDrawingFont() {
      CommonFont r = this.BaseFont;
      if (!float.IsNaN(this._DrawingFontSize)) {
        float size = this._DrawingFontSize;
        r = r.FontWithSize(size);
      }
      return r;
    }
    public CommonFont GetFont(bool drawingFont) {
      CommonFont r;
      if (drawingFont) {
        r = this.GetDrawingFont();
      } else {
        r = this.BaseFont;
      }
      return r;
    }
    public Color Color { get; set; }
    public Color GetTextColor() {
      return this.Color;
    }
    public void SetText(string text, bool replaceUnderscoresWithUnderlines) {
      BitArray underlines = null;
      int index = 0;
      if (replaceUnderscoresWithUnderlines && !(string.IsNullOrEmpty(text))) {
        index = text.IndexOfInvariant(StringConstants.Underscore, 1);
        while (index > 0) {
          // If the character is at index zero, we have to leave can't remove it and
          // underline the previous character, so instead we leave it in place.
          char[] array = text.ToCharArray();
          char previous = array[index - 1];
          if (previous.IsNumeric()) {
            if (underlines == null) {
              underlines = new BitArray(0);
            }
            underlines.GrowAndSet(index - 1, true);
            text = text.Remove(index, 1);
          }
          if (index + 1 >= text.Length) {
            break;
          } else {
            index = text.IndexOfInvariant(StringConstants.Underscore, index + 1);
          }
        }
      }
      this.Text = text;
      this.Underline = underlines;
    }
    /// <summary>If the color is null, does nothing; otherwise sets it as our color.</summary>
    public void SetColorQ(Color? color) {
      if (color.HasValue) {
        this.Color = color.Value;
      }
    }
    /// Returns true iff all three are equal. 
    public bool EqualsTextFontColor(AttributedString otherAttributedString) {
      if (this.Text == otherAttributedString.Text) {
        if (this.BaseFont.EqualsFont(otherAttributedString.BaseFont)) {
          if (this.Color == otherAttributedString.Color) {
            return true;
          }
        }
      }
      return false;
    }
    protected AttributedString()
    {
    }
    public AttributedString(IAttributedString otherAttributedString) : this()
    {
      this.DeepCopyIvarsFrom(otherAttributedString);
    }

    public string Tag {
      get {
        return this.Attributes.GetTag();
      }
      set {
        this.Attributes.SetTag(value);
      }
    }


    public void DeepCopyIvarsFrom(IAttributedString cloneMe) {
      base.DeepCopyAttributesFrom(cloneMe);
      this.Text = cloneMe.Text;
      this.Color = cloneMe.GetTextColor();
      Func<CommonFont> getter = cloneMe.FontGetter;
      this.FontGetter = getter;
      this.SetDrawingFontSize(cloneMe.GetDrawingFontSize(false));
    }
    /// <summary>Typical use case is when the style information is coming from a style sheet</summary>
    public static AttributedString TextOnly(string text) {
      return new AttributedString(text, (CommonFont)null, Color.Transparent);
    }
    public AttributedString(string text, Func<CommonFont> fontGetter, Color color, AttributeCollection additionalAttributes = null) : base()
    {
      Text = text;
      FontGetter = fontGetter;
      Color = color;
      _Attributes = additionalAttributes;
    }
    public AttributedString(string text, CommonFont font, Color color, AttributeCollection additionalAttributes = null) :
      this(text, () => font, color, additionalAttributes)
    {
    }
    public AttributedString(string text, CommonFont font, Color color, HorizontalAlignmentEnum? horizontalAlignment, VerticalAlignmentEnum? verticalAlignment) : this(text, font, color)
    {
      if (horizontalAlignment != null) {
        this.HorizontalAlignmentQ = horizontalAlignment;
      }
      if (verticalAlignment != null) {
        this.VerticalAlignmentQ = verticalAlignment;
      }
    }
    public AttributedString(string text, Func<CommonFont> fontGetter, Color color, HorizontalAlignmentEnum? horizontalAlignment,
                            VerticalAlignmentEnum? verticalAlignment = null) : this(text, fontGetter, color)
    {
      if (horizontalAlignment != null) {
        this.HorizontalAlignmentQ = horizontalAlignment;
      }
      if (verticalAlignment != null) {
        this.VerticalAlignmentQ = verticalAlignment;
      }
    }

    private static AttributedString _SharedInvisibleAttributedString;
    public static AttributedString SharedInvisibleAttributedString {
      get {
        if (_SharedInvisibleAttributedString == null) {
          _SharedInvisibleAttributedString = AttributedStrings.CreateOrUpdate("", new CommonFont(0), Color.Transparent);
        }
        return _SharedInvisibleAttributedString;
      }
    }
    public int Length {
      get {
        string s = this.Text;
        if (s != null) {
          int r = s.Length;
          return r;
        }
        return 0;
      }
    }
    public override string ToString() {
      string text = this.Text;
      string r = (text == null) ? "AttrbitedString text=null" : text.ToString();
      CommonFont font = this.BaseFont;
      float size = font.GetPointSize();
      if (size < 8) {
        r += (" fontSize " + size.ToString());
      }
      return r;
    }
    public string GetAccessibilityLabel(bool tryHard) {
      string r1 = this.Attributes.AccessibilityLabel;
      string r2 = this.Text;
      string r3 = this.GhostedText;
      string r;
      if (tryHard) {
        r = StringAdditions.FirstNonempty(r1, r2, r3);
      } else {
        r = r1;
      }
      return r;
    }
    public void SetTextColor(Color color) {
      this.Color = color;
    }
  }
}