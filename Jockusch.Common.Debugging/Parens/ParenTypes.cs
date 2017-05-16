using System;

namespace Jockusch.Common
{
  public static class ParenTypes
  {
    public static ParenType Parens = new ParenType("(", ")");
    public static ParenType SquareBrackets = new ParenType("[", "]");
    public static ParenType CurlyBraces = new ParenType("{", "}");
    public static ParenType None = new ParenType("", "");
  }
}

