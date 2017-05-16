using System;
using System.Drawing;
namespace Jockusch.Common
{
  public class DebugTags
  {
    public const string Startup = "Startup";
    public const string GlobalCache = "GlobalCache";
  }
  public static class DebugTagColors {
    public static int GetColorIndex(string tag) {
      switch (tag) {
        case DebugTags.Startup:
          return 2;
        default:
          return -1;
      }
    }
    public static Color GetColor(string tag) {
      int index = DebugTagColors.GetColorIndex(tag);
      Color r = DebugStatics.DebugColor(index);
      return r;
    }
  }
}

