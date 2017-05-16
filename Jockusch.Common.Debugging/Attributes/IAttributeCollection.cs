using System;
using System.Drawing;
using System.Collections;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  ///<summary> Not every class with an AttributeDictionary will implement IAttributeCollection,
  /// as the supported attributes may differ.  However, every class which owns an AttributeCollection
  /// object should implement it.</summary>
  public interface IAttributeCollection: IAttributeDictionaryOwner
  {
    AttributeCollection GetAttributes(bool createIfNull);
    void SetAttributes(AttributeCollection attributes);
    PointF Location { get; set; }
    string AttributeDisplayString { get; set; }
    float MaxWidth { get; set; }
    VerticalAlignmentEnum? VerticalAlignmentQ{ get; set; }
    HorizontalAlignmentEnum? HorizontalAlignmentQ{ get; set; }
    BitArray Underline { get; set; }
    string AccessibilityLabel { get; set; }
  }

  public static class IAttributeCollectionAdditions
  {
    public static bool HasAttributes(this IAttributeCollection collection) {
      AttributeCollection attributes = collection.GetAttributes(false);
      bool r = (attributes != null);
      return r;
    }
    public static void SetAttributes(this IAttributeCollection collection, string text) {
      AttributeCollection attributes = AttributeCollections.FromString(text);
      collection.SetAttributes(attributes);
    }
    public static void ClearAttributes(this IAttributeCollection owner) {
      AttributeCollection collection = owner.GetAttributes(false);
      collection?.Clear();
    }
    /// <summary>
    ///</summary>
    public static void CloneAttributes(
      this IAttributeCollection collection, IAttributeCollection otherCollection) {
      AttributeCollection otherAttributes = otherCollection.GetAttributes(false);
      if (otherAttributes == null || otherAttributes.IsEmpty()) {
        collection.ClearAttributes();
      } else {
        AttributeCollection attributes = collection.GetAttributes(true);
        attributes.DeepCopyIvarsFrom(otherAttributes, true);
      }
    }
    /// <summary>Typically used when drawing.  If you want the stored value, use HorizontalAlignmentQ.</summary>
    public static HorizontalAlignmentEnum GetHorizontalAlignment(this IAttributeCollection owner) {
      HorizontalAlignmentEnum? rQ = owner.HorizontalAlignmentQ;
      HorizontalAlignmentEnum r = rQ ?? HorizontalAlignmentEnum.Center;
      return r;
    }
    /// <summary>Typically used when drawing.  If you want the stored value, use VerticalAlignmentQ.</summary>
    public static VerticalAlignmentEnum GetVerticalAlignment(this IAttributeCollection owner) {
      VerticalAlignmentEnum? rQ = owner.VerticalAlignmentQ;
      VerticalAlignmentEnum r = rQ ?? VerticalAlignmentEnum.Center;
      return r;
    }
    public static string GetEncodingIncludingAttributes(this IAttributeCollection owner, 
      string encodingWithoutAttributes,
      EncodingAttributeHandling handling
    ) {
      string r = encodingWithoutAttributes;
      CommonDebug.NullCheck(r);
      AttributeCollection attributes = owner?.GetAttributes(false);
      if (attributes != null) {
          string encodeAttributes = attributes.Encoding(handling);
          if (!string.IsNullOrEmpty(encodeAttributes)) {
            r = StringConstants.LeftBrace + encodingWithoutAttributes
            + StringConstants.AtString + encodeAttributes + StringConstants.RightBrace;
          }
      }
      return r;
    }
    /// <summary> If the object already has a alignment, the horizontal alignment will not be set.  Same for vertical.</summary>
    public static void SetAlignmentIfAbsent(this IAttributeCollection collection, HorizontalAlignmentEnum horizontalAlignment, VerticalAlignmentEnum verticalAlignment) {
      HorizontalAlignmentEnum? existingH = collection.HorizontalAlignmentQ;
      VerticalAlignmentEnum? existingV = collection.VerticalAlignmentQ;
      if (existingH == null) {
        collection.HorizontalAlignmentQ = horizontalAlignment;
      }
      if (existingV == null) {
        collection.VerticalAlignmentQ = verticalAlignment;
      }
    }
  }
}

