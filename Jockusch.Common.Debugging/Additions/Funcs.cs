using System;

namespace Jockusch.Common
{
  public static class Funcs
  {
    public static Func<T1, T2> Default<T1, T2>() {
      return (T1 t1) => default(T2);
    }


  }
}

