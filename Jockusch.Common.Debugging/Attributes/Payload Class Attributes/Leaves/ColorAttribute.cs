using System;
using System.Drawing;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public class ColorAttribute: Attribute<Color>
  {
    public ColorAttribute(Color payload) : base(payload)
    {
    }
    public override Color? PayloadColor(Color? defaultValue = null) {
      Color r = this.Payload;
      return r;
    }
    public override string Encoding {
      get {
        throw new BreakingException(); // We should never need the encoding
      }
    }
    public override GenericAttribute CloneAttribute() {
      return new ColorAttribute(this.Payload);
    }
  }
}

