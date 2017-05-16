using System;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public class UnknownOperatingSystem: WJOperatingSystem
  {
    public UnknownOperatingSystem()
    {
    }


    public override void SlightDelay(Action a, IGetNativeObject helper)
    {
      throw new BreakingException();
    }


    public override Screen CreateScreen(object platformContext) {
      throw new BreakingException(); // probably implement something
    }
  }
}

