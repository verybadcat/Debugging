using System;
using Newtonsoft.Json;

namespace Jockusch.Common
{
  public class JsonEscaping: CustomEscaping
  {
    public JsonEscaping()
    {
    }

    protected override string DoEscape(string str)
    {
      return JsonConvert.SerializeObject(str);
    }

    protected override string DoUnescape(string str)
    {
      try {
        return JsonConvert.DeserializeObject<string>(str);
      } catch (Exception e) {
        e.HandleViaOS();
        return str;
      }
    }
  }
}

