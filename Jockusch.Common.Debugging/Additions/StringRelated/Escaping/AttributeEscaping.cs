using System;

namespace Jockusch.Common
{
  public class AttributeEscaping: CustomEscaping
  {


    protected override string DoEscape(string s) {
      string t = s.Replace("%", EscapedPercent);
      string u = t.Replace(",", EscapedComma);
      string r = u.Replace("}", EscapedRightBrace);
      return r;
    }
    protected override string DoUnescape(string s) {
      string t = s.Replace(EscapedRightBrace, "}");
      string u = t.Replace(EscapedComma, ",");
      string r = u.Replace(EscapedPercent, "%");
      return r;
    }
  }
}

