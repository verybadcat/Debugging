using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Jockusch.Common.Debugging;

namespace Jockusch.Common {
  public struct ThicknessF {
    public float Left { get; set; }
    public float Top { get; set; }
    public float Right { get; set; }
    public float Bottom { get; set; }
    public ThicknessF(float left, float top, float right, float bottom): this() {
      this.Left = left;
      this.Top = top;
      this.Right = right;
      this.Bottom = bottom;
    }
    public ThicknessF(float allEdges): this (allEdges, allEdges, allEdges, allEdges) {
    }
    public ThicknessF(float side, float topBot): this(side, topBot, side, topBot) {
    }
    public static ThicknessF NaN() {
      return new ThicknessF(float.NaN);
    }
    public override string ToString () {
      return string.Format ("[ThicknessF: Left={0}, Top={1}, Right={2}, Bottom={3}]", Left, Top, Right, Bottom);
    }
    public string ToShortString () {
      int intLeft = (int)Left;
      int intTop = (int)Top;
      int intRight = (int)Right;
      int intBottom = (int)Bottom;
      string r = intLeft.ToString() + " " + intTop.ToString() + " " + intRight.ToString() + " " + intBottom.ToString();
      return r;
    }
    public ThicknessF Plus (ThicknessF otherThickness) {
      ThicknessF r = new ThicknessF (
        this.Left + otherThickness.Left,
        this.Top + otherThickness.Top,
        this.Right + otherThickness.Right,
        this.Bottom + otherThickness.Bottom
      );
      return r;
    }
    public ThicknessF Minus (ThicknessF otherThickness) {
      ThicknessF r = new ThicknessF (
        this.Left - otherThickness.Left,
        this.Top - otherThickness.Top,
        this.Right - otherThickness.Right,
        this.Bottom - otherThickness.Bottom
      );
      return r;
    }
 
