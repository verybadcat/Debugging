using System;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public class FontAttribute: Attribute<CommonFont>
  {
    public FontAttribute()
    {
    }
    public FontAttribute(CommonFont payload) : base(payload)
    {
    }
    public FontAttribute(FontAttribute other) : this(
      other.Payload == null ? null : other.Payload
    ) {
    }
    public override bool PayloadsAreEqual(CommonFont payload1, CommonFont payload2) {
      bool r = payload1.EqualsFont(payload2);
      return r;
    }
    public override CommonFont PayloadFont(CommonFont defaultValue = null) {
      return this.Payload;
    }
    public override GenericAttribute CloneAttribute() {
      FontAttribute r = new FontAttribute(this);
      return r;
    }
    public override string Encoding {
      get {
        CommonDebug.BreakPoint();
        CommonFont font = this.PayloadFont();
        string r;
        if (font==null) {
          r = "Missing font";
        } else {
          r = font.CacheHash.ToString();
        }
        return r;
      }
    }
  }
}

