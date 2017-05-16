using System;
using Jockusch.Common.Debugging;

namespace Jockusch.Common {
  public enum FuzzyBool {
    False,
    True,
    Maybe
  }
  public static class FuzzyBoolAdditions {
    public static bool IsFalse(this FuzzyBool fuzzy) {
      return fuzzy.Equals (FuzzyBool.False);
    }
    public static bool IsTrue(this FuzzyBool fuzzy) {
      return fuzzy.Equals (FuzzyBool.True);
    }
    public static bool CouldBeFalse(this FuzzyBool fuzzy) {
      return !fuzzy.IsTrue();
    }
    public static bool CouldBeTrue(this FuzzyBool fuzzy) {
      return !fuzzy.IsFalse();
    }
    public static bool IsMaybe (this FuzzyBool fuzzy) {
      return fuzzy.Equals (FuzzyBool.Maybe);
    }
    /// <summary>Throws an exception if fuzzy is maybe.</summary>
    public static bool ToBool (this FuzzyBool fuzzy) {
      switch (fuzzy) {
        case FuzzyBool.True:
          return true;
        case FuzzyBool.False:
          return false;
        case FuzzyBool.Maybe:
          default:
          throw new Exception("idk if FuzzyBool.Maybe is true or false.");
      }
    }
    public static FuzzyBool FirstKnown (this FuzzyBool fuzz1, FuzzyBool fuzz2) {
      FuzzyBool r = fuzz1.IsMaybe() ? fuzz2 : fuzz1;
      return r;
    }
    public static FuzzyBool Not(this FuzzyBool fuzz) {
      switch (fuzz) {
        case FuzzyBool.True:
          return FuzzyBool.False;
        case FuzzyBool.False:
          return FuzzyBool.True;
        default:
          return fuzz;
      }
    }
    public static FuzzyBool Or(this FuzzyBool fuzz1, FuzzyBool fuzz2) {
      if (fuzz1.IsTrue() || fuzz2.IsTrue()) {
        return FuzzyBool.True;
      } else if (fuzz1.IsMaybe() || fuzz2.IsMaybe()) {
        return FuzzyBool.Maybe;
      } else {
        return FuzzyBool.False;
      }
    }
    public static FuzzyBool And(this FuzzyBool fuzz1, FuzzyBool fuzz2) {
      if (fuzz1.IsFalse() || fuzz2.IsFalse()) {
        return FuzzyBool.False;
      } else if (fuzz1.IsMaybe() || fuzz2.IsMaybe()) {
        return FuzzyBool.Maybe;
      } else {
        return FuzzyBool.True;
      }
    }
    public static bool FirstKnown (this FuzzyBool fuzz, bool b) {
      switch (fuzz) {
        case FuzzyBool.False:
          return false;
        case FuzzyBool.Maybe:
          return b;
        case FuzzyBool.True:
          return true;
        default:
          throw new BreakingException();
      }
    }
    public static FuzzyBool FromBool (bool value) {
      if (value) {
        return FuzzyBool.True;
      } else {
        return FuzzyBool.False;
      }
    }
    /// <summary> All is an operation which determines whether or not the inputs are all true or all false.
    /// All true => returns true
    /// All false => returns false
    /// Any other case => returns Maybe </summary>
    public static FuzzyBool All(this FuzzyBool b1, FuzzyBool b2) {
      if (b1.IsTrue() && b2.IsTrue()) {
        return FuzzyBool.True;
      } else if (b1.IsFalse() && b2.IsFalse()) {
        return FuzzyBool.False;
      } else {
        return FuzzyBool.Maybe;
      }
    }
    /// <summary> Returns true if x is positive,
    /// false if x is negative,
    /// and Maybe for other cases (0 or NaN.)</summary>
    public static FuzzyBool TestPositive(double x) {
      if (x > 0) {
        return FuzzyBool.True;
      } else if (x < 0) {
        return FuzzyBool.False;
      } else {
        return FuzzyBool.Maybe;
      }
    }
    public static FuzzyBool TryParseQ(string encodedValue)
    {
      FuzzyBool r = FuzzyBool.Maybe;
      string lowercaseEncodedValue = encodedValue.ToLowerInvariant().Trim();
      if (lowercaseEncodedValue.StartsWithInvariant("1", "y", "t")) {
        r = FuzzyBool.True;
      } else if (lowercaseEncodedValue.StartsWithInvariant("0", "n", "f")) {
        r = FuzzyBool.False;
      }
      return r;
    }
    /// <summary>0 = false; 1=true; all others = maybe</summary>
    public static FuzzyBool TryParseQ(int encodedValue)
    {
      switch (encodedValue) {
      case 0:
        return FuzzyBool.False;
      case 1:
        return FuzzyBool.True;
      default:
        return FuzzyBool.Maybe;
      }
    }

    /// <summary>0=false; 1=true; others=maybe</summary> 
    public static FuzzyBool TryParseQ(double encodedValue)
    {
      if (encodedValue == 0) {
        return FuzzyBool.False;
      } else if (encodedValue == 1) {
        return FuzzyBool.True;
      } else {
        return FuzzyBool.Maybe;
      }
    }
  }
}
