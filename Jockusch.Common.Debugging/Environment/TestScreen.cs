using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jockusch.Common
{
  public class TestScreen: Screen
  {
    public TestScreen():this(new Size(750, 1200)) {
      
    }
    public override bool LikelyToHaveMultipleWindows {
      get {
        return false;
      }
    }
    public TestScreen(Size pixelSize)
    {
      this.PixelSizeStore = pixelSize;
    }
    public override float ScreenInch {
      get { return 150f; }
    }
        
    public override bool IsTest() {
      return true;
    }
    public Size PixelSizeStore { get; set; }

    public override Size PixelSize {
      get {
        return this.PixelSizeStore;
      }
    }
  }
}
