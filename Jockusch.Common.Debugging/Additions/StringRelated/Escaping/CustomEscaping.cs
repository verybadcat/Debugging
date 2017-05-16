using System;

namespace Jockusch.Common
{
  public abstract class CustomEscaping
  {
    protected const string Percent = "%";
    protected const string EscapedPercent = "%25";
    protected const string Comma = ",";
    protected const string EscapedComma = "%2C";
    protected const string RightBrace = "}";
    protected const string EscapedRightBrace = "%7D";
    protected const string Space = " ";
    protected const string EscapedSpace = "%20";
    protected const string Underscore = "_";
    protected const string EscapedSingleQuote = "%27";
    protected const string Carat = "^";
    protected const string Asterisk = "*";
    protected const string Minus = "-";

    protected abstract string DoEscape (string str);
    protected abstract string DoUnescape (string str);
    public virtual string Escape(string str) {
      string r = null;
      if (str!=null) {
        r = this.DoEscape(str);
      }
      return r;
    }
    public virtual string Unescape(string str) {
      string r = null;
      if (str!=null) {
        r = this.DoUnescape(str);
      }
      return r;
    }
  }
}

