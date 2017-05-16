using System;
namespace Jockusch.Common
{
  public class DateAttribute: Attribute<DateTime>
  {
    public DateAttribute()
    {
    }

    public override string Encoding {
      get {
        DateTime value = this.Payload;
        Int64 ticks = value.Ticks;
        string stringValue = ticks.ToString();
        return stringValue;
      }
    }

    public DateAttribute(DateTime payload) : base(payload){}

    public override DateTime PayloadDateTime(DateTime defaultValue) {
      return this.Payload;
    }

    public DateAttribute(DateAttribute other): this(other.Payload) {}

    public override GenericAttribute CloneAttribute()
    {
      return new DateAttribute(this);
    }
  }
}

