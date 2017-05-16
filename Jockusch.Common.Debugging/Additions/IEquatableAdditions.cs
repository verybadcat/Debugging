using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jockusch.Common
{
  public static class IEquatableAdditions
  {
    public static bool OperatorEqualsImplementation<T>(this IEquatable<T> first, IEquatable<T> second)
    {
      bool r = false;
      if (null == first && null == second)
      {
        r = true;
      }
      else if (null != first && null != second)
      {
        r = first.Equals(second);
      }
      return r;
    }
    public static bool EqualsObjectImplementation<T>(this T equatable, object obj)
      where T : IEquatable<T>
    {
      bool r = false;
      if (obj is T)
      {
        r = equatable.Equals((T)obj);
      }
      return r;
    }
  }
}
