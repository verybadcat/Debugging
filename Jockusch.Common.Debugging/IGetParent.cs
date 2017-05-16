using System;
using System.Collections.Generic;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public interface IGetParent<TParent>
  {
    /// <summary>In general, returning null is perfectly acceptable.</summary>
    TParent GetParent();
  }
  public static class IGetParentAdditions {
    public static IEnumerable<T> Ancestors<T, U>(this T t)
      where T: IGetParent<U> {
      Func<T, U> parentGetter = tt => tt.GetParent();
      return t.Ancestors(parentGetter);
    }
    public static IEnumerable<T> Ancestors<T, U>(this T t, Func<T, U> parentGetter) {
      T r = t;
      while(r!=null) {
        yield return r;
        U parent = parentGetter(r);
        if (AnyType.DoesEqual<object>(parent, r)) {
          CommonDebug.BreakPoint();
          break;
        } else {
          if (parent is T) {
            r = (T)(object)parent;
          } else {
            break;
          }
        }
      }
    }
    public static T FindAncestor<T> (this T t, Predicate<T> condition, ErrorHandling errorHandling)
      where T: IGetParent<T> {
      T r = t;
      while (r !=null) {
        if (condition(r)) {
          break;
        } else {
          r = r.GetParent();
        }
      }
      if (r == null) {
        errorHandling.HandleError();
      }
      return r;
    }
    public static R FindAncestor<T, U, R>(this T t, ErrorHandling errorHandling = ErrorHandling.BreakPoint)
      where T: IGetParent<U>
      where R: U {
      if (t is R) {
        return (R)(object)t;
      } else {
        while (t!=null) {
          U parent = t.GetParent();
          if (AnyType.DoesEqual<object>(t, parent)) {
            break;
          }
          if (parent is R) {
            return (R)parent;
          } else {
            if (parent is T) {
              t = (T)(object)parent;
            } else {
              break;
            }
          }
        }
      }
      errorHandling.HandleError();
      return default(R);
    }
  }

}

