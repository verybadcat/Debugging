using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jockusch.Common.Debugging
{
  public static class TaskContinuations
  {
    public static Action<Task> AppendExceptionsToDebugString() {
      return task =>
      {
        if (task.IsFaulted) {
          Exception e = task.Exception;
          string debug = e.EverythingDebugString();
          if (CommonDebug.DebugString.IsNullOrEmpty()) {
            CommonDebug.DebugString = debug;
          }
        }
      };
    }
  }
}
