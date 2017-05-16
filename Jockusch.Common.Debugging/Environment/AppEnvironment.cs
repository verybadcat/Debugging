using System;
using Jockusch.Common.Debugging;
using System.Drawing;
using System.Threading.Tasks;

namespace Jockusch.Common
{
  public enum ScreenType
  {
    Phone,
    SmallTablet,
    BigTablet
  };

  public enum TabBarTabType
  {
    TextLeftOfIcon,
    IconOnly
  };

  public static class AppEnvironment
  {
    public static bool HaveRunPreStartup;
    public static void PreStartup(WJOperatingSystem os, object platformContext) {
      AppEnvironment.OperatingSystem = os;
      Screen screen = os.CreateScreen(platformContext);
      Screen.SetMainScreen(screen);
      AppEnvironment.HaveRunPreStartup = true;
    }




    /// <summary>If more than one context push or pop request comes from the same sender within this duration, 
    /// all but the first may be ignored.</summary>
    public static int RepeatNavigationMilliseconds = 1000;
    private static WJOperatingSystem _OperatingSystem { get; set; }
    public static bool HasOperatingSystem() {
      return (AppEnvironment._OperatingSystem != null);
    }
    /// <summary>Won't whine if the OS is null, unlike the property, which will whine.</summary>
    public static WJOperatingSystem OperatingSystemNullOk() {
      return _OperatingSystem;
    }
    public static WJOperatingSystem OperatingSystem {
      get {
        if (_OperatingSystem == null) {
          CommonDebug.ShouldNeverHappenBreakpoint(); // need to set the OS before getting it
          _OperatingSystem = new UnknownOperatingSystem();
        }
        return _OperatingSystem;
      }
      set {
        _OperatingSystem = value;
      }
    }
    public const bool IsDebug =
#if DEBUG
      true;
#else
      false;
#endif

    public static int MaxDisplayHistorySize {
      get {
        return 25;
      }
    }
    public static ScreenType GetScreenType() {
      Screen screen = Screen.MainScreen;
      SizeF size = screen.InchesSize;
      float min = size.MinSide();
      ScreenType r;
      if (min > 5f) {
        r = ScreenType.BigTablet;
      } else if (min > 3.2f) {
        r = ScreenType.SmallTablet;
      } else {
        r = ScreenType.Phone;
      }
      return r;
    }
  }
}
