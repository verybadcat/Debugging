using System;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public class VerticalAlignmentAttribute: Attribute<VerticalAlignmentEnum?> {
    public VerticalAlignmentAttribute (VerticalAlignmentEnum? alignment): base(alignment)
    {
    }
    public override bool PayloadsAreEqual(VerticalAlignmentEnum? payload1, VerticalAlignmentEnum? payload2) {
      return (payload1 == payload2);
    }
    public override VerticalAlignmentEnum? PayloadVerticalAlignment {
      get {
        return this.Payload;
      }
    }
    public override GenericAttribute CloneAttribute() {
      return new VerticalAlignmentAttribute(this.Payload);
    }
    public VerticalAlignmentAttribute(string encodedAlignment):
    this(VerticalAlignmentAdditions.FromString(encodedAlignment)) {}
    public override string Encoding {
      get {
        CommonDebug.BreakPoint();
        return this.Payload.ToString();
      }
    }
  }
}

