using System;
using System.Collections.Generic;

namespace Jockusch.Common
{

  public static class StringConstants
  {

    public const string EulerE = "e";
    public const string DegreeSign = "°";
    public const string Null = "null";
    public const string Quote = "\"";
    public const string SingleQuote = @"'";
    public const string Infinity = "Infinity";
    public const string MinusInfinity = "-" + StringConstants.Infinity;
    public const string PlusInfinity = "+" + StringConstants.Infinity;
    public const string AppNameSearchReplace = "%APPNAME%";
    public const string LessThan = "<";
    public const string GreaterThan = ">";
    public const string LessThanOrEqualTo = "≤";
    public const string GreaterThanOrEqualTo = "≥";
    public const string PlusSign = "+";

    public const string ComplexNaN = "ComplexNaN";
    public const string Underscore = "_";
    public const string ImaginaryI = "i";
    public const string Complex = "Complex";
    public const string Unknown = "unknown";
    public const string Newline = "\n";
    public const string LeftBrace = "{";
    public const string RightBrace = "}";
    public const string AtString = "@";
    public const string Ans = "ans";
    public const string ParseError = "Parse error!";
    public const string Undefined = "undefined";

    public const string Ellipsis = "…";

    public static IEnumerable<string> AllNewlines() {
      yield return "\r";
      yield return "\n";
      yield return "\r\n";
    }
  }
}

