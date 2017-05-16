using System;
using System.Collections.Generic;
using Jockusch.Common.Debugging;
using System.ComponentModel;

namespace Jockusch.Common
{
  public static class AnyType
  {
    public static bool EqualsDefault<T>(T t) {
      EqualityComparer<T> comparer = EqualityComparer<T>.Default;
      T defaultT = default(T);
      bool r = comparer.Equals(t, defaultT);
      return r;
    }
    public static bool DoesEqual<T>(T t, T s) {
      EqualityComparer<T> comparer = EqualityComparer<T>.Default;
      bool r = comparer.Equals(t, s);
      return r;
    }
    public static bool DoesNotEqual<T>(T t, T s) {
      bool equal = DoesEqual(t, s);
      bool r = !equal;
      return r;
    }
    public static string DebugName(object obj, bool createIfNeeded = true) {
      if (createIfNeeded) {
        return ObjectNamer.NameForObject(obj);
      } else {
        return ObjectNamer.ExistingNameForObject(obj);
      }
    }
    public static bool DebugNameStartsWith(object obj, bool createNameIfNeeded, params string[] prefixes) {
      bool r = false;
      string name = DebugName(obj, createNameIfNeeded);
      foreach (string prefix in prefixes) {
        if (name?.StartsWithInvariant(prefix) == true) {
          r = true;
          break;
        }
      }
      return r;
    }
    public static bool DebugNameStartsWith(object obj, params string[] prefixes) {
      return DebugNameStartsWith(obj, false, prefixes);
    }
    public static T MaybeApply<T>(T t, Func<T, T> function, bool really) {
      T r;
      if (really && !EqualsDefault(t)) {
        r = function(t);
      } else {
        r = t;
      }
      return r;
    }
    /// <summary>Error handling is invoked only if the input object has a non-defualt value and
    /// is not of the target output type.</summary>
    public static TP Cast<T, TP>(this T t, ErrorHandling handling) {
      if (t is TP) {
        return (TP)(object)(t);
      } else {
        if (!EqualsDefault(t)) {
          handling.HandleError();
        }
        return default(TP);
      }
    }
    public static string ToStringWithBackup(object obj, string backup = "null") {
      return obj?.ToString() ?? backup;
    }
    public static bool ReturningNullCheck(this object obj) {
      return CommonDebug.ReturningNullCheck(obj);
    }

    /// <summary>If q is null, breakpoint, then returns default(T).</summary>
    public static T ReturnOrConvert<T, Q>(Q q, Func<Q, T> convert)
      where T: Q {
      if (q == null) {
        CommonDebug.BreakPoint();
        return default(T);
      }
      if (q is T) {
        return (T)q;
      } else {
        return convert(q);
      }
    }
  }
}

