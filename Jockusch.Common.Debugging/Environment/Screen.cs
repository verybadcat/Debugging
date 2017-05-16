using System;
using Jockusch.Common.Debugging;
using System.Drawing;

namespace Jockusch.Common
{
  public abstract class Screen
  {
    /// <summary> screen is used to size certain UI elements. If there are likely to be multiple windows on the screen (such as a desktop),
    /// we make certain things smaller.</summary>
    public abstract bool LikelyToHaveMultipleWindows { get; } 

    private static Screen _MainScreen;
    public Screen()
    {
    }
    public virtual bool IsTest()
    {
      return false;
    }
    /// <summary>Set will typically only be called once, during startup. Set using SetMainScreen().</summary>
    public static Screen MainScreen {
      get {
        if (_MainScreen == null) {
          // Non-test environments should set MainScreen.  Need to figure out how to enforce this.
          _MainScreen = new TestScreen();
        }
        return _MainScreen;
      }
      private set {
        _MainScreen = value;
      }
    }
    public static void SetMainScreen(Screen screen) {
      Screen.MainScreen = screen;
    }
    /// <summary>
    /// Gets approximately one inch, in the metrics used by the particular OS.  Some OS do not have access to this
    /// information; they should guess.
    /// </summary>
    /// <value>The screen inch.</value>
    public abstract float ScreenInch { get; }

    public float ScreenCM {
      get {
        float inch = this.ScreenInch;
        float r = inch / 2.54f;
        CommonDebug.LogColoredOnce(4, "ScreenCM", r);
        return r;
      }
    }
    /// <summary> pixels per unit of measurement.  Corresponds to iOS Screen.MainScreen.Scale.</summary>
    public virtual float Scale {
      get {
        return 1;
      }
    }
    /// <summary> Does not actually have to be pixels.  iOS uses iOS points, for example.
    /// The only requirement is that a particular front end has to use it consistently.</summary>
    public abstract Size PixelSize { get; }
    public SizeF InchesSize {
      get {
        Size pixels = this.PixelSize;
        SizeF pixelsFloat = new SizeF (pixels.Width, pixels.Height);
        float ratio = 1 / (float)this.ScreenInch;
        SizeF r = pixelsFloat.Times (ratio);
        return r;
      }
    }

    public virtual Rectangle PixelFrame {
      get {
        Size size = this.PixelSize;
        Rectangle r = new Rectangle(0, 0, size.Width, size.Height);
        return r;
      }
    }
     

    public SizeF CMSize {
      get {
        SizeF inches = this.InchesSize;
        float ratio = 2.54f;
        SizeF r = inches.Times (ratio);
        return r;
      }
    }


    public float InchesDiagonal {
      get {
        SizeF inchesSize = this.InchesSize;
        float r = (float)Math.Sqrt (inchesSize.Width *inchesSize.Width + inchesSize.Height *inchesSize.Height);
        return r;
      }
    }
  }

  public static class ScreenAdditions
  {
    public static float ScreenInches(this Screen screen, float inches, bool rounded = true) {
      float inch = screen.ScreenInch;
      float r = inch * inches;
      if (rounded) {
        r = (float)(Math.Round(r));
      }
      return r;
    }
    public static float ScreenCM (this Screen screen, float cm, bool rounded = true) {
      float inches = cm / 2.54f;
      float r = screen.ScreenInches (inches, rounded);
      return r;
    }
    public static float ScreenCM (this Screen screen, float cm, int minPixels) {
      float r = screen.ScreenCM(cm);
      return Math.Max(r, minPixels);
    }
    public static float ScreenMM (this Screen screen, float mm, bool rounded = true) {
      float inches = mm / 25.4f;
      float r = screen.ScreenInches(inches, rounded);
      return r;
    }
    public static float MinSideInches(this Screen screen) {
      SizeF size = screen.InchesSize;
      float r = size.MinSide();
      return r;
    }
    public static double GeometricMeanMinSideAndInch (this Screen screen) {
      float minSide = screen.MinSideInches();
      float inch = screen.ScreenInch;
      double sqrtSide = Math.Sqrt(minSide);
      double r = inch * sqrtSide;
      return r;
    }
  }
}

