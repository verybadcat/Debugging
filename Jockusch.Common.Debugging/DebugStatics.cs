using System;
using System.Collections.Generic;
using System.Drawing;

namespace Jockusch.Common
{
  public class DebugStatics
  {
    public static List<Color> DebugColors = new List<Color>() {
      Color.FromArgb(255, 255, 43, 0), //red
      Color.FromArgb(255, 0, 140, 255), //blue with some green
      Color.FromArgb(255, 31, 144, 24),  // green
      Color.FromArgb(255, 157, 64, 171), // bright purple
      Color.FromArgb(255, 255, 127, 0),
      Color.FromArgb(255, 255, 20, 127),
      Color.FromArgb(255, 153, 102, 51),
      Color.FromArgb(255, 0, 230, 230),
      Color.FromArgb(255, 127, 127, 0),
      Color.FromArgb(255, 127, 0, 127),
    };

    public static Color DebugColor(int index) {
      int modIndex = index % DebugColors.Count;
      if (index < 0) {
        return Color.Black;
      } else {
        return DebugColors[modIndex];
      }
    }
  }
}

