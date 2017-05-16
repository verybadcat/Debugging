using System;
using Jockusch.Common.Debugging;

namespace Jockusch.Common.Debugging
{
  public class BreakingException: Exception
  {
    public BreakingException()
    {
      CommonDebug.BreakPoint();
    }
    public BreakingException(string errorMessage): base(errorMessage) {
      CommonDebug.RecordOperation("Breaking exception", errorMessage);
      if (WJOperatingSystem.Current.IsTest()) {
        CommonDebug.BreakPoint();
      } else {
        CommonDebug.HardBreakPoint();
      }
    }
    public static void ThrowIf(bool condition) {
      if (condition) {
        throw new BreakingException();
      }
    }
  }
}
