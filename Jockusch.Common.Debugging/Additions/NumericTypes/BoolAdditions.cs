using System;
using System.Collections.Generic;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class BoolAdditions
  {
    public static FuzzyBool ToFuzzy(this bool b) {
      return b ? FuzzyBool.True : FuzzyBool.False;
    }
    public static bool HideCompilerWarnings(this bool b) {
      return b;
    }
    public static bool TryParse(string encodedValue) {
      bool r;
      string lowercaseEncodedValue = encodedValue.ToLowerInvariant().Trim();
      if (lowercaseEncodedValue.StartsWithInvariant("1")||lowercaseEncodedValue.StartsWithInvariant("y")
        ||lowercaseEncodedValue.StartsWithInvariant("t")) {
        r = true;
      } else {
        r = false;
      }
      return r;
    }
    public static IEnumerable<bool> AllBools {
      get {
        yield return false;
        yield return true;
      }
    }
    public static int ToInt(this bool b)
    {
      return b ? 1 : 0;
    }
    public static bool AssertFalse(this bool b) {
      bool r = !b;
      CommonDebug.Assert(r);
      return r;
    }
    public static bool AssertTrue(this bool b) {
      CommonDebug.Assert(b);
      return b;
    }
  }
}

