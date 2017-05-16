using System;
using System.Collections.Generic;
using System.Linq;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class ListAdditions {
    public static void BecomeSingleton<T>(this List<T> list, T content) {
      int count = list.Count;
      switch (count) {
        case 0:
          list.Add(content);
          break;
        case 1:
          if (AnyType.DoesNotEqual(list[0], content)) {
            list[0] = content;
          }
          break;
        default:
          list.Clear();
          list.Add(content);
          break;
      }
    }
    public static List<string> IntegerStringList(int min, int max) {
      List<string> r = new List<string> ();
      for (int i = min; i <= max; i++) {
        r.Add (i.ToString ());
      }
      return r;
    }
    /// <summary>Safe to call on any enumerable; even null (in which case returns false).
    /// The only exception is if the enumerable is non-null, but its count method fails
    /// to return a value.</summary>
    public static bool ContainsIndex<T>(this IEnumerable<T> enumerable, int index) {
      if (enumerable == null || index<0) {
        return false;
      }
      if (index >= enumerable.Count()) {
        return false;
      }
      return true;
    }
    public static void SetAtIndex<T>(this List<T> list, int index, T value, Func<T> getDefaultValue) {
      while (list.Count < index) {
        list.Add(getDefaultValue());
      }
      if (list.Count <= index) {
        list.Insert(index, value);
      } else {
        list[index] = value;
      }
    }
    public static T SafeEntry<T>(this IList<T> list, int index, T defaultValue = default(T)) {
      return list.SafeEntry(index, n => defaultValue);
    }
    public static T SafeEntry<T>(this IList<T> list, int index, Func<int, T> getDefaultValue) {
      if (list.ContainsIndex(index)) {
        return list[index];
      } else {
        return getDefaultValue(index);
      }
    }
    public static T SafeEntry<T>(this IList<T> list, int index, Func<T> getDefaultValue) {
      return list.SafeEntry(index, n => getDefaultValue());
    }
    public static int SafeIndexOf<T>(this IList<T> list, T entry) {
      if (list == null) {
        return -1;
      } else {
        return list.IndexOf(entry);
      }
    }
    public static int SafeIndexOf<T>(this IList<T> list, T entry, int failureValue) {
      int r = list.SafeIndexOf(entry);
      if (r == -1) {
        r = failureValue;
      }
      return r;
    }
    private static Random _Randomizer {get;set;}
    public static Random Randomizer {
      get {
        if (_Randomizer == null) {
          _Randomizer = new Random();
        }
        return _Randomizer;
      }
    }
		/// <summary> Adjusts the size of the list, by adding (if positive sign) or removing (if negative sign) the given
		/// numbers of elements at the beginning and end of the list.  It is the calling code's responsibility to make sure
		/// this is possible.  If elements need to be added, they are created by the factory.
		public static void AdjustSize<T>(this List<T> list, int nAddOrRemoveBeginning, int nAddOrRemoveEnd, Func<T> factory) {
			if (nAddOrRemoveBeginning > 0) {
				for (int i=0; i<nAddOrRemoveBeginning; i++) {
					T insertMe = factory();
					list.Insert(0, insertMe);
				}
			}
			if (nAddOrRemoveEnd > 0) {
				for (int i=0; i<nAddOrRemoveEnd; i++) {
					T addMe = factory();
					list.Add(addMe);
				}
			}
			if (nAddOrRemoveBeginning < 0) {
				int nRemove = -nAddOrRemoveBeginning;
				list.RemoveRange(0, nRemove);
			}
			if (nAddOrRemoveEnd < 0) {
				int nRemove = -nAddOrRemoveEnd;
				list.RemoveRange(list.Count - nRemove, nRemove);
			}
		}
    public static void AdjustSize<T>(this List<T> list, int targetSize, Func<T> creator) {
      int nAddOrRemove = targetSize - list.Count;
      list.AdjustSize(0, nAddOrRemove, creator);
    }
		public static void InsertReversed<T>(this List<T> list, int index, IEnumerable<T> enumerable) {
			// Enumerable.Reverse() returns a new enumerable.  By contrast, List.Reverse() reverses in place.
			IEnumerable<T> reversed = enumerable.Reverse();
			list.InsertRange(index, reversed);
		}
		public static void AddReversed<T>(this List<T> list, IEnumerable<T> enumerable) {
			IEnumerable<T> reversed = enumerable.Reverse();
			list.AddRange(reversed);
		}
    /// <summary>e.g. If the debugStrings of the entries are "foo" and "bar",
    /// will return "{foo, bar}"</summary>
    public static string ToBracedDebugString<T> (this List<T> list) {
      string r = "{";
      foreach (T entry in list) {
        if (entry is IDebugString) {
          r += (entry as IDebugString).ToDebugString();
        } else {
          r += entry.ToString();
        }
        r += ", ";
      }
      r = r.Substring(0, r.Length - 2);
      r += "}";
      return r;
    }

    public static string ToCommaSeparatedList<T>(this IEnumerable<T> This, Func<T, string> entryToString, string separator = ", ", ParenType parens = null) {
      if (parens == null) {
        parens = ParenTypes.None;
      }
      string r = parens.Open;
      bool first = true;
      foreach (T obj in This) {
        if (first) {
          first = false;
        } else {
          r += separator;
        }
        r += entryToString (obj);
      }
      r += parens.Close;
      return r;
    }

    /// <summary>Returns null if our string is not in fact a comma separated list with the given
    /// paren type.  If paren type is null, defaults to None.</summary>
    public static string[] FromCommaSeparatedList(this string list, ParenType parens = null) {
      parens = parens ?? ParenTypes.None;
      string open = parens.Open;
      string close = parens.Close;
      string[] r = null;
      if (list.StartsWithInvariant(open) && list.EndsWithInvariant(close)) {
        string rest = list.Substring(open.Length, list.Length - open.Length - close.Length);
        if (rest.Length == 0) {
          r = new List<string>().ToArray();
        } else {
          string[] separator = { "," };
          r = rest.Split(separator, StringSplitOptions.None);
        }
      }
      return r;
    }

    /// <summary>The end result is the same list as clearing the list, then adding all the entries.
    /// BUT we have cases where it is more desirable to instead go through the entries
    /// and the list, keeping anything that is still good, hence avoiding the remove-and-re-add.
    /// That's what we do. It only works
    /// if the list entries we want to keep are in the same order as the enumerable.
    /// Anything out of order will be removed and re-added.</summary>
    public static void UpdateEntries<T>(this IList<T> list, IEnumerable<T> entries) {
      List<T> entriesList = entries.ToList();
      int listIndex = 0;
      foreach(T t in entriesList) {
        while (listIndex < list.Count && AnyType.DoesNotEqual(list[listIndex], t)) {
          list.RemoveAt(listIndex);
        }
        if (listIndex < list.Count) {
          CommonDebug.Assert(AnyType.DoesEqual(list[listIndex], t));
        } else {
          list.Insert(listIndex, t);
        }
        listIndex++;
      }
      while (listIndex < list.Count) {
        list.RemoveAt(listIndex);
      }
    }

    public static IEnumerable<List<T>> ChopWhere<T>(this IEnumerable<T> enumerable, Func<T, T, bool> chop) {
      List<T> piece = new List<T>();
      bool first = true; // yes, we need this, even though we have previous.
      T previous = default(T); 
      foreach (T item in enumerable) {
        if (first) {
          first = false;
        } else {
          if (chop(item, previous)) {
            yield return piece;
            piece = new List<T>();
          }
        }
        piece.Add(item);
        previous = item;
      }
      if (!first) {
        yield return piece;
      }
    }

    public static T CreateOrUpdateEntry<T> (this IList<T> list, int index, Func<T, T> creatorAndUpdater) {
      int myCount = list.Count;
      T r;
      if (myCount < index) {
        r = default(T);
        CommonDebug.BreakPoint();
      } else if (myCount == index) {
        r = creatorAndUpdater(default(T));
        list.Add(r);
      } else {
        T entry = list[index];
        r = creatorAndUpdater(entry);
        if (!(ReferenceEquals(r, entry))) {
          list[index] = r;
        }
      }
      return r;
    }

    public static T CreateOrUpdateEntry<T> (this IList<T> list, int index, Func<T> creator, Action<T> updater = null) {
      int myCount = list.Count;
      T r = default(T);
      if (myCount < index) {
        CommonDebug.BreakPoint();
      } else if (myCount == index) {
        r = creator();
        list.Add(r);
      } else {
        r = list[index];
        if (updater != null) {
          updater(r);
        }
      }
      return r;
    }
    public static void SetAll<T>(this List<T> list, T value) {
      for (int i=0; i<list.Count; i++) {
        list[i] = value;
      }
    }
    /// <summary>Removes any trailing default entries.</summary>
    public static List<T> DecodeList<T>(List<string> encodedEntries, Func<string, T> decodeEntry, T defaultValue) {
      List<T> r = new List<T>();
      foreach (string entry in encodedEntries) {
        T decoded = decodeEntry(entry);
        r.Add(decoded);
        while (!r.IsEmptyEnumerable() && (AnyType.DoesEqual(r.Last(), defaultValue))) {
          r.RemoveAt(r.Count - 1);
        }
      }
      return r;
    }


    public static void Sort<T>(this List<T> list, Func<T, string> ordering) {
      list.Sort((t1, t2) => String.CompareOrdinal(ordering(t1), ordering(t2)));
    }
    public static void Sort<T>(this List<T> list, Func<T, int> ordering) {
      list.Sort((t1, t2) => ordering(t1).CompareTo(ordering(t2)));
    }
    public static void Sort<T>(this List<T> list, Func<T, double> ordering) {
      list.Sort((t1, t2) => ordering(t1).CompareTo(ordering(t2)));
    }
    public static void Add<T>(this List<T> list, T t1, T t2) {
      list.Add(t1);
      list.Add(t2);
    }
    public static void Add<T>(this List<T> list, T t1, T t2, T t3) {
      list.Add(t1);
      list.Add(t2);
      list.Add(t3);
    }
  }
}

