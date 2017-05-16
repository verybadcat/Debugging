using System;
using Jockusch.Common;

namespace Jockusch.Common
{
  public class BooleanAttribute: Attribute<bool>
  {
    public BooleanAttribute(bool payload) : base(payload)
    {

    }
    public override FuzzyBool PayloadBoolQ() {
      return FuzzyBoolAdditions.FromBool(this.Payload);
    }
    public override string Encoding {
      get {
        if (this.Payload) {
          return "1";
        } else {
          return "0";
        }
      }
    }
    public override GenericAttribute CloneAttribute() {
      return new BooleanAttribute(this.Payload);
    }
    public BooleanAttribute(string encodedPayload) : this(BoolAdditions.TryParse(encodedPayload))
    {
    }
  }
}

