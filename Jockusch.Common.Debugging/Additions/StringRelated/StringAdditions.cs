using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class StringAdditions
  {
    #region identity (what kind of a string am I?)
    public static bool IsNonempty(this string s) {
      bool r = !(string.IsNullOrEmpty(s));
      return r;
    }
    public static bool IsNotWhiteSpace(this string s) {
      bool r = !(string.IsNullOrWhiteSpace(s));
      return r;
    }

    #endregion
    #region getting substrings
    /// <summary>Safe, including if str = null. If str does not contain
    /// toThis, will returns str. Otherwise, returns everything before the
    /// first occurrence of toThis, not including toThis itself.</summary>
    public static string SubstringToFirstOccurrence(this string str, string toThis) {
      if (str == null) {
        return null;
      }
      int index = str.IndexOfInvariant(toThis);
      if (index == -1) {
        return str;
      }
      return str.Substring(0, index);
    }
    /// <summary>For example, "1+2+3=3+3=6".SubstringFromFirstOccurrence("=") = "3+3=6".</summary>
    public static string SubstringFromFirstOccurrence(this string str, string afterThis) {
      int index = str.IndexOfInvariant(afterThis);
      string r;
      if (index == -1) {
        r = str;
      } else {
        r = str.Substring(index + afterThis.Length);
      }
      return r;
    }
    public static string SubstringFromLastOccurrence(this string str, string afterMe) {
      int index = str.LastIndexOfInvariant(afterMe);
      string r;
      if (index == -1) {
        r = str;
      } else {
        r = str.Substring(index + afterMe.Length);
      }
      return r;
    }
    #endregion
    #region prefix and suffix manipulations
    /// <summary>Null-propogating. In other words, null input causes null output, no crash.
    /// If repeat is true, keeps on removing the prefix as long as its there.</summary>
    public static string RemovePrefix(this string s, string prefix, bool repeat = false) {
      string r = s;
      if (r != null) {
        if (!(string.IsNullOrEmpty(prefix))) {
          while (r.StartsWithInvariant(prefix)) {
            r = r.Substring(prefix.Length);
            if (!repeat) {
              break;
            }
          }
        }
      }
      return r;
    }
    /// <summary>Returns the number of copies of the prefix at the start of the string.
    /// If the prefix is null or empty, returns -1.</summary>
    public static int PrefixCount(this string s, string prefix) {
      int r = 0;
      if (string.IsNullOrEmpty(prefix)) {
        r = -1;
      } else {
        while (s.StartsWithInvariant(prefix)) {
          s = s.Substring(prefix.Length);
          r++;
        }
      }
      return r;
    }
    public static string RemoveSuffix(this string s, string suffix, bool repeat = false) {
      string r = s;
      if (r != null) {
        if (!(string.IsNullOrEmpty(suffix))) {
          while (r.EndsWithInvariant(suffix)) {
            r = r.Substring(0, r.Length - suffix.Length);
            if (!repeat) {
              break;
            }
          }
        }
      }
      return r;
    }
    public static string StringWithPrefix(this string s, string prefix) {
      if (string.IsNullOrEmpty(prefix)) {
        return s;
      }
      if (string.IsNullOrEmpty(s)) {
        return prefix;
      }
      if (s.StartsWithInvariant(prefix)) {
        return s;
      }
      return prefix + s;
    }
    /// <summary>The returned string is guaranteed to end with the suffix,
    /// unless said suffix is null. If the input string already ends with
    /// the suffix, it is returned</summary>
    public static string StringWithSuffix(this string s, string suffix) {
      if (string.IsNullOrEmpty(suffix)) {
        return s;
      }
      if (string.IsNullOrEmpty(s)) {
        return suffix;
      }
      if (s.EndsWithInvariant(suffix)) {
        return s;
      }
      return s + suffix;
    }
    #endregion
    #region English language
    /// <summary>"a" or "an"</summary>
    public static string GetArticle(this string s) {
      if (string.IsNullOrWhiteSpace(s)) {
        return "";
      } else if (s.ToLowerInvariant().Trim()[0].IsVowel().IsTrue()) {
        return "an";
      } else {
        return "a";
      }
    }
    public static string PluralString(this string noun, int n, bool includeN) {
      if (n == 1) {
        if (includeN) {
          return "1 " + noun;
        } else {
          return noun;
        }
      } else {
        return n + " " + noun + "s";
      }
    }
    #endregion
    #region looking at a string as a list of lines
    /// <summary>Does not remove empty entries</summary>
    public static string[] SplitIntoLines(this string s) {
      string cleaned = s.Replace("\r\n", "\n");
      string[] r = cleaned.Split(new[] { '\r', '\n' });
      return r;
    }

    public static string RemoveLines(this string s, Predicate<string> remove) {
      string r = s;
      if (remove != null) {
        List<string> lines = s.SplitIntoLines().ToList();
        for (int i = lines.Count - 1; i >= 0; i--) {
          string line = lines[i];
          if (remove(line)) {
            lines.RemoveAt(i);
          }
        }
        r = AssembleFromLines(lines);
      }
      return r;
    }

    public static string SimplifyNewlines(this string str) {
      return str.Replace("\r\n", "\n");
    }

    public static string IndentEachLine(this string s, string prefix = "  ") {
      string r;
      if (s == null) {
        r = null;
      } else {
        string[] split = s.SplitIntoLines();
        r = "";
        bool first = true;
        foreach (string line in split) {
          if (first) {
            first = false;
          } else {
            r += Environment.NewLine;
          }
          r += (prefix + split);
        }
      }
      return r;
    }
    public static string AssembleFromLines(params string[] lines) {
      return string.Join("\n", lines);
    }
    public static string AssembleFromLines(IEnumerable<string> lines) {
      return AssembleFromLines(lines.ToArray());
    }
    #endregion
    #region conveniences for StartsWith, EndsWith, and substring searches

    public static bool StartsWithInvariant(this string s, string prefix) {
      bool r = false;
      if (s != null && prefix != null) {
        r = s.StartsWith(prefix, StringComparison.Ordinal);
      }
      return r;
    }
    /// <summary>Returns true iff the string starts with any of the prefixes</summary>
    public static bool StartsWithInvariant(this string s, string prefix1, string prefix2) {
      bool r = s.StartsWithInvariant(prefix1) || s.StartsWithInvariant(prefix2);
      return r;
    }
    /// <summary>Returns true iff the string starts with any of the prefixes</summary>
    public static bool StartsWithInvariant(this string s, string prefix1, string prefix2, string prefix3) {
      bool r = s.StartsWithInvariant(prefix1, prefix2) || s.StartsWithInvariant(prefix3);
      return r;
    }
    public static bool StartsWithInvariantIgnoreCase(this string s, string prefix) {
      bool r = false;
      if (s != null && prefix != null) {
        r = s.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
      }
      return r;
    }
    public static bool EndsWithInvariant(this string s, string suffix) {
      bool r = false;
      if (s != null && suffix != null) {
        r = s.EndsWith(suffix, StringComparison.Ordinal);
      }
      return r;
    }
    public static bool EndsWithInvariantIgnoreCase(this string s, string suffix) {
      bool r = false;
      if (s != null && suffix != null) {
        r = s.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
      }
      return r;
    }
    /// <summary>Returns -1 if either string is null.</summary> 
    public static int SafeIndexOf(this string s, string substring, StringComparison comparison) {
      int r = -1;
      if (s != null && !(string.IsNullOrEmpty(substring))) {
        r = s.IndexOf(substring, comparison);
      }
      return r;
    }
    public static int IndexOfInvariant(this string s, string substring) {
      return s.SafeIndexOf(substring, StringComparison.Ordinal);
    }
    public static int LastIndexOfInvariant(this string s, string substring) {
      return s.LastIndexOf(substring, StringComparison.Ordinal);
    }
    public static int IndexOfInvariantIgnoreCase(this string s, string substring) {
      return s.SafeIndexOf(substring, StringComparison.OrdinalIgnoreCase);
    }
    public static int IndexOfInvariant(this string s, string substring, int startIndex) {
      int r = -1;
      if (s != null && !(string.IsNullOrEmpty(substring))) {
        r = s.IndexOf(substring, startIndex, StringComparison.Ordinal);
      }
      return r;
    }
    /// <summary>Returns false if either string is null.</summary> 
    public static bool ContainsInvariant(this string s, string substring) {
      int index = s.IndexOfInvariant(substring);
      return (index != -1);
    }

    public static bool ContainsInvariant(this string s, string substring, bool ignoreCase, bool ignoreCommas) {
      if (ignoreCommas) {
        s = s.Replace(",", "");
        substring = substring.Replace(",", "");
      }
      bool r = ignoreCase ? s.ContainsInvariantIgnoreCase(substring) : s.ContainsInvariant(substring);
      return r;
    }

    public static bool ContainsInvariantIgnoreCase(this string s, string substring) {
      int index = s.IndexOfInvariantIgnoreCase(substring);
      return (index != -1);
    }

    public static bool ContainsIgnoreSpaces(this string s, string substring, bool ignoreCase = true, bool ignoreCommas = false) {
      string withoutSpaces = s.Replace(" ", "");
      string withoutSpaces2 = substring.Replace(" ", "");
      bool r = withoutSpaces.ContainsInvariant(withoutSpaces2, ignoreCase, ignoreCommas);
      if (substring == "save") {
        CommonDebug.LogLine(s);
      }
      return r;
    }

    #endregion
    #region path and filename-related (not applicable to every os.)
    public static string CommonPathPrefix(string s1, string s2) {
      int _, __;
      return StringAdditions.CommonPathPrefix(s1, s2, out _, out __);
    }
    public static string CommonPathPrefix(string s1, string s2, out int suffix1Length, out int suffix2Length) {
      string[] splits1 = s1.Split('/');
      string[] splits2 = s2.Split('/');
      int count1 = splits1.Count();
      int count2 = splits2.Count();
      int count = Math.Max(count1, count2);
      int nMatching = 0;
      for (int i = 0; i < count; i++) {
        if (splits1[i] == splits2[i]) {
          nMatching++;
        } else {
          break;
        }
      }
      suffix1Length = count1 - nMatching;
      suffix2Length = count2 - nMatching;
      string r = string.Join("/", splits1.Take(nMatching));
      return r;
    }
    /// <summary>Not actually completely reliable, unfortunately,
    /// as Path.GetInvalidFileNameChars() is not completely reliable.</summary>
    public static string ToValidFilename(this string s) {
      char[] invalid = Path.GetInvalidFileNameChars();
      string invalidString = new string(invalid);
      char[] titleChars = s.ToCharArray();
      char[] output = new Char[s.Length];
      int outputLength = 0;
      foreach (char input in titleChars) {
        if (invalidString.IndexOf(input) == -1) {
          output[outputLength] = input;
          outputLength++;
        }
      }
      string rawR = new string(output, 0, outputLength);
      string r = rawR.Trim();
      if (r == "") {
        r = "NonAlphaName";
      }
      return r;
    }
    #endregion
    public static string FirstNonempty(string s1, string s2 = null, string s3 = null, string s4 = null) {
      string r = s1;
      if (string.IsNullOrEmpty(r)) {
        r = s2;
        if (string.IsNullOrEmpty(r)) {
          r = s3;
          if (string.IsNullOrEmpty(r)) {
            r = s4;
          }
        }
      }
      return r;
    }

    public static List<string> ToList(this string s) {
      return new List<string> { s };
    }

    public static string AppendIf(this string str, bool condition, string appendMe) {
      if (condition) {
        return str + appendMe;
      } else {
        return str;
      }
    }
    public static string RemoveCharactersInString(this string str, string removeUs) {
      string r = str;
      if (removeUs != null) {
        char[] chars = removeUs.ToCharArray();
        foreach (char c in chars) {
          r = r.Replace(c.ToString(), "");
        }
      }
      return r;
    }
    public static string RemoveWhitespace(this string str) {
      string r = str.RemoveCharactersInString(" \r\n\t");
      return r;
    }


    /// <summary>null string returns zero</summary>
    public static int SafeLength(this string str) {
      if (str == null) {
        return 0;
      } else {
        return str.Length;
      }
    }
    public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison) {
      StringBuilder sb = new StringBuilder();

      int previousIndex = 0;
      int index = str.IndexOf(oldValue, comparison);
      while (index != -1) {
        sb.Append(str.Substring(previousIndex, index - previousIndex));
        sb.Append(newValue);
        index += oldValue.Length;

        previousIndex = index;
        index = str.IndexOf(oldValue, index, comparison);
      }
      sb.Append(str.Substring(previousIndex));

      string r = sb.ToString();
      return r;
    }
    /// <summary>Replaces each double quote (") with two double quotes (""). No other changes.</summary>
    public static string EscapeQuotes(this string s) {
      string quote = StringConstants.Quote;
      string doubleQuote = quote + quote;
      if (s.ContainsInvariant(quote)) {
        return s.Replace(quote, doubleQuote);
      } else {
        return s;
      }
    }
    public static string UnescapeQuotesIf(this string s, bool condition) {
      if (condition) {
        string quote = StringConstants.Quote;
        string doubleQuote = quote + quote;
        if (s.ContainsInvariant(doubleQuote)) {
          return s.Replace(doubleQuote, quote);
        }
      }
      return s;
    }

    public static string TrimCharsInString(this string s, string trimChars) {
      string r = s.Trim(trimChars.ToCharArray());
      return r;
    }

    public static string RemoveNumberCommas(this string str) {
      double value = DoubleAdditions.TryParse(str);
      if (!double.IsNaN(value)) {
        str = str.Replace(",", "");
      }
      return str;
    }

    public static string RemoveWrappingQuotes(this string str) {
      string quote = StringConstants.Quote;
      if (str.StartsWithInvariant(quote) && str.EndsWithInvariant(quote) && str.Length > 1) {
        return str.Substring(1, str.Length - 2);
      } else {
        return str;
      }
    }

    /// <summary>If the string is already wrapped in quotes, it is returned,
    /// without any further wrapping.</summary> 
    public static string WrapInQuotes (this string str) {
      string quote = StringConstants.Quote;
      string r = str.StringWithPrefix(quote).StringWithSuffix(quote);
      return r;
    }


    public static string InsertSpaced(this string s, int startIndex, int maxCopies, int spacing, char insertMe) {
      IEnumerable<char> chars = s.ToCharArray();
      IEnumerable<char> moreChars = chars.InsertSpaced(startIndex, maxCopies, spacing, insertMe);
      char[] moreArray = moreChars.ToArray();
      string r = new string(moreArray);
      return r;
    }
    /// <summary>Useful for checking if a string is likely to be text. Returns NaN if s is null or empty.</summary>
    public static double AsciiProportion(this string s) {
      double r;
      if (s.IsNullOrEmpty()) {
        r = double.NaN;
      } else {
        char[] chars = s.ToCharArray();
        int n = chars.Count();
        int nAscii = chars.Count(c => c < 128);
        r = nAscii / (double)n;
      }
      return r;
    }
  }
}
