using System;

namespace Jockusch.Common
{
  public class DatasetFileSaveEscaping: HumanFriendlyQuoteEscaping
  {
    public DatasetFileSaveEscaping() :base(true) {}
    protected override string DoEscape(string str) {
      string t = base.DoEscape(str);
      string r = t.Replace(StringConstants.SingleQuote, EscapedSingleQuote);
      return r;
    }
    protected override string DoUnescape(string str) {
      string t = str.Replace(EscapedSingleQuote, StringConstants.SingleQuote);
      string r = base.DoUnescape(t);
      return r;
    }
  }
}

