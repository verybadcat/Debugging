using System;

namespace Jockusch.Common
{
  public static class CharAdditions
  {
    /// <summary>Does not include decimal point or minus
    /// sign. See also IsNumeric.</summary>
    public static bool IsDigit(this char c) {
      bool r = (c >= '0' && c <= '9');
      return r;
    }
    /// <summary>Includes decimal point, minus sign, and comma.</summary>
    public static bool IsNumeric(this char c) {
      bool r = false;
      if (c >= '0' && c <= '9') {
        r = true;
      }
      if (c == '.' || c == '-' || c == ',') {
        r = true;
      }
      return r;
    }
    public static bool IsRelation(this char c) {
      switch (c) {
        case '=':
        case '<':
        case '>':
        case '≤':
        case '≥':
          return true;
        default:
          return false;
      }
    }
    public static FuzzyBool IsVowel(this char c) {
      switch (c) {
        case 'a':
        case 'e':
        case 'i':
        case 'o':
        case 'u':
          return FuzzyBool.True;
        case 'y':
        case 'w':
          return FuzzyBool.Maybe;
        default:
          return FuzzyBool.False;
      }
    }
    /// <summary> See AlphaLetter in StringReaderLexer.g4</summary>
    public static bool IsLexerLetter(this char c) {
      if (c >= 'A' && c <= 'z') {
        return true;
      }
      return false;
    }
    public static bool IsLexerDigit(this char c) {
      bool r = c.IsDigit();
      return r;
    }
    /// <summary> See AlphaLetter in StringReaderLexer.g4</summary>
    public static bool IsLexerAlphaLetter(this char c) {
      if (c.IsLexerLetter()) {
        return true;
      } else if (c.IsLexerDigit()) {
        return true;
      } else if (c == '_') {
        return true;
      } else {
        return false;
      }
    }

  }
}

