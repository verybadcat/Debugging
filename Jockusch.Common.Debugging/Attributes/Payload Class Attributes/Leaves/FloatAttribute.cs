using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using numericType = System.Single;

namespace Jockusch.Common
{
  class FloatAttribute: Attribute<numericType>
  {
    public FloatAttribute(numericType value)
      : base(value)
    {

    }
    public override bool PayloadsAreEqual(numericType payload1, numericType payload2) {
      return (payload1 == payload2);
    }
    public override numericType PayloadFloat(float defaultValue = float.NaN) {
      numericType r = this.Payload;
      return r;
    }
    public override string Encoding {
      get {
        numericType value = this.Payload;
        string r = value.ToString("R");
        return r;
      }
    }
    public FloatAttribute(string s)
    {
      numericType value = FloatAdditions.TryParse(s);
      this.Payload = value;
    }
    public override bool PayloadIsUndefined() {
      return numericType.IsNaN(this.Payload);
    }
    public override GenericAttribute CloneAttribute() {
      return new FloatAttribute(this.Payload);
    }
  }
}
