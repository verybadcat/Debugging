using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jockusch.Common.Debugging;

namespace Jockusch.Common {
  public static class AttributeKeys {
    public const string ACCESSIBILITY_LABEL_KEY = "AccessibilityLabel";
    public const string UNDERLINE_KEY = "Underline";
		public const string ERROR_MESSAGE_KEY = "ErrorMessage";
    public const string LOCATION_KEY = "Location";
    public const string MAX_WIDTH_KEY = "MaxWidth";
    public const string VERTICAL_ALIGNMENT_KEY = "VerticalAlignment";
    public const string HORIZONTAL_ALIGNMENT_KEY = "HorizontalAlignment";
    public const string DISPLAY_STRING_KEY = "Display";
    public const string CURSOR_LOGICAL_INDEX_KEY = "CursorLogicalIndex";
    public const string CURSOR_MOVE_KEY = "CursorMove";
    public const string ISSTATIC_KEY = "IsStatic";
    public const string FONT_KEY = "Font";
    public const string TEXT_COLOR_KEY = "TextColor";
    public const string GHOSTED_TEXT_KEY = "GhostedText";
    public const string BACKGROUND_COLOR_KEY = "BackgroundColor";
    public const string TAG_KEY = "Tag";
    public const string ISPIVOT_KEY = "IsPivot";
    public static GenericAttribute NewGenericAttribute(string key, string value) {
      GenericAttribute r = null;
      switch (key) {
				case AttributeKeys.ERROR_MESSAGE_KEY:
					r = new StringAttribute(value, true);
					break;
        case AttributeKeys.LOCATION_KEY:
          r = new PointFAttribute (value);
          break;
        case AttributeKeys.MAX_WIDTH_KEY:
          r = new FloatAttribute (value);
          break;
        case AttributeKeys.VERTICAL_ALIGNMENT_KEY:
          r = new VerticalAlignmentAttribute (value);
          break;
        case AttributeKeys.HORIZONTAL_ALIGNMENT_KEY:
          r = new HorizontalAlignmentAttribute (value);
          break;
        case AttributeKeys.DISPLAY_STRING_KEY:
          r = new StringAttribute (value, true);
          break;
        case AttributeKeys.ISPIVOT_KEY:
          r = new BooleanAttribute(value);
          break;
        case AttributeKeys.CURSOR_LOGICAL_INDEX_KEY:
          r = IntAttribute.FromString(value);
          break;
        case AttributeKeys.CURSOR_MOVE_KEY:
          r = IntAttribute.FromString(value);
          break;
        case AttributeKeys.ISSTATIC_KEY: 
          r = new BooleanAttribute (value);
          break;
      }
      if (r == null) {
        CommonDebug.UnexpectedInputBreakpoint();
      }
      return r;
    }
  }
}
