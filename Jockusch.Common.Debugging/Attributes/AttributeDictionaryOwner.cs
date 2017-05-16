using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  /// <summary> We are going to have different classes that use the AttributeDictionary,
  /// with different possible collections of attributes.  Hence we have this class,
  /// with protected methods for getting/setting attributes in the dictionary.
  /// <remarks>Does not implement IAttributeCollection as the attributes supported by
  /// a subclass may be different -- the setters and getters in this class are not
  /// public, which is deliberate, as a subclass may not want to support all attributes.</remarks></summary>
  public abstract class AttributeDictionaryOwner
  {
    public int AttributeCount() {
      return this._Content.SafeCount();
    }
    protected AttributeDictionaryOwner(AttributeDictionaryOwner cloneMe) : this() {
      this.DeepCopyAttributesFrom(cloneMe);
    }
    public void Clear() {
      if (this._Content != null) {
        this._Content.Clear();
      }
    }
    /// <summary>An undefined attribute should always be treated the same as the attribute not existing.  Therefore,
    /// we should be able to call ClearUndefinedAttributes() at any time.<summary>
    public void ClearUndefinedAttributes() {
      List<string> killList = new List<string>(); // avoids mutation errors.
      foreach (KeyValuePair<string, GenericAttribute> pair in this.GetContent()) {
        GenericAttribute value = pair.Value;
        if (value.PayloadIsUndefined()) {
          string key = pair.Key;
          killList.Add(key);
        }
      }
      foreach (string key in killList) {
        this.GetContent()[key] = null;
      }
    }
    protected Dictionary<string, GenericAttribute> _Content;
    public Dictionary<string, GenericAttribute> GetContent() {
      if (_Content == null) {
        _Content = new Dictionary<string, GenericAttribute>();
      }
      return _Content;
    }
    public void ResetAttributes() {
      var content = this._Content;
      if (content != null) {
        content.Clear();
      }
    }
    public bool AttributesAreEmpty() {
      var content = this._Content;
      bool r = content.IsNullOrEmpty();
      return r;
    }
    /// <summary>Will never be null.  Returns the empty string if there is nothing to encode.</summary>
    public string Encoding(EncodingAttributeHandling handling) {
      this.ClearUndefinedAttributes();
      string r = "";
      Dictionary<string, GenericAttribute> content = this.GetContent();
      bool first = true;
      foreach (KeyValuePair<string, GenericAttribute> pair in content) {
        bool shouldEncode = handling.ShouldEncodeAttribute(pair);
        if (shouldEncode) {
          if (first) {
            first = false;
          } else {
            r += ",";
          }
          string key = pair.Key;
          GenericAttribute value = pair.Value;
          string encodedValue = value.Encoding;
          r += (key + "=" + encodedValue);
        }
      }
      return r;
    }
    protected AttributeDictionaryOwner(string encodedAttributes) : this() {
      char[] comma = ",".ToCharArray();
      string[] splitEncodedCollection = encodedAttributes.Split(comma, StringSplitOptions.RemoveEmptyEntries);
      foreach (string encodedAttribute in splitEncodedCollection) {
        int equalsIndex = encodedAttribute.IndexOf('=');
        if (equalsIndex != -1) {
          string key = encodedAttribute.Substring(0, equalsIndex);
          string value = encodedAttribute.Substring(equalsIndex + 1);
          GenericAttribute attribute = AttributeKeys.NewGenericAttribute(key, value);
          if (attribute != null) {
            this.GetContent()[key] = attribute;
          }
        }
      }
    }
    protected AttributeDictionaryOwner() {
    }

    #region get and set specific atributes.

    protected string HandleGetAccessibilityLabel() {
      string key = AttributeKeys.ACCESSIBILITY_LABEL_KEY;
      string r = this.HandleGetStringAttribute(key);
      return r;
    }

    protected void HandleSetAccessibilityLabel(string value) {
      string key = AttributeKeys.ACCESSIBILITY_LABEL_KEY;
      this.HandleSetStringAttribute(key, value);
    }

    public string GetTag() {
      return this.HandleGetTag();
    }
    public void SetTag(string value) {
      this.HandleSetTag(value);
    }

    protected void HandleSetUnderline(BitArray underline) {
      string key = AttributeKeys.UNDERLINE_KEY;
      this.CoalescingSetter(key, underline, u => u == null, u => new BitArrayAttribute(u));
    }

    protected BitArray HandleGetUnderline() {
      BitArray r = null;
      string key = AttributeKeys.UNDERLINE_KEY;
      Dictionary<string, GenericAttribute> attributes = this.GetContent();
      if (attributes.ContainsKey(key)) {
        GenericAttribute value = attributes[key];
        r = value.PayloadBitArray(null);
      }
      return r;
    }
    protected string HandleGetTag() {
      string key = AttributeKeys.TAG_KEY;
      string r = this.HandleGetStringAttribute(key);
      return r;
    }
    protected void HandleSetTag(string value) {
      string key = AttributeKeys.TAG_KEY;
      this.HandleSetStringAttribute(key, value);
    }
    protected void HandleSetErrorMessage(string message) {
      string key = AttributeKeys.ERROR_MESSAGE_KEY;
      this.HandleSetStringAttribute(key, message);
    }
    protected string HandleGetErrorMessage() {
      string key = AttributeKeys.ERROR_MESSAGE_KEY;
      string r = this.HandleGetStringAttribute(key);
      return r;
    }
    protected PointF HandleGetLocation() {
      PointF r = PointFAdditions.NaN;
      string key = AttributeKeys.LOCATION_KEY;
      Dictionary<string, GenericAttribute> attributes = this.GetContent();
      if (attributes.ContainsKey(key)) {
        GenericAttribute value = attributes[key];
        r = value.PayloadPointF(PointFAdditions.NaN);
      }
      return r;
    }
    private void CoalescingSetter<T, TAttribute>(string key, T value, Predicate<T> valueIsSpecial, Func<T, TAttribute> constructor)
      where TAttribute : Attribute<T> {
      if (valueIsSpecial(value) && this._Content != null) {
        this._Content.Remove(key);
      } else {
        Dictionary<string, GenericAttribute> attributes = this.GetContent();
        GenericAttribute existing = attributes.GetValueOrDefault(key);
        bool handled = false;
        if (existing != null) {
          if (existing is Attribute<T>) {
            (existing as Attribute<T>).Payload = value;
            handled = true;
          } else {
            CommonDebug.BreakPoint();
          }
        }
        if (!handled) {
          TAttribute attribute = constructor(value);
          attributes[key] = attribute;
        }
      }
    }
    protected void HandleSetLocation(PointF value) {
      this.CoalescingSetter(AttributeKeys.LOCATION_KEY, value, p => p.IsNaN(), p => new PointFAttribute(p));
    }
    protected string HandleGetDisplayString() {
      string key = AttributeKeys.DISPLAY_STRING_KEY;
      string r = this.HandleGetStringAttribute(key);
      return r;
    }
    protected void HandleSetDisplayString(string value) {
      string key = AttributeKeys.DISPLAY_STRING_KEY;
      this.HandleSetStringAttribute(key, value);
    }
    protected string HandleGetGhostedText() {
      string key = AttributeKeys.GHOSTED_TEXT_KEY;
      string r = this.HandleGetStringAttribute(key);
      return r;
    }
    protected void HandleSetGhostedText(string value) {
      string key = AttributeKeys.GHOSTED_TEXT_KEY;
      this.HandleSetStringAttribute(key, value);
    }
    protected float HandleGetMaxWidth() {
      string key = AttributeKeys.MAX_WIDTH_KEY;
      float r = this.HandleGetFloatAttribute(key);
      return r;
    }
    protected void HandleSetMaxWidth(float value) {
      string key = AttributeKeys.MAX_WIDTH_KEY;
      this.HandleSetFloatAttribute(key, value);
    }
    protected VerticalAlignmentEnum? HandleGetVerticalAlignmentQ() {
      VerticalAlignmentEnum? r = null;
      string key = AttributeKeys.VERTICAL_ALIGNMENT_KEY;
      Dictionary<string, GenericAttribute> attributes = this.GetContent();
      if (attributes.ContainsKey(key)) {
        GenericAttribute value = attributes[key];
        r = value.PayloadVerticalAlignment;
      }
      return r;
    }
    protected void HandleSetVerticalAlignmentQ(VerticalAlignmentEnum? value) {
      string key = AttributeKeys.VERTICAL_ALIGNMENT_KEY;
      Dictionary<string, GenericAttribute> attributes = this.GetContent();
      if (value == null) {
        attributes.RemoveIfPresent(key);
      } else {
        VerticalAlignmentAttribute attribute = new VerticalAlignmentAttribute(value.Value);
        attributes[key] = attribute;
      }
    }
    public VerticalAlignmentEnum GetSafeVerticalAlignment() {
      VerticalAlignmentEnum? r = this.HandleGetVerticalAlignmentQ();
      return r ?? VerticalAlignmentEnum.Center;
    }
    protected HorizontalAlignmentEnum? HandleGetHorizontalAlignmentQ() {
      HorizontalAlignmentEnum? r = null;
      string key = AttributeKeys.HORIZONTAL_ALIGNMENT_KEY;
      Dictionary<string, GenericAttribute> attributes = this.GetContent();
      if (attributes.ContainsKey(key)) {
        GenericAttribute value = attributes[key];
        r = value.PayloadHorizontalAlignment;
      }
      return r;
    }
    protected void HandleSetHorizontalAlignmentQ(HorizontalAlignmentEnum? value, bool preserveExisting = false) {
      string key = AttributeKeys.HORIZONTAL_ALIGNMENT_KEY;
      Dictionary<string, GenericAttribute> attributes = this.GetContent();
      if (!attributes.ContainsKey(key) || !preserveExisting) {
        if (value == null) {
          attributes.RemoveIfPresent(key);
        } else {
          HorizontalAlignmentAttribute attribute = new HorizontalAlignmentAttribute(value.Value);
          attributes[key] = attribute;
        }
      }
    }
    public HorizontalAlignmentEnum GetSafeHorizontalAlignment() {
      HorizontalAlignmentEnum? r = this.HandleGetHorizontalAlignmentQ();
      return r ?? HorizontalAlignmentEnum.Center;
    }

    protected int? HandleGetCursorLogicalIndexQ() {
      string key = AttributeKeys.CURSOR_LOGICAL_INDEX_KEY;
      int? r = this.HandleGetIntAttribute(key);
      return r;
    }
    protected void HandleSetCursorLogicalIndexQ(int? value) {
      string key = AttributeKeys.CURSOR_LOGICAL_INDEX_KEY;
      this.HandleSetIntAttribute(key, value);
    }
    protected int? HandleGetCursorMove() {
      string key = AttributeKeys.CURSOR_MOVE_KEY;
      int? r = this.HandleGetIntAttribute(key);
      return r;
    }
    protected void HandleSetCursorMove(int? value) {
      string key = AttributeKeys.CURSOR_MOVE_KEY;
      this.HandleSetIntAttribute(key, value);
    }
    protected FuzzyBool HandleGetIsStaticQ() {
      string key = AttributeKeys.ISSTATIC_KEY;
      FuzzyBool r = this.HandleGetBoolQAttribute(key);
      return r;
    }
    protected void HandleSetIsStaticQ(FuzzyBool value) {
      string key = AttributeKeys.ISSTATIC_KEY;
      this.HandleSetBoolAttribute(key, value);
    }
    public CommonFont HandleGetFont() {
      CommonFont r = null;
      string key = AttributeKeys.FONT_KEY;
      Dictionary<string, GenericAttribute> attributes = this.GetContent();
      if (attributes.ContainsKey(key)) {
        GenericAttribute attribute = attributes[key];
        r = attribute.PayloadFont();
      }
      return r;
    }
    public void HandleSetFont(CommonFont value) {
      string key = AttributeKeys.FONT_KEY;
      Dictionary<string, GenericAttribute> attributes = this.GetContent();
      if (value == null) {
        attributes.RemoveIfPresent(key);
      } else {
        FontAttribute attribute = new FontAttribute(value);
        attributes[key] = attribute;
      }
    }
    /// <summary> If nullOK is false, will breakpoint in the debug version and substitute a default font in the release version.</summary>
    public CommonFont GetFont(bool nullOK) {
      CommonFont r = this.HandleGetFont();
      if (!nullOK) {
        r = r ?? LastResorts.Font;
      }
      return r;
    }

    public void HandleSetTextColorQ(Color? value) {
      string key = AttributeKeys.TEXT_COLOR_KEY;
      this.HandleSetColorAttribute(key, value);
    }
    public Color? HandleGetTextColorQ() {
      string key = AttributeKeys.TEXT_COLOR_KEY;
      Color? r = this.HandleGetColorAttribute(key);
      return r;
    }
    public void HandleSetBackgroundColorQ(Color? value) {
      string key = AttributeKeys.BACKGROUND_COLOR_KEY;
      this.HandleSetColorAttribute(key, value);
    }
    public Color? HandleGetBackgroundColorQ() {
      string key = AttributeKeys.BACKGROUND_COLOR_KEY;
      Color? r = this.HandleGetColorAttribute(key);
      return r;
    }

    public abstract AttributeDictionaryOwner Clone();

    public void DeepCopyAttributesFrom(AttributeDictionaryOwner cloneMe) {
      Dictionary<string, GenericAttribute> cloneContent = cloneMe.GetContent();
      Dictionary<string, GenericAttribute> myContent = this.GetContent();
      myContent.Clear();
      foreach (string key in cloneContent.Keys) {
        GenericAttribute value = cloneContent[key];
        GenericAttribute cloneValue = value.CloneAttribute();
        myContent[key] = cloneValue;
      }
    }

    #endregion
  }
  #region additions
  public static class AttributeDictionaryOwnerAdditions
  {
    public static string HandleGetStringAttribute(this AttributeDictionaryOwner owner, string key) {
      string r = null;
      Dictionary<string, GenericAttribute> attributes = owner.GetContent();
      if (attributes.ContainsKey(key)) {
        GenericAttribute value = attributes[key];
        r = value.PayloadString();
      }
      return r;
    }

    public static void HandleSetStringAttribute(this AttributeDictionaryOwner owner, string key, string value) {
      Dictionary<string, GenericAttribute> attributes = owner.GetContent();
      if (value == null) {
        attributes.RemoveIfPresent(key);
      } else {
        StringAttribute attribute = new StringAttribute(value, false);
        attributes[key] = attribute;
      }
    }

    public static void HandleSetFloatAttribute(this AttributeDictionaryOwner owner, string key, float value) {
      Dictionary<string, GenericAttribute> attributes = owner.GetContent();
      if (float.IsNaN(value)) {
        attributes.RemoveIfPresent(key);
      } else {
        FloatAttribute attribute = new FloatAttribute(value);
        attributes[key] = attribute;
      }
    }

    public static float HandleGetFloatAttribute(this AttributeDictionaryOwner owner, string key, float defaultValue = float.NaN) {
      float r = defaultValue;
      Dictionary<string, GenericAttribute> attributes = owner.GetContent();
      if (attributes.ContainsKey(key)) {
        GenericAttribute value = attributes[key];
        r = value.PayloadFloat(defaultValue);
      }
      return r;
    }

    public static void HandleSetColorAttribute(this AttributeDictionaryOwner owner, string key, Color? value) {
      Dictionary<string, GenericAttribute> attributes = owner.GetContent();
      if (value == null) {
        attributes.RemoveIfPresent(key);
      } else {
        ColorAttribute attribute = new ColorAttribute(value.Value);
        attributes[key] = attribute;
      }
    }

    public static Color? HandleGetColorAttribute(this AttributeDictionaryOwner owner, string key) {
      Color? r = null;
      Dictionary<string, GenericAttribute> attributes = owner.GetContent();
      if (attributes.ContainsKey(key)) {
        GenericAttribute attribute = attributes[key];
        r = attribute.PayloadColor();
      }
      return r;
    }

    public static int? HandleGetIntAttribute(this AttributeDictionaryOwner owner, string key) {
      int? r = null;
      Dictionary<string, GenericAttribute> attributes = owner.GetContent();
      if (attributes.ContainsKey(key)) {
        GenericAttribute attribute = attributes[key];
        r = attribute.PayloadIntQ;
      }
      return r;
    }

    public static void HandleSetIntAttribute(this AttributeDictionaryOwner owner, string key, int? value) {
      Dictionary<string, GenericAttribute> attributes = owner.GetContent();
      if (value == null) {
        attributes.RemoveIfPresent(key);
      } else {
        IntAttribute attribute = new IntAttribute(value.Value);
        attributes[key] = attribute;
      }
    }

    public static bool HandleGetBoolAttribute(this AttributeDictionaryOwner owner, string key) {
      bool r = false;
      Dictionary<string, GenericAttribute> attributes = owner.GetContent();
      if (attributes.ContainsKey(key)) {
        GenericAttribute attribute = attributes[key];
        r = attribute.PayloadBool(false);
      }
      return r;
    }

    public static void HandleSetBoolAttribute(this AttributeDictionaryOwner owner, string key, bool value) {
      Dictionary<string, GenericAttribute> attributes = owner.GetContent();
      if (value == false) {
        attributes.RemoveIfPresent(key);
      } else {
        BooleanAttribute attribute = new BooleanAttribute(value);
        attributes[key] = attribute;
      }
    }

    public static void HandleSetBoolAttribute(this AttributeDictionaryOwner owner, string key, FuzzyBool value) {
      Dictionary<string, GenericAttribute> attributes = owner.GetContent();
      if (value.IsMaybe()) {
        attributes.RemoveIfPresent(key);
      } else {
        BooleanAttribute attribute = new BooleanAttribute(value.ToBool());
        attributes[key] = attribute;
      }
    }

    public static FuzzyBool HandleGetBoolQAttribute(this AttributeDictionaryOwner owner, string key) {
      FuzzyBool r = FuzzyBool.Maybe;
      Dictionary<string, GenericAttribute> attributes = owner.GetContent();
      if (attributes.ContainsKey(key)) {
        GenericAttribute attribute = attributes[key];
        r = attribute.PayloadBoolQ();
      }
      return r;
    }
  }
  #endregion
}
