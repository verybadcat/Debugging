using System;
using System.Drawing;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class ColorAdditions
  {
    public static bool IsGrayScale(this Color color)
    {
      bool r = color.R == color.B && color.B == color.G;
      return r;
    }
    public static Color ToSlightlyDifferentColor(this Color color) {
      byte red = color.R;
      if (red < 128) {
        red += 16;
      } else {
        red -= 48;
      }
      Color r = Color.FromArgb(color.A, red, color.G, color.B);
      return r;
    }
    public static Color SafeTextColorWithCheck(this Color? color) {
      CommonDebug.NullCheck(color);
      return color ?? LastResorts.TextColor;
    }
    public static Color SafeBackgroundColorWithCheck(this Color? color) {
      CommonDebug.NullCheck(color);
      return color ?? LastResorts.BackgroundColor;
    }
    public static bool IsDarkOrTransparent(this Color color) {
      bool r = (color.R + color.B + color.G < 300) || (color.A < 50);
      return r;
    }
    public static string ToStringTransparentEmpty(this Color color) {
      string r;
      if (color.A == 0) {
        r = "";
      } else {
        r = color.ToString();
      }
      return r;
    }
  }
}

