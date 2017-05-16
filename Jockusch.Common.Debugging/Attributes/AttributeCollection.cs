using System;
using System.Collections.Generic;
using System.Drawing;
using Jockusch.Common;
using Jockusch.Common.Debugging;
using System.Collections;
using System.Linq;

namespace Jockusch.Common
{
  /// <summary>For tracking a collection of attributes of an object.</summary>
  public class AttributeCollection: AttributeDictionaryOwner, IAttributeCollection
  {
    public override string ToString() {
      int count = this.AttributeCount();
      return "AttributeCollection count=" + count;
    }
    public bool IsEmpty() {
      bool r = this._Content.IsNullOrEmpty();
      return r;
    }

    public AttributeCollection GetAttributes(bool createIfNull) {
      return this;
    }
    public void SetAttributes(AttributeCollection attributes) {
      this.DeepCopyIvarsFrom(attributes, true);
    }
    public bool AnythingToEncode(EncodingAttributeHandling handling) {
      foreach (KeyValuePair<string, GenericAttribute> pair in this.GetContent()) {
        if (handling.ShouldEncodeAttribute(pair)) {
          return true;
        }
      }
      return false;
    }

    public bool EqualsCollection(AttributeCollection otherCollection) {
      bool r = false;
      Dictionary<string, GenericAttribute> content = this._Content;
      Dictionary<string, GenericAttribute> otherContent = otherCollection._Content;
      if (content == null && otherContent == null) {
        r = true;
      } else if (content!=null && otherContent!=null) {
        if (content.Count == otherContent.Count) {
          r = true;
          foreach (KeyValuePair<string, GenericAttribute> pair in content) {
            string key = pair.Key;
            GenericAttribute value = pair.Value;
            if (otherContent.ContainsKey(key)) {
              GenericAttribute otherValue = otherContent[key];
              if ((value == null && otherValue == null) || otherValue.EqualsAttribute(value)) {
                continue;
              }
            }
            r = false;
            break;
          }
        }
      }
      return r;
    }
    #region specific attributes

    public string AccessibilityLabel {
      get {
        string r = this.HandleGetAccessibilityLabel();
        return r;
      }
      set {
        this.HandleSetAccessibilityLabel(value);
      }
    }



    public PointF Location {
      get {
        PointF r = this.HandleGetLocation();
        return r;
      }
      set {
        this.HandleSetLocation(value);
      }
    }
    public string AttributeDisplayString {
      get {
        string r = this.HandleGetDisplayString();
        return r;
      }
      set {
        this.HandleSetDisplayString(value);
      }
    }
    public float MaxWidth {
      get {
        float r = this.HandleGetMaxWidth();
        return r;
      }
      set {
        this.HandleSetMaxWidth(value);
      }
    }
    public BitArray Underline {
      get {
        BitArray r = this.HandleGetUnderline();
        return r;
      }
      set {
        this.HandleSetUnderline(value);
      }
    }
    /// <summary>
    /// See also VerticalAlignment, which is not nullable and returns the default value (center) if this property is null.
    /// When drawing, one would typically use VerticalAlignment, not VerticalAlignmentQ.
    /// </summary>
    public VerticalAlignmentEnum? VerticalAlignmentQ {
      get {
        VerticalAlignmentEnum? r = this.HandleGetVerticalAlignmentQ();
        return r;
      }
      set {
        this.HandleSetVerticalAlignmentQ(value);
      }
    }
    /// <summary>Typically used when drawing.  If you want the stored value, use VerticalAlignmentQ.</summary>
    public VerticalAlignmentEnum VerticalAlignment {
      get {
        VerticalAlignmentEnum? rQ = this.VerticalAlignmentQ;
        VerticalAlignmentEnum r = rQ??VerticalAlignmentEnum.Center;
        return r;
      }
    }

    /// <summary>
    /// See also HorizontalAlignment, which is not nullable and returns the default value (center) if this property is null.
    /// When drawing, one would typically use HorizontalAlignment, not HorizontalAlignmentQ.
    /// </summary>
    public HorizontalAlignmentEnum? HorizontalAlignmentQ {
      get {
        HorizontalAlignmentEnum? r = this.HandleGetHorizontalAlignmentQ();
        return r;
      }
      set {
        this.HandleSetHorizontalAlignmentQ(value);
      }
    }
    /// <summary>Typically used when drawing.  If you want the stored value, use HorizontalAlignmentQ.</summary>
    public HorizontalAlignmentEnum HorizontalAlignment {
      get {
        HorizontalAlignmentEnum? rQ = this.HorizontalAlignmentQ;
        HorizontalAlignmentEnum r = rQ??HorizontalAlignmentEnum.Left;
        return r;
      }
    }

    /// <summary> If the string already has a horizontal alignment, the horizontal alignment will not be notify.  Same for vertical.</summary>
    public void SetAlignmentIfAbsent(HorizontalAlignmentEnum horizontalAlignment, VerticalAlignmentEnum verticalAlignment) {
      HorizontalAlignmentEnum? existingH = this.HorizontalAlignmentQ;
      VerticalAlignmentEnum? existingV = this.VerticalAlignmentQ;
      if (existingH==null) {
        this.HorizontalAlignmentQ = horizontalAlignment;
      }
      if (existingV == null) {
        this.VerticalAlignmentQ = verticalAlignment;
      }
    }


