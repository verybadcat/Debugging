using System;
using System.Collections.Generic;
using System.Linq;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class StringReplacements
  {
    public static StringReplacement Identity() {
      return new StringReplacement("a", "a");
    }
    public static StringReplacement FromPipeSeparatedString(string input)
    {
      int index = input.IndexOfInvariant("|");
      if (index <= 0 || index == input.Length) {
        CommonDebug.BreakPoint();
        return new StringReplacement("a", "a"); // i.e, a do-nothing replacement.
      } else {
        string prefix = input.Substring(0, index);
        string rest = input.Substring(index + 1);
        return new StringReplacement(prefix, rest);
      }
    }
    public static IEnumerable<StringReplacement> FromPipeSeparatedStrings(params string[] inputs)
    {
      foreach (string str in inputs) {
        yield return StringReplacements.FromPipeSeparatedString(str);
      }
    }
  }
}
