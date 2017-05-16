using System;
using System.Drawing;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class LastResorts
  {
    public static CommonFont Font {get;} = new CommonFont(Screen.MainScreen.ScreenCM(0.4f));
    public static HorizontalAlignmentEnum HorizontalAlignment {get;} = HorizontalAlignmentEnum.Center;
    public static VerticalAlignmentEnum VerticalAlignment {get;} = VerticalAlignmentEnum.Center;
    public static Color BackgroundColor {get;} = Color.Brown;
    public static Color TextColor {get;} = Color.HotPink;
    public static ThicknessF Padding {get;} = new ThicknessF();
  }
}

