using System;

namespace Jockusch.Common
{
  public static class NumericalTolerances
  {
    public const double TrigFunctionInputCloseToPiOver2 = 1E-14;
    public const double DoubleCloseToInteger = NumericalTolerances.DoubleTypicalRoundoff;
    public const double DoubleSmallRoundoff = 1E-15;
    public const double DoubleTypicalRoundoff = 1E-12;
    public const double DoubleGenerousRoundoff = 1E-6;
    public const double DoubleModerateRoundoff = 1E-9;
    public const float FloatTypicalRoundoff = 1E-6f;
    public const float ScreenWidthOrHeightTooBig = 1E5f;
    public const int TooManyDigits = 100000;
  }
}