    public int? CursorLogicalIndexQ {
      get {
        int? r = this.HandleGetCursorLogicalIndexQ();
        return r;
      }
      set {
        this.HandleSetCursorLogicalIndexQ(value);
      }
    }

    public int? CursorMove {
      get {
        int? r = this.HandleGetCursorMove();
        return r;
      }
      set {
        this.HandleSetCursorMove(value);
      }
    }

    public FuzzyBool IsStaticQ {
      get {
        FuzzyBool r = this.HandleGetIsStaticQ();
        return r;
      }
      set {
        this.HandleSetIsStaticQ(value);
      }
    }
    public bool IsStatic {
      get {
        FuzzyBool rQ = this.IsStaticQ;
        return rQ.FirstKnown(false);
      }
    }
    public string GhostedText {
      get {
        string r = this.HandleGetGhostedText();
        return r;
      }
      set {
        this.HandleSetGhostedText(value);
      }
    }
    #endregion
    public void DeepCopyAttributesFrom(AttributeCollection cloneMe) {
      this.DeepCopyIvarsFrom(cloneMe, true);
    }
    /// <summary>If this has a property for something, a soft copy will not change it.</summary>
    public void DeepCopyIvarsFrom(AttributeCollection cloneMe, bool hard) {
      if (cloneMe != null && cloneMe!=this) {
        Dictionary<string, GenericAttribute> myContent = this.GetContent();
        Dictionary<string, GenericAttribute> cloneContent = cloneMe.GetContent();
        foreach (string key in cloneContent.Keys) {
          if (hard || myContent[key] == null) {
            GenericAttribute cloneAttribute = cloneContent[key];
            GenericAttribute myAttribute = myContent.GetValueOrDefault(key, ErrorHandling.None);
            GenericAttribute recycled = cloneAttribute.CloneOrUpdateGeneric(myAttribute);
            myContent[key] = recycled;
          } 
        }
        if (hard) {
          List<string> myKeys = myContent.Keys.Where(key => !(cloneContent.ContainsKey(key))).ToList();
          foreach (string myKey in myKeys) {
            myContent.Remove(myKey);
          }
        }
      }
    }
    public AttributeCollection() {}
    public AttributeCollection(AttributeCollection otherCollection)
      : this() {
      this.DeepCopyAttributesFrom(otherCollection);
    }
    public AttributeCollection(string encodedCollection): base(encodedCollection) {
    }
    public AttributeCollection CloneAttributeCollection() {
      AttributeCollection r = new AttributeCollection (this);
      return r;
    }
    public override AttributeDictionaryOwner Clone() {
      return this.CloneAttributeCollection();
    }
    public static AttributeCollection CloneOrNull(AttributeCollection otherCollection) {
      AttributeCollection r = null;
      if (otherCollection != null) {
        r = otherCollection.CloneAttributeCollection();
      }
      return r;
    }
    #region soft setters
    /// <summary>A soft set does nothing if there is already a value for the attribute.</summary>
    public void SetHorizontalAlignmentQ(HorizontalAlignmentEnum? value, bool hard = true) {
      if (hard || AnyType.EqualsDefault(this.HorizontalAlignmentQ)) {
        this.HorizontalAlignmentQ = value;
      }
    }
    public void SetVerticalAlignmentQ(VerticalAlignmentEnum? value, bool hard = true) {
      if (hard || AnyType.EqualsDefault(this.VerticalAlignmentQ)) {
        this.VerticalAlignmentQ = value;
      }
    }
    public void SetGhostedText(string text, bool hard = true) {
      if (hard || AnyType.EqualsDefault(this.GhostedText)) {
        this.GhostedText = text;
      }
    }
    public void SetAttributeDisplayString(string value, bool hard = true) {
      if (hard || AnyType.EqualsDefault(this.AttributeDisplayString)) {
        this.AttributeDisplayString = value;
      }
    }
    public void SetMaxWidth(float value, bool hard = true) {
      if (hard || float.IsNaN(this.MaxWidth)) {
        this.MaxWidth = value;
      }
    }
    public void SetIsStaticQ(FuzzyBool value, bool hard = true) {
      if (hard || AnyType.EqualsDefault(this.IsStaticQ)) {
        this.IsStaticQ = value;
      }
    }
    public void SetUnderline(BitArray value, bool hard = true) {
      if (hard || AnyType.EqualsDefault(this.Underline)) {
        this.Underline = value;
      }
    }
    public void SetAccessiblityLabel(string value, bool hard = true) {
      if (hard || AnyType.EqualsDefault(this.AccessibilityLabel)) {
        this.AccessibilityLabel = value;
      }
    }
    public void SetLocation(PointF value, bool hard = true) {
      if (hard || AnyType.EqualsDefault(this.Location)) {
        this.Location = value;
      }
    }
    public void SetCursorLogicalIndexQ(int? value, bool hard = true) {
      if (hard || AnyType.EqualsDefault(this.CursorLogicalIndexQ)) {
        this.CursorLogicalIndexQ = value;
      }
    }
    #endregion
#if DEBUG
    public Dictionary<string, GenericAttribute> ContentDebugConvenience {
      get {
        return this._Content;
      }
    }
#endif
  }
}

