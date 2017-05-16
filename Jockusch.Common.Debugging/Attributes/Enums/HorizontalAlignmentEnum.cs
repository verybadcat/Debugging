using System;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public enum HorizontalAlignmentEnum
  {
    Left,
    Center,
    Right
  };

  public static class HorizontalAlignmentAdditions
  {
    /// <summary>
    /// left = 0; center = 0.5; right=1
    /// </summary>
    public static float Multiplier(this HorizontalAlignmentEnum alignment) {
      float r;
      switch (alignment) {
        case HorizontalAlignmentEnum.Left:
          r = 0;
          break;
        case HorizontalAlignmentEnum.Center:
          r = 0.5f;
          break;
        case HorizontalAlignmentEnum.Right:
          r = 1f;
          break;
        default:
          CommonDebug.BreakPointLog("Unknown alignment");
          r = 0f;
          break;
      }
      return r;
    }
    public static HorizontalAlignmentEnum? FromString(string s) {
      foreach (HorizontalAlignmentEnum alignment in Enum.GetValues(typeof(HorizontalAlignmentEnum))) {
        if (s.ToLowerInvariant() == alignment.ToString().ToLowerInvariant()) {
          return alignment;
        }
      }
      return null;
    }
    public static HorizontalAlignmentEnum NotNull (this HorizontalAlignmentEnum? aligmentQ) {
      CommonDebug.NullCheck(aligmentQ);
      return aligmentQ ?? LastResorts.HorizontalAlignment;
    }
  }
}

