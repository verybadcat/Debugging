using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Jockusch.Common {
  public class PointFAttribute: Attribute<PointF> {
    public PointFAttribute(PointF point)
      : base(point) {
    }
    public override bool PayloadsAreEqual(PointF payload1, PointF payload2) {
      return (payload1 == payload2);
    }
    private PointFAttribute(){
    }
    public override PointF PayloadPointF(PointF defaultValue) {
        PointF r = this.Payload;
        return r;
    }
    public override string Encoding {
      get {
        string r = this.Payload.Encoding();
        return r;
      }
    }
    public PointFAttribute (string encodedLocation) {
      PointF point = PointFAdditions.Decode(encodedLocation);
      this.Payload = point;
    }
    public override bool PayloadIsUndefined() {
      float x = this.Payload.X;
      float y = this.Payload.Y;
      bool r = (float.IsNaN(x) || float.IsNaN(y));
      return r;
    }
    public override GenericAttribute CloneAttribute() {
      return new PointFAttribute(this.Payload);
    }
  }
}
