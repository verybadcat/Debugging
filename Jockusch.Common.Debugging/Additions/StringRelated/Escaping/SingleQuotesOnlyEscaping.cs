using System;

namespace Jockusch.Common
{
  public class SingleQuotesOnlyEscaping: CustomEscaping
  {
    public SingleQuotesOnlyEscaping()
    {
    }
    protected override string DoEscape(string str) {
      string s = str.Replace(Percent, EscapedPercent);
      string r = s.Replace(StringConstants.SingleQuote, EscapedSingleQuote);
      return r;
    }
    protected override string DoUnescape(string str) {
      string s = str.Replace(EscapedSingleQuote, StringConstants.SingleQuote);
      string r = s.Replace(EscapedPercent, Percent);
      return r;
    }
  }
}

