using System;
using System.Drawing;

namespace Jockusch.Common
{
  public interface IAttributedString: IAttributeCollection
  {
    string Text { get; set; }
    string Tag { get; set;}
    Func<CommonFont> FontGetter {get;set;}
    CommonFont GetFont(bool forDrawing);
    Color GetTextColor();
    void SetTextColor(Color color);
    void SetDrawingFontSize(float pointSize);
    void DeepCopyIvarsFrom(IAttributedString updatedAttribText);
	}


}

