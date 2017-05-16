using System;
using System.Drawing;
using RectangleClass = System.Drawing.Rectangle;
using System.Collections.Generic;
using System.Linq;

namespace Jockusch.Common
{
  public static class RectangleAdditions
  {
    public static String ToShortString (this Rectangle rect) {
      string r;
      if (rect.IsNaN()) {
        r = EncodeNaNRectangle;
      } else {
        r = rect.X + " " + rect.Y + " " + rect.Width + " " + rect.Height;
      }
      return r;
    }
    public static RectangleClass FromShortString(string s) {
      RectangleClass r = RectangleAdditions.NaN;
      if (s != null) {
        string[] entries = s.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        int min = int.MinValue;
        if (entries.Count() == 4) {
          int entry0 = IntAdditions.TryParse(entries[0], min);
          int entry1 = IntAdditions.TryParse(entries[1], min);
          int entry2 = IntAdditions.TryParse(entries[2], min);
          int entry3 = IntAdditions.TryParse(entries[3], min);
          if (entry0 != min && entry1 != min && entry2 != min && entry3 != min) {
            r = new RectangleClass(entry0, entry1, entry2, entry3);
          }
        }
      }
      return r;
    }
    public static RectangleF ToRectangleF(this RectangleClass rect) {
      return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
    }
    public const string EncodeNaNRectangle = "NaN Rectangle";
    /// <summary>Since int doesn't have NaN, we use int.MinValue.</summary> 
    public static RectangleClass NaN {
      get {
        return new RectangleClass(int.MinValue, int.MinValue, int.MinValue, int.MinValue);
      }
    }
    public static bool IsNaN(this RectangleClass rect) {
      int pseudoNaN = int.MinValue;
      if (rect.X == pseudoNaN || rect.Y == pseudoNaN || rect.Width == pseudoNaN || rect.Height == pseudoNaN) {
        return true;
      } else {
        return false;
      }
    }
  }
}
