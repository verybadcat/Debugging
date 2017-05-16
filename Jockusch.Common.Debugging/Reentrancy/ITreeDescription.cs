using System;
using System.Linq;
using System.Collections.Generic;
using Jockusch.Common.Timers;

namespace Jockusch.Common.Debugging
{
  public interface ITreeDescription
  {
    string ToShortString();
    IEnumerable<object> GetTreeDescriptionChildren();
    bool RequiresMainThreadForTreeDescription();
  }

  public interface ITreeOwnerDescription
  {
    /// <summary> Tries to get the owner of the object. The owner must never return the original object from GetTreeDescriptionChildren(),
    /// or infinite recursion will result. But if you have an IElement, returning the INativeElement as its owner would allow
    /// ToStringTree() methods on the element to actually use the corresponding nativeElement.</summary>
    ITreeDescription TryGetOwner();
  }

  public static class ITreeDescriptionAdditions
  {
    public static string DescendantStringTreeWithPredicate(this ITreeDescription node, Predicate<ITreeDescription> stop = null, string prefix = "", bool tryOwner = true) {
      string r = "";
      if (node!=null) {
        IEnumerable<object> kids = node.GetTreeDescriptionChildren();

        string extendedPrefix = prefix + "  ";
        if (kids != null) {
          foreach (object item in kids) {
            ITreeDescription childNode = item as ITreeDescription;
            if (childNode != null) {
              string entryTree = childNode.ToStringTreeWithPredicate(stop, extendedPrefix, tryOwner);
              r += entryTree;
            } else {
              string debug = CommonDebug.GetDebugString(item, true);
              r += (extendedPrefix + debug + "\n");
            }
          }
        }
      }
      return r;
    }
    public static string ToShortString(this ITreeDescription node, string prefix = "") {
      string s = node?.ToShortString()??"null";
      string r = prefix + s;
      return r;
    }
    public static string ToStringTreeGeneric<T, U>(this object node, Func<T, string> nodeShortStringGetter, Func<T, IEnumerable<U>> childrenGetter) {
      Func<T, IEnumerable<object>> objEnum = t => childrenGetter(t).Select(u => (object)u);
      Func<T, string, string> expandedShortStringGetter = (t, prefix) => prefix + nodeShortStringGetter(t);
      string r = node.ToStringTreeGenericWithPredicate(t => false, "", expandedShortStringGetter, objEnum);
      return r;
    }
    public static string ToStringTreeGenericWithPredicate<T>(this object node, Predicate<T> stop,
                                                     string prefix,
                                                      Func<T, string, string> nodeShortStringGetter,
                                                      Func<T, IEnumerable<object>> childrenGetter) {
      string r;
      if (node is T) {
        T t = (T)node;
        r = nodeShortStringGetter(t, prefix) + "\n";
        string extendedPrefix = prefix + "  ";
        if (!stop(t)) {
          foreach (object child in childrenGetter(t)) {
            r += child.ToStringTreeGenericWithPredicate(stop, extendedPrefix, nodeShortStringGetter, childrenGetter);
          }
        }
      } else {
        r = CommonDebug.CombineDebugString(prefix, node);
      }
      return r;
    }
    /// <summary>Will not recurse if the predicate evaluates to true.Sty
    /// Incidentally, this has to be named differently from the string version, or it won't know what to do
    /// if the argument is null.</summary>
    public static string ToStringTreeWithPredicate(this ITreeDescription node, Predicate<ITreeDescription> stop = null,
                                                   string prefix = "", bool tryOwner = true, 
                                                   Func<ITreeDescription, string, string> nodeShortStringGetter = null) {
      string r = null;
      if (nodeShortStringGetter == null) {
        nodeShortStringGetter = ((n, s) => n.ToShortString(s));
      }
      if (tryOwner && node is ITreeOwnerDescription) {
        ITreeDescription owner = (node as ITreeOwnerDescription).TryGetOwner();
        if (owner != null) {
            r = owner.ToStringTreeWithPredicate(stop, prefix, tryOwner);
        }
      }
      if (r == null) {
        string shortString = node.ToShortString(prefix);
        r = shortString + StringConstants.Newline;
        if (stop == null || stop(node) == false) {
          string kids = node.DescendantStringTreeWithPredicate(stop, prefix, tryOwner);
          r += kids;
        }
      }
      return r;
    }

    public static string ToStringTree(this ITreeDescription node, string stop = null, string prefix = "", bool tryOwner = true) {
      Predicate<ITreeDescription> predicate = n => (stop != null && n.ToShortString().IndexOfInvariant(stop) != -1);
      string r = node.ToStringTreeWithPredicate(predicate, "", tryOwner);
      return r;
    }



    public static string ToShallowStringTree(this ITreeDescription node, string prefix = "", bool tryOwner = true) {
      return node.ToStringTreeWithPredicate(n => node != n, prefix, tryOwner);
    }

    public static void LogStringTree(this ITreeDescription node, string stop = null, string prefix = "", bool tryOwner = true) {
      string tree = node.ToStringTree(stop, prefix, tryOwner);
      CommonDebug.LogLine(tree);
    }
    public static void LogStringTreeSansOwner(this ITreeDescription node, string stop = null) {
      node.LogStringTree(stop, "", false);
    }
    private static void OnMainThreadIfPossible(this ITreeDescription node, Action action) {
      action ();
      // You will want to hook up to main thread info for your environment
    }
    public static void DebugLog(this ITreeDescription node, string recursionStop, List<int> delays) {
      delays.Sort();
      if (delays.Count == 0) {
        delays.Insert(0, 0);
      }
      if (node != null) {
        int first = delays[0];
        if (first == 0) {
          node.OnMainThreadIfPossible(() => node.LogStringTree(recursionStop));
          delays.RemoveAt(0);
        }
        if (delays.Count > 0) {
          int newFirst = delays[0];
          delays = delays.Select(n => n - newFirst).ToList();
          Timer timer = new Timer(_ => node.DebugLog(recursionStop, delays), null, 1000 * newFirst);
          CommonDebug.Timers.Add(timer);
        }
      }
    }
    public static void DebugLog(this ITreeDescription node, string recursionStop, params int[] delays) {
      node?.DebugLog(recursionStop, delays.ToList());
    }
    public static void DebugLog(this ITreeDescription node, params int[] delays) {
      node?.DebugLog(null, delays);
    }
  }
}
