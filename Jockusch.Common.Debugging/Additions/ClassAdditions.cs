using System;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class ClassAdditions
  {
    public static T NotNull<T> (this T obj, T backup) where T: class {
      CommonDebug.NullCheck(obj);
      T r = obj ?? backup;
      return r;
    }
  }
}

