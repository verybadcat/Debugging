using System;

namespace Jockusch.Common
{
  public class ParenType
  {
    public string Open {get; private set;}
    public string Close { get; private set;}
    public ParenType(string open, string close)
    {
      this.Open = open;
      this.Close = close;
    }
  }
}

