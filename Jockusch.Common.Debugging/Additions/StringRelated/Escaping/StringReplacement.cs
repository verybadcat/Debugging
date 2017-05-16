using System;
using System.Collections.Generic;
using System.Linq;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  /// <summary>A single replacement; case-sensitive, one search pattern and one replacement.
  /// Purpose is for easy reversal.</summary>
  public class StringReplacement
  {
    public string Search { get; set;}
    public string Replace { get; set;}
    public StringReplacement(string search, string replace)
    {
      this.Search = search;
      this.Replace = replace;
    }
    public string Apply(string target) {
      return target.Replace(this.Search, this.Replace, StringComparison.Ordinal);
    }
    public string Unapply(string target) {
      return target.Replace(this.Replace, this.Search, StringComparison.Ordinal);
    }
  }
  public static class StringReplacementAdditions {
    public static StringReplacement Reverse(this StringReplacement replacement) {
      return new StringReplacement(replacement.Replace, replacement.Search);
    }
    public static string ApplyAll(this IEnumerable<StringReplacement> replacements, string target) {
      foreach (StringReplacement replacement in replacements) {
        target = replacement.Apply(target);
      }
      return target;
    }
    /// <summary>Reverses both the individual replacements and their order.</summary>
    public static IEnumerable<StringReplacement> ReverseReplacements(this IEnumerable<StringReplacement> enumerable) {
      if (enumerable.ReturningNullCheck()) {
        IEnumerable<StringReplacement> reverseEnum = enumerable.Reverse();
        foreach (StringReplacement replacement in reverseEnum) {
          yield return replacement.Reverse();
        }
      }
    }
  }
}
