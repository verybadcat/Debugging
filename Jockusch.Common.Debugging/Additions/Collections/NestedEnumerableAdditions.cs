using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Jockusch.Common
{
  public static class NestedEnumerableAdditions
  {
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> nestedList) {
      foreach (IEnumerable<T> enumerable in nestedList) {
        foreach (T obj in enumerable) {
          yield return obj;
        }
      }
    }
    public static T ElementAt<T>(this IEnumerable<IEnumerable<T>> nestedList, Point rowAndColumn) {
      int x = rowAndColumn.X;
      int y = rowAndColumn.Y;
      IEnumerable<T> row = nestedList.ElementAt(x);
      T r = row.ElementAt(y);
      return r;
    }
    public static T ElementAtOrDefault<T>(this IEnumerable<IEnumerable<T>> nestedList, Point rowAndColumn) {
      int x = rowAndColumn.X;
      int y = rowAndColumn.Y;
      IEnumerable<T> row = nestedList.ElementAtOrDefault(x);
      T r = row.ElementAtOrDefault(y);
      return r;
    }
    public static IEnumerable<U> Apply<T, U>(this IEnumerable<T> source, Func<T, U> function, Predicate<U> failure = null) {
      List<U> r = null;
      if (source != null) {
        r = new List<U>();
        if (failure == null) {
          failure = (u => false);
        }
        foreach (T t in source) {
          U ft = function(t);
          if (failure(ft)) {
            r = null;
            break;
          } else {
            r.Add(ft);
          }
        }
      }
      return r;
    }
    public static IEnumerable<IEnumerable<U>> NestedApply<T, U>(this IEnumerable<IEnumerable<T>> source, Func<T, U> function, Predicate<U> failure = null) {
      Func<IEnumerable<T>, IEnumerable<U>> inner = (et => NestedEnumerableAdditions.Apply(et, function, failure));
      IEnumerable<IEnumerable<U>> r = NestedEnumerableAdditions.Apply(source, inner, list => list == null);
      return r;
    }
    public static void NestedApply<T>(this IEnumerable<IEnumerable<T>> nestedEnum, Action<T> action) {
      foreach(IEnumerable<T> enumerable in nestedEnum) {
        foreach(T obj in enumerable) {
          action(obj);
        }
      }
    }
    public static bool NestedEqualsEnumerable<T>(this IEnumerable<IEnumerable<T>> nestedEnum, IEnumerable<IEnumerable<T>> otherNestedEnum, 
      Func<T, T, bool> equalityTester) {
      Func<IEnumerable<T>, IEnumerable<T>, bool> innerTester = (e1, e2) => e1.EqualsEnumerable(e2, equalityTester);
      bool r = nestedEnum.EqualsEnumerable(otherNestedEnum, innerTester);
      return r;
    }
    public static List<List<T>> ToNestedList<T>(this IEnumerable<IEnumerable<T>> nestedEnum) {
      List<List<T>> r = null;
      if (nestedEnum != null) {
        r = nestedEnum.Select(e => e?.ToList())?.ToList();
      }
      return r;
    }
    public static T NestedFirst<T>(this IEnumerable<IEnumerable<T>> enumerable, Predicate<T> condition,
                                   out int iRow, out int iColumn) {
      iRow = 0;
      foreach(IEnumerable<T> row in enumerable) {
        iColumn = 0;
        foreach(T t in row) {
          if (condition(t)) {
            return t;
          }
          iColumn++;
        }
        iRow++;
      }
      iRow = -1;
      iColumn = -1;
      return default(T);
    }
    /// <summary>In the case of a jagged input enumerable, where later rows are longer than
    /// earlier ones, we need a way to get new T objects. That's what getFiller is for.</summary>
    public static List<List<T>> ListTranspose<T>(this IEnumerable<IEnumerable<T>> inputEnumerable, Func<T> getFiller) {
      List<List<T>> r = new List<List<T>>();
      int iRow = 0;
      foreach(IEnumerable<T> row in inputEnumerable) {
        int iCol = 0;
        foreach (T obj in row) {
          if (iCol >= r.Count) {
            r.Add(new List<T>());
          }
          List<T> rRow = r[iCol];
          while (rRow.Count<iRow) {
            T newObj = getFiller();
            rRow.Add(newObj);
          }
          rRow.Add(obj);
          iCol++;
        }
        iRow++;
      }
      return r;
    }
    /// <summary>Parens defaults to ParenTypes.None.</summary>
    public static string ToNestedCommaSeparatedList<T>(this IEnumerable<IEnumerable<T>> nestedEnumerable,
      Func<T, string> convertEntry, string separator = ",", ParenType parens = null) {
      IEnumerable<string> rows = nestedEnumerable.Select(e => e.ToCommaSeparatedList(convertEntry, separator, parens));
      string r = rows.ToCommaSeparatedList(s => s, separator, parens);
      return r;
    }
  }
}
