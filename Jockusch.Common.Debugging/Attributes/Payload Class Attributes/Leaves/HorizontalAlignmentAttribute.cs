using System;

namespace Jockusch.Common
{
  public class HorizontalAlignmentAttribute: Attribute<HorizontalAlignmentEnum?> {
    public HorizontalAlignmentAttribute (HorizontalAlignmentEnum? alignment): base(alignment)
    {
      this.Payload = alignment;
    }
    public override bool PayloadsAreEqual(HorizontalAlignmentEnum? payload1, HorizontalAlignmentEnum? payload2) {
      return (payload1 == payload2);
    }
    public override HorizontalAlignmentEnum? PayloadHorizontalAlignment {
      get {
        return this.Payload;
      }
    }
    public override string Encoding {
      get {
        HorizontalAlignmentEnum? payload = this.Payload;
        string r;
        if (payload == null) {
          r = "";
        } else {
          r = payload.Value.ToString();
        }
        return r;
      }
    }
    public HorizontalAlignmentAttribute (string encodedAlignment): this(HorizontalAlignmentAdditions.FromString(encodedAlignment)) {
    }
    public override bool PayloadIsUndefined() {
      return this.Payload == null;
    }
    public override GenericAttribute CloneAttribute() {
      return new HorizontalAlignmentAttribute(this.Payload);
    }
  }
}
