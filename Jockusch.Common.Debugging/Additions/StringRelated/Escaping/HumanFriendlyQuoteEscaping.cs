using System;

namespace Jockusch.Common
{
  // basic idea is to replace spaces with underscores. As much as possible, leave everything else alone.
  public class HumanFriendlyQuoteEscaping: CustomEscaping
  {
    public bool XToEmpty { get; set;}
    public HumanFriendlyQuoteEscaping(bool xToEmpty)
    {
      this.XToEmpty = xToEmpty;
    }
    protected override string DoEscape(string str) {
      if (str == null) {
        return "%null";
      }
      string t = str.Replace(Percent, EscapedPercent);
      string u = t.Replace(Underscore, EscapedSpace);
      string v = u.Replace(Space, Underscore);
      string r;
      if (this.XToEmpty) {
        switch (v) {
          case "x":
            r = "%x";
            break;
          case "":
            r = "x";
            break;
          default:
            r = v;
            break;
        }
      } else {
        r = v;
      }
      return r;
    }
    protected override string DoUnescape(string str) {
      string s;
      if (this.XToEmpty) {
        switch (str) {
          case "%null":
            return null;
          case "%x":
            s = "x";
            break;
          case "x":
            s = "";
            break;
          default:
            s = str;
            break;
        }
      } else {
        s = str;
      }
      string u = s.Replace(Underscore, Space);
      string v = u.Replace(EscapedSpace, Underscore);
      string r = v.Replace(EscapedPercent, Percent);
      return r;
    }
  }
}

