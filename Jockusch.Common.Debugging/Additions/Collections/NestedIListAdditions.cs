using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jockusch.Common
{
  public static class NestedIListAdditions
  {
    /// <summary>Given a nested list, returns the maxima of the *columns*.</summary>
    public static List<T> NestedColumnMax<T> (this List<List<T>> list, T minValue)
      where T: IComparable {
      List<T> r = new List<T>();
      foreach (List<T> row in list) {
        for (int i = 0; i < row.Count; i++) {
          if (i == r.Count) {
            r.Add(minValue);
          }
          bool bigger = row[i].CompareTo(r[i]) > 0;
          if (bigger) {
            r[i] = row[i];
          }
        }
      }
      return r;
    }
  }
}
