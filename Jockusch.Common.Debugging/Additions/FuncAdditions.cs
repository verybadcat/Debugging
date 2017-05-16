using System;
using System.Collections.Generic;
using Jockusch.Common.Debugging;
using System.Linq;

namespace Jockusch.Common
{
  public static class FuncAdditions
  {
    /// <summary>Continues applying the function until an item returns either itself or default(T).
    /// Returns the last non-default T (i.e, the last non-null object) found. In particular, if
    /// function(input)==null, will return the original input.</summary>
    public static U FollowChainToEnd<T, U>(this Func<T, U> function, T input)
      where U: T {
      U r = function(input);
      if (AnyType.EqualsDefault(r)) {
        if (input is U) {
          r = (U)input;
        }
      }
#if DEBUG
      int recursionCheck = 0;
      #endif
      while (!AnyType.EqualsDefault(r)) {
#if DEBUG
        recursionCheck++;
        if (recursionCheck == 100) {
          CommonDebug.BreakPoint();
          return r;
        }
        #endif
        U r1 = function(r);
        if (AnyType.DoesEqual(r1, r)) {
          break;
        } else {
          r = r1;
        }
      }
      return r;
    }
    public static Func<DSub, R> NarrowDomain<D, R, DSub>(this Func<D, R> function)
      where DSub : D
    {
      return (ds) => function(ds);
    }
    public static Func<D, RSup> BroadenRange<D, R, RSup>(this Func<D, R> function)
      where R : RSup
    {
      return (d) => function(d);
    }
    public static Func<DP, R> CastDomain<D, R, DP>(this Func<D, R> function, ErrorHandling handling)
    {
      return (DP dp) => {
        D d = dp.Cast<DP, D>(handling);
        return function(d);
      };
    }
    public static Func<T, V> Compose<T, U, V>(this Func<T, U> f1, Func<U, V> f2)
    {
      return (T t) => {
        U u = f1(t);
        V v = f2(u);
        return v;
      };
    }
    /// <summary>If the input is null, a null output will not raise any alarms.</summary>
    public static U InvokeNullChecking<T, U>(this Func<T, U> f, T t)
    {
      U r = default(U);
      if (CommonDebug.ReturningNullCheck(f)) {
        r = f(t);
#if DEBUG
      if (t != null) {
        if (!CommonDebug.ReturningNullCheck(r)) {
          r = f(t);
        }
      }
#endif
      }
      return r;
    }
    public static V InvokeNullChecking<T, U, V>(this Func<T, U, V> f, T t, U u)
    {
      V r = default(V);
      if (CommonDebug.ReturningNullCheck(f)) {
        r = f(t, u);
#if DEBUG
      if (t!=null && u!=null) {
        if (!CommonDebug.ReturningNullCheck(r)) {
          r = f(t, u);
        }
      }
#endif
      }
      return r;
    }
    public static U SafeInvoke<T, U>(this Func<T, U> f, T t, ErrorHandling handling) {
      U r = default(U);
      if (f!=null) {
        r = f(t);
      } else {
        handling.HandleError();
      }
      return r;
    }
    /// <summary>If called on null object or with null function, returns the backup value</summary>
    public static U SafeInvoke<T, U>(Func<T, U> function, T t, U backupValue) {
      U r;
      if (t == null || function == null) {
        r = backupValue;
      } else {
        r = function(t);
      }
      return r;
    }
    /// <summary>If the function is null and no backup is passed in, this will return default(U).</summary>
    public static U SafeInvoke<T, U>(this Func<T, U> f, T t, Func<U> backup = null) {
      if (f == null) {
        if (backup == null) {
          return default(U);
        } else {
          return backup();
        }
      } else {
        return f(t);
      }
    }
    /// <summary>If the function is null and no backup is passed in, this will return default(T).</summary>
    public static T SafeInvoke<T>(this Func<T> f, Func<T> backup = null) {
      if (f == null) {
        if (backup == null) {
          return default(T);
        } else {
          return backup();
        }
      } else {
        return f();
      }
    }
    public static IEnumerable<T> Chain<T>(this Func<T, T> map, T seed) {
      T value = seed;
      while (!(AnyType.EqualsDefault(value))) {
        yield return value;
        value = map(value);
      }
    }

    public static U FollowChain<T, U>(this Func<T, T> map, T seed)
    where U: class, T {
      IEnumerable<T> chain = map.Chain(seed);
      foreach (T t in chain) {
        if (t is U) {
          return (U)t;
        }
      }
      return null;
    }
  }
}

