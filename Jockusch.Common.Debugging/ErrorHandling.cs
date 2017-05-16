using System;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public enum ErrorHandling
  {
    None,
    BreakPoint,
    Exception,
  }
  public static class ErrorHandlingAdditions {
    public static void HandleError(this ErrorHandling handling) {
      switch(handling) {
        case ErrorHandling.BreakPoint:
          CommonDebug.BreakPoint();
          break;
        case ErrorHandling.Exception:
          throw new BreakingException();
        case ErrorHandling.None:
          break;
        default:
          CommonDebug.BreakPoint("Unknown error handling");
          break;
      }
    }
    public static void HandleException(this ErrorHandling handling, Exception e) {
      switch (handling) {
      case ErrorHandling.BreakPoint:
        e.LogAndBreak();
        break;
      case ErrorHandling.None:
        break;
      case ErrorHandling.Exception:
      default:
        throw e; // not actually ideal; would prefer just a "throw" here.
      }
    }
  }
}

