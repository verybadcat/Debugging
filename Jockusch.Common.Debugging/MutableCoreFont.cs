using System;
using Jockusch.Common;
using System.Drawing;
using Jockusch.Common.Debugging;

namespace Jockusch.Common {
  public class MutableCoreFont: CommonFont {
    public MutableCoreFont() : this(-1) {
      
    }
    public MutableCoreFont(
      float pointSize, bool bold = false, bool italic = false) : base(pointSize, bold, italic){
      
    }
    public MutableCoreFont(CommonFont otherFont) {
      this.DeepCopyIvarsFrom(otherFont);
    }
    public void SetBold(bool value) {
      this._Bold = value;
    }
    public void SetItalic(bool value) {
      this._Italic = value;
    }
    public void SetPointSize(float value) {
      this._PointSize = value;
    }
  }
}