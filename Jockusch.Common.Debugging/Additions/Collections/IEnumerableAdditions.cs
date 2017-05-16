using System;
using System.Collections;
using System.Collections.Generic;
using Jockusch.Common.Debugging;
using System.Linq;
using System.Diagnostics;

namespace Jockusch.Common
{
  public static class IEnumerableAdditions
  {
    /// <summary>Intended for situations where other threads may modify the enumerable, but won't do
    /// so very often, and furthermore the enumeration procedure itself can be redone without harm.
    /// If an exception is thrown, we simply give up and retry.</summary>
    [DebuggerNonUserCode]
    public static void RetryingEnumerate<T>(this IEnumerable<T> enumerable, Action<T> action, int maxAllowedRetries = 3) {
      if (enumerable.IsNonempty()) {
        try {
          foreach (T t in enumerable) {
            action(t);
          }
        }
        catch {
          if (maxAllowedRetries > 0) {
            enumerable.RetryingEnumerate(action, maxAllowedRetries - 1);
          } else {
            CommonDebug.BreakPoint();
            throw;
          }
        }
      }
    }
    /// <summary>The integer argument of the action is the index within the enumerable.</summary> 
    [DebuggerNonUserCode]
    public static void RetryingEnumerate<T>(this IEnumerable<T> enumerable, Action<T, int> action, int maxAllowedRetries = 3) {
      if (enumerable.IsNonempty()) {
        int i = 0;
        try {
          foreach(T t in enumerable) {
            action(t, i);
            i++;
          }
        }
        catch {
          if (maxAllowedRetries > 0) {
            enumerable.RetryingEnumerate(action, maxAllowedRetries - 1);
          } else {
            CommonDebug.BreakPoint();
            throw;
          }
        }
      }
    }
    public static bool IsNullOrEmpty(this IEnumerable enumerable)
    {
      if (enumerable != null)
      {
        return enumerable.IsEmptyEnumerable();
      }
      return true;
    }
    public static bool IsNonempty(this IEnumerable enumerable) {
      if (enumerable!=null) {
        foreach(object obj in enumerable) {
          return true;
        }
      }
      return false;
    }
    public static bool IsEmptyEnumerable(this IEnumerable enumerable) {
      foreach (object obj in enumerable) {
        return false;
      }
      return true;
    }
    /// <summary>If exactly one entry in the enumerable satisfies the predicate,
    /// returns it. Otherwise, returns default(T).</summary>
    public static T OnlyOrDefault<T>(this IEnumerable<T> enumerable, Predicate<T> predicate) {
      T r = default(T);
      int found = 0;
      if (enumerable != null) {
        foreach (T t in enumerable) {
          if (predicate(t)) {
            r = t;
            found++;
          }
        }
      }
      if (found > 1) {
        r = default(T);
      }
      return r;
    }
    /// <summary>Searches the enumerable for the first entry that satisfies the condition, then
    /// returns its index. Returns -1 if not found, or if the enumerable is null.</summary>
    public static int FirstIndex<T>(this IEnumerable<T> enumerable, Predicate<T> condition) {
      if (enumerable!=null) {
        int index = 0;
        foreach (T t in enumerable) {
          if (condition(t)) {
            return index;
          }
          index++;
        }
      }
      return -1;
    }
    /// <summary>If exactly one entry of the enumerable satisfies the predicate, returns its index.
    /// Otherwise, returns -1.
    public static int OnlyIndexOrNegativeOne<T>(this IEnumerable<T> enumerable, Predicate<T> predicate) {
      int r = -1;
      if (enumerable != null) {
        int i = 0;
        foreach (T t in enumerable) {
          if (predicate(t)) {
            if (r == -1) {
              r = i;
            } else {
              r = -1; 
              break;
            }
          }
          i++;
        }
      }
      return r;
    }
    public static T InvokeInSeries<T>(this IEnumerable<Func<T, T>> functions, T target) {
      T r = target;
      foreach (Func<T, T> function in functions) {
        r = function(r);
      }
      return r;
    }
		public static IEnumerable<T> SkipFirst<T>(this IEnumerable<T> enumerable) {
			if (enumerable.IsNullOrEmpty()) {
				CommonDebug.BreakPoint();
				return enumerable;
			} else {
				return enumerable.Reverse().Take(enumerable.Count() - 1).Reverse();
			}
		}
		public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> enumerable) {
			if (enumerable.IsNullOrEmpty()) {
				CommonDebug.BreakPoint();
				return enumerable;
			} else {
				return enumerable.Take(enumerable.Count() - 1);
			}
		}
    public static bool EqualsEnumerable<T>(this IEnumerable<T> enumerable, IEnumerable<T> otherEnumerable,
      Func<T, T, bool> equalityTester = null) {
      if (equalityTester == null) {
        equalityTester = (T t1, T t2) => t1.Equals(t2);
      }
      bool r;
      if (enumerable == null && otherEnumerable == null) {
        r = true;  
      } else if (enumerable != null && otherEnumerable != null) {
        r = enumerable.Count() == otherEnumerable.Count();
        if (r) {
          using (IEnumerator<T> enum1 = enumerable.GetEnumerator())
          using (IEnumerator<T> enum2 = otherEnumerable.GetEnumerator()) {
            while (r && enum1.MoveNext() && enum2.MoveNext()) {
              T curr1 = enum1.Current;
              T curr2 = enum2.Current;
              r = equalityTester(curr1, curr2);
            }
          }
        }
      } else {
        // one enumerable is null and the other isn't
        r = false;
      }
      return r;
    }
    /// <summary>Returns the first entry in the enumerable that does not
    /// equal the special value, or the special value if none is found.</summary>
    public static T Coalesce<T>(this IEnumerable<T> values, T specialValue) {
      T r = specialValue;
      EqualityComparer<T> comparer = EqualityComparer<T>.Default;
      foreach(T value in values) {
        if (comparer.Equals(r, specialValue)) {
          r = value;
        } else {
          break;
        }
      }
      return r;
    }
    public static TOutput Coalesce<TInput, TOutput>(this IEnumerable<TInput> enumerable, Func<TInput, TOutput> map, TOutput specialValue) {
      IEnumerable<TOutput> outputEnumerable = enumerable.Select(input => map(input));
      TOutput r = outputEnumerable.Coalesce(specialValue);
      return r;
    }
    public static TOutput Coalesce<TInput, TOutput>(this IEnumerable<TInput> enumerable, Func<TInput, TOutput> map,
                                                    Predicate<TOutput> valueIsSpecial, Func<TOutput> getDefaultValue) {
      IEnumerable<TOutput> outputEnumerable = enumerable.Select(input => map(input));
      TOutput r = outputEnumerable.Coalesce(valueIsSpecial, getDefaultValue);
      return r;
    }
    public static T Coalesce<T>(this IEnumerable<T> enumerable, 
                                Predicate<T> valueIsSpecial, 
                                Func<T> getDefaultValue) {
      foreach (T t in enumerable) {
        if (valueIsSpecial(t)) {
          continue;
        } else {
          return t;
        }
      }
      return getDefaultValue();
    }
    public static int SafeCount<T>(this IEnumerable<T> enumerable) {
      if (enumerable == null) {
        return 0;
      } else {
        return enumerable.Count();
      }
    }
    /// <summary>Null is mapped to the empty enumerable.</summary>
    public static IEnumerable<U> SafeSelect<T, U>(this IEnumerable<T> input, Func<T, U> map) {
      if (input == null) {
        return Enumerable.Empty<U>();
      } else {
        return input.Select(map);
      }
    }
    public static void CoEnumerate<T1, T2>( this IEnumerable<T1> first, IEnumerable<T2> second, Action<T1, T2> action)
    {
      using (var e1 = first.GetEnumerator())
      using (var e2 = second.GetEnumerator())
      {
        while (e1.MoveNext() && e2.MoveNext())
        {
          action(e1.Current, e2.Current);
        }
      }
    }
    /// <summary>Winner is the first enum to win a comparison.
    /// If they tie at every step, the longer enum wins.
    /// If that doesn't break the tie, they are tied.</summary>
    public static int Compare<T> (IEnumerable<T> first, IEnumerable<T> second, IComparer<T> innerComparer)
    {
      var e1 = first.GetEnumerator();
      var e2 = second.GetEnumerator();
      while (true)
      {
        bool next1 = e1.MoveNext();
        bool next2 = e2.MoveNext();
        if (!(next1 || next2))
        {
          return 0;
        }
        if (next1 && !next2)
        {
          return -1;
        }
        if (next2 && !next1)
        {
          return 1;
        }
        T one = e1.Current;
        T two = e2.Current;
        int innerCompare = innerComparer.Compare(one, two);
        if (innerCompare!=0)
        {
          return innerCompare;
        }
      }
    }


    public static IEnumerable<T> InsertSpaced<T>(this IEnumerable<T> enumerable, int startIndex, int maxCopies, int spacing, T insertMe) {
      int index = 0;
      if (spacing == 0) {
        foreach (T t in enumerable) {
          if (index == startIndex) {
            for (int i = 0; i < maxCopies; i++) {
              yield return insertMe;
            }
          }
          yield return t;
        }
      } else {
        foreach (T t in enumerable) {
          if (index >= startIndex && (index - startIndex) % spacing == 0 && (index - startIndex) / spacing < maxCopies) {
            yield return insertMe;
          }
          yield return t;
          index++;
        }
      }
    }

    public static IEnumerable<U> AsPairs<T, U>(this IEnumerable<T> enumerable, Func<T, T, U> makeU) {
      T cache = default(T);
      bool haveCache = false;
      foreach(T t in enumerable) {
        if (haveCache) {
          U u = makeU(cache, t);
          yield return u;
        } else {
          cache = t;
        }
        haveCache = !haveCache;
      }
    }
    public static IEnumerable<object> ToObjectEnumerable<T>(this IEnumerable<T> enumerable)
    {
      foreach (T obj in enumerable)
      {
        yield return obj;
      }
    }
    public static T SafeFirstOrDefault<T>(this IEnumerable<T> enumerable) {
      if (enumerable == null) {
        return default(T);
      } else {
        return enumerable.FirstOrDefault();
      }
    }
    /// <summary>Null goes to the empty list. This is still
    /// "unsafe" in one sense -- either your enumerable needs to 
    /// be finite, or maxCount needs to be sensibly small.</summary>
    public static List<T> ToSafeList<T>(this IEnumerable<T> enumerable, Func<T, bool> accept = null, int maxCount = int.MaxValue) {
      if (enumerable == null) {
        return new List<T>();
      } else {
        return enumerable.SafeWhere(accept).Take(maxCount).ToList();
      }
    }
    public static List<U> ToSafeList<T, U>(this IEnumerable<T> enumerable, Func<T, bool> accept = null, int maxCount = int.MaxValue)
    where T: U {
      IEnumerable<U> selectEnumerable = enumerable.Select<T, U>(t => t);
      List<U> r = selectEnumerable.ToSafeList(null, maxCount);
      return r;
    }
    /// <summary>Always returns a non-null enumerable. If accept is null, it is treated as always
    /// returning true.</summary>
    public static IEnumerable<T> SafeWhere<T>(this IEnumerable<T> enumerable, Func<T, bool> accept) {
      if (enumerable == null) {
        return Enumerable.Empty<T>();
      }
      if (accept == null) {
        return enumerable;
      } else {
        return enumerable.Where(accept);
      }
    }
    public static int IndexWithMaxOutput<T>(this IEnumerable<T> enumerable, Func<T, double> function) {
      int r = -1;
      double test = double.MinValue;
      int i = 0;
      foreach (T t in enumerable) {
        double output = function(t);
        if (output > test) {
          r = i;
          test = output;
        }
        i++;
      }
      return r;
    }
  }
}