    public ThicknessF Times (float scalar, bool round = false) {
      ThicknessF r = new ThicknessF (
        (this.Left * scalar),
        this.Top * scalar,
        this.Right * scalar,
        this.Bottom * scalar);
      if (round) {
        r = r.RoundedThickness;
      }
      return r;
    }
    public ThicknessF DividedBy (float scalar) {
      if (scalar == 0) {
        CommonDebug.BreakPoint();
        throw new DivideByZeroException();
      }
      ThicknessF r = new ThicknessF (
        this.Left / scalar,
        this.Top / scalar,
        this.Right / scalar,
        this.Bottom / scalar);
      return r;
    }
    public static ThicknessF operator + (ThicknessF thickness1, ThicknessF thickness2) {
      ThicknessF r = thickness1.Plus(thickness2);
      return r;
    }
    public static ThicknessF operator - (ThicknessF thickness1, ThicknessF thickness2) {
      ThicknessF r = thickness1.Minus(thickness2);
      return r;
    }
    public static ThicknessF operator * (ThicknessF thickness, float scalar) {
      ThicknessF r = thickness.Times(scalar);
      return r;
    }
    public static ThicknessF operator - (ThicknessF thickness) {
      ThicknessF r = thickness*-1f;
      return r;
    }
    public static ThicknessF operator / (ThicknessF thickness, float scalar) {
      ThicknessF r = thickness.DividedBy(scalar);
      return r;
    }
    public ThicknessF RoundedThickness {
      get {
        float left = this.Left;
        float top = this.Top;
        float right = this.Right;
        float bottom = this.Bottom;
        float rLeft = left.RoundToFloat();
        float rTop = top.RoundToFloat();
        float rRight = right.RoundToFloat();
        float rBottom = bottom.RoundToFloat();
        ThicknessF r = new ThicknessF(rLeft, rTop, rRight, rBottom);
        return r;
      }
    }
    /// <summary> The constant of proportionality is the geometric mean of the minimum screen dimension, measured in pixels, and the screen pixels per inch.</summary>
    public static ThicknessF GeometricMeanScreenAndInchProportionalThickness (ThicknessF thickness) {
      float factor = (float)Screen.MainScreen.GeometricMeanMinSideAndInch();
      ThicknessF r = thickness.MultiplyAndRound(factor);
      return r;
    }
    /// <summary>
    /// Input thickness is measured in inches.  Output thickness is measured in device metrics.
    /// </summary>
    public static ThicknessF ProportionalThicknessIN (ThicknessF thickness) {
      float inch = Screen.MainScreen.ScreenInch;
      ThicknessF unRounded = thickness * inch;
      ThicknessF r = unRounded.RoundedThickness;
      return r;
    }
    /// <summary>
    /// Left, top, right, and bottom are measured in inches.  Output is in device metrics.
    /// </summary>
    public static ThicknessF ProportionalThicknessIN (float left, float top, float right, float bottom) {
      ThicknessF unScaled = new ThicknessF(left, top, right, bottom);
      ThicknessF r = ThicknessF.ProportionalThicknessIN(unScaled);
      return r;
    }
		public static ThicknessF ProportionalThicknessIN(float sides, float aboveBelow) {
			ThicknessF r = ThicknessF.ProportionalThicknessIN(sides, aboveBelow, sides, aboveBelow);
			return r;
		}
    public static ThicknessF ProportionalThicknessMM (ThicknessF unscaledThickness) {
      return ProportionalThicknessCM(unscaledThickness.DividedBy(10));
    }
    public static ThicknessF ProportionalThicknessMM(float sides, float aboveBelow) {
      return ProportionalThicknessCM(sides/10, aboveBelow/10);
    }
    public static ThicknessF ProportionalThicknessMM(float left, float top, float right, float bottom) {
      return ProportionalThicknessCM(left / 10, top / 10, right / 10, bottom / 10);
    }
    public static ThicknessF ProportionalThicknessMM(float allEdges) {
      return ProportionalThicknessMM(allEdges, allEdges);
    }
    public static ThicknessF ProportionalThicknessCM (ThicknessF unscaledThickness) {
      ThicknessF inches = unscaledThickness.DividedBy(2.54f);
      ThicknessF r = ThicknessF.ProportionalThicknessIN(inches);
      return r;
    }
    public static ThicknessF ProportionalThicknessCM (float allEdges) {
      ThicknessF r = ThicknessF.ProportionalThicknessCM (allEdges, allEdges, allEdges, allEdges);
      return r;
    }
    public static ThicknessF ProportionalThicknessCM (float left, float top, float right, float bottom) {
      ThicknessF unscaled = new ThicknessF(left, top, right, bottom);
      ThicknessF r = ThicknessF.ProportionalThicknessCM(unscaled);
      return r;
    }
		public static ThicknessF ProportionalThicknessCM (float sides, float aboveBelow) {
			ThicknessF r = ThicknessF.ProportionalThicknessCM(sides, aboveBelow, sides, aboveBelow);
			return r;
		}
    public static ThicknessF Zero {get;} = new ThicknessF();
  }
  public static class ThicknessFAdditions {
    public static float LinearWidth(this ThicknessF thickness) {
      return thickness.Left + thickness.Right;
    }
    public static float LinearHeight(this ThicknessF thickness) {
      return thickness.Top + thickness.Bottom;
    }
    public static SizeF LinearSize(this ThicknessF thickness) {
      float width = thickness.LinearWidth ();
      float height = thickness.LinearHeight ();
      SizeF r = new SizeF (width, height);
      return r;
    }
		public static SizeF SizeF(this ThicknessF thickness) {
			float width = (float)thickness.LinearWidth();
			float height = (float)thickness.LinearHeight();
			SizeF r = new SizeF (width, height);
			return r;
		}
    /// <summary>Multiplies by the float, then rounds everything to the nearest integer</summary>
    public static ThicknessF MultiplyAndRound(this ThicknessF thickness, float factor) {
      ThicknessF r = thickness * factor;
      return r.RoundedThickness;
    }
    public static bool IsNaN(this ThicknessF thickness) {
      bool r = float.IsNaN(thickness.Left) || float.IsNaN(thickness.Top) || float.IsNaN(thickness.Right) || float.IsNaN(thickness.Bottom);
      return r;
    }
  }
}
