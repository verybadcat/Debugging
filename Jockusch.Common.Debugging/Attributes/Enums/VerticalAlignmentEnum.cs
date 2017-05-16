using System;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public enum VerticalAlignmentEnum
  {
    Baseline,
    Top,
    Center,
    Bottom
  };

  public static class VerticalAlignmentAdditions
  {
    /// <summary>
    /// top = 0; center = 0.5; baseline = 0.75 (which is hackish and may need to change); bottom=1
    /// </summary>
    public static float Multiplier(this VerticalAlignmentEnum alignment) {
      float r;
      switch (alignment) {
        case VerticalAlignmentEnum.Top:
          r = 0;
          break;
        case VerticalAlignmentEnum.Center:
          r = 0.5f;
          break;
        case VerticalAlignmentEnum.Baseline:
          r = 0.75f;
          break;
        case VerticalAlignmentEnum.Bottom:
          r = 1f;
          break;
        default:
          CommonDebug.BreakPointLog("Unknown alignment");
          r = 0f;
          break;
      }
      return r;
    }
    public static VerticalAlignmentEnum? FromString(string s) {
      foreach (VerticalAlignmentEnum alignment in Enum.GetValues(typeof(VerticalAlignmentEnum))) {
        if (s.ToLowerInvariant() == alignment.ToString().ToLowerInvariant()) {
          return alignment;
        }
      }
      return null;
    }
    public static VerticalAlignmentEnum NotNull (this VerticalAlignmentEnum? alignment) {
      CommonDebug.NullCheck(alignment);
      return alignment ?? LastResorts.VerticalAlignment;
    }
  }
}

