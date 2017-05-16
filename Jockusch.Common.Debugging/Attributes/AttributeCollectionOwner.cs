using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Jockusch.Common;
using Jockusch.Common.Debugging;
using System.Collections;

namespace Jockusch.Common {
  public class AttributeCollectionOwner:  IAttributeCollection {
    public virtual BitArray Underline {
      get {
        BitArray r = this._Attributes?.Underline;
        return r;
      } set {
        this.NullCoalescingSetter(a => a.Underline = value, value);
      }
    }
    public virtual void ResetAttributes() {
      AttributeCollection attributes = this.GetAttributes(false);
      attributes?.ResetAttributes();
    }
    public void DeepCopyAttributesFrom(IAttributeCollection cloneMe, bool really = true) {
      if (really && cloneMe!=this) {
        AttributeCollection attributes = cloneMe?.GetAttributes(false);
        if (attributes != null) {
          this.Attributes.DeepCopyAttributesFrom(attributes);
        }
      }
    }
    public AttributeCollectionOwner() {}
    public AttributeCollectionOwner(AttributeCollectionOwner otherOwner)
      : this() {
        this.DeepCopyAttributesFrom(otherOwner);
    }

    public void SetAttributes(AttributeCollection attributes) {
      if (attributes == null) {
        this.Attributes = null;
      } else {
        this.Attributes.SetAttributes(attributes);
      }
    }


    #region IAttributeCollection

    public string AccessibilityLabel {
      get {
        return this.GetAttributes(false)?.AccessibilityLabel;
      }
      set {
        this.NullCoalescingSetter(a => a.AccessibilityLabel = value, value);
      }
    }


    protected AttributeCollection _Attributes {get;set;}
    public AttributeCollection Attributes {
      get {
        if (_Attributes == null) {
          _Attributes = new AttributeCollection ();
        }
        return _Attributes;
      }
      set {
        _Attributes = value;
      }
    }
    public AttributeCollection GetAttributes(bool createIfNull) {
      if (createIfNull) {
        return this.Attributes;
      } else {
        return this._Attributes;
      }
    }
    public virtual PointF Location {
      get {
        PointF r = PointFAdditions.NaN; 
        if (this._Attributes!=null) {
          r = this._Attributes.Location;
        }
        return r;
      }
      set {
        this.CoalescingSetter(a => a.Location = value, value, p => p.IsNaN());
      }
    }
    /// <summary>
    /// See also VerticalAlignment, which is not nullable and returns the default value (center) if this property is null.
    /// When drawing, one would typically use VerticalAlignment, not VerticalAlignmentQ.
    /// </summary>
    public virtual VerticalAlignmentEnum? VerticalAlignmentQ {
      get {
        return this.GetAttributes(false)?.VerticalAlignmentQ;
      }
      set {
        this.NullCoalescingSetter(a => a.VerticalAlignmentQ = value, value);
      }
    }

    /// <summary>
    /// See also GetHorizontalAlignment(), which is not nullable and returns the default value (center) if this property is null.
    /// When drawing, one would typically use HorizontalAlignment, not HorizontalAlignmentQ.
    /// </summary>
    public virtual HorizontalAlignmentEnum? HorizontalAlignmentQ {
      get {
        return this.GetAttributes(false)?.HorizontalAlignmentQ;
      }
      set {
        this.NullCoalescingSetter(a => a.HorizontalAlignmentQ = value, value);
      }
    }


    public virtual string GhostedText {
      get {
        string r = this.GetAttributes(false)?.GhostedText;
        return r;
      }
      set {
        this.NullCoalescingSetter(a => a.GhostedText = value, value);
      }
    }
    private void NullCoalescingSetter<T>(Action<AttributeCollection> action, T value) {
      this.CoalescingSetter(action, value, v => AnyType.EqualsDefault(v));
    }

    private void CoalescingSetter<T>(Action<AttributeCollection> action, T value, Predicate<T> valueIsSpecial) {
      bool createAttributes = !valueIsSpecial(value);
      AttributeCollection attributes = this.GetAttributes(createAttributes);
      if (attributes!=null) {
        action(attributes);
      }
    }
    public string AttributeDisplayString {
      get {
        return this.GetAttributes(false)?.AttributeDisplayString;
      }
      set {
        this.NullCoalescingSetter(a => a.AttributeDisplayString = value, value);
      }
    }
    public float MaxWidth {
      get {
        return this.GetAttributes(false)?.MaxWidth ?? float.NaN;
      }
      set {
        this.CoalescingSetter(a => a.MaxWidth = value, value, v => float.IsNaN(v));
      }
    }
    public void SetHorizontalAlignmentQ(HorizontalAlignmentEnum? value, bool hard = true) {
      this.NullCoalescingSetter(a => a.SetHorizontalAlignmentQ(value, hard), value);
    }
    public void SetVerticalAlignmentQ(VerticalAlignmentEnum? value, bool hard = true) {
      this.NullCoalescingSetter(a => a.SetVerticalAlignmentQ(value, hard), value);
    }
    #endregion
  }
}
