using System;
using Jockusch.Common.Debugging;
using System.Drawing;

namespace Jockusch.Common
{
  public static class NullableAdditions
  {
    public static R Construct<T, R>(this Nullable<T> nullable, Func<T, R> constructor)
      where T: struct
      where R: class {
      R r = null;
      if (nullable.HasValue) {
        T t = nullable.Value;
        r = constructor(t);
      }
      return r;
    }
  }
}

