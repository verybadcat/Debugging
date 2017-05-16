using System;
using System.Diagnostics;

namespace Jockusch.Common.Debugging {
  public class CommonDebugException: Exception {
    public CommonDebugException () {
    }
    public CommonDebugException(string errorMessage): base(errorMessage) {
    }
  }
}

