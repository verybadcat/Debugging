using System;
using System.Drawing;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class AttributedStringDrawingFontSizeAdditions
  {
    #region public methods
    public static float GetDrawingFontSize(this IAttributedString aString, bool substituteBaseSizeIfUndefined) {
      CommonFont font = aString.GetFont(true);
      float r = float.NaN;
      if (font != null) {
        r = font.GetPointSize();
      }
      if (substituteBaseSizeIfUndefined && float.IsNaN(r)) {
        CommonFont baseFont = aString.GetFont(false);
        if (baseFont != null) {
          r = baseFont.GetPointSize();
        }
      }
      return r;
    }
    /// <summary>Use this method when the string might be allowed to be multi-line.
    /// at the minimum size is greater than the height of the available region.</summary>
    public static void ComputeDrawingFontSize(this IAttributedString attributedString, SizeF available, int minimumFontSize, float singleLineShrinkRatio) {
      float fitFontSize = attributedString.MultilineFontSizeThatFits(available, minimumFontSize, singleLineShrinkRatio);
      if (fitFontSize >= attributedString.GetFont(false).GetPointSize()) {
        fitFontSize = float.NaN;
      }
      attributedString.SetDrawingFontSize(fitFontSize);
    }
    /// <summary>
    /// Computes the size of the single line drawing font. If 
    /// length of the string is above dontBotherLength, AND it is
    /// too big for the space by a factor of 1.5, we don't bother.
    /// </summary>
    public static void ComputeSingleLineDrawingFontSize(this IAttributedString attributedString, 
                                                        SizeF available, TextSizing ts = null, float expandToSize = float.NaN,
                                                       int dontBotherLength = 20) {
      // At first sight this looks like code duplication -- why not have a single method for the single-line
      // and multi-line cases? That might not be crazy, but there is another difference -- the multi-line case
      // has a singleLineShrinkRatio (where we scrunch the text slightly to avoid going multi-line. If we are single-line,
      // that isn't a consideration, and the extra parameter could be confusing. On top of all that, the multiline
      // case has a minimumFontSize, which we could but don't have in single line. So I'm going to leave this as two separate
      // methods.
      attributedString.ClearDrawingFontSize();
      if (attributedString.IsEmpty()) {
        // we still need to trim for height.
        CommonFont font = attributedString.GetFont(false);
        float height = available.Height;
        if (height < NumericalTolerances.ScreenWidthOrHeightTooBig) {
          CommonFont shrunkFont = font.FontThatFitsHeight(height, expandToSize);
          float fontSize = shrunkFont.GetPointSize();
          attributedString.SetDrawingFontSize(fontSize);
        }
      }
      else {
        if (attributedString.MightFitOnSingleLine(available.Width*1.5f, false) || attributedString.Length() <= dontBotherLength) {
          // what we really want here may be height-related; not sure. Point is that
          // if the string is short but big, we do need the call.
          attributedString.ComputeSingleLineDrawingFontSize(available, 10, ts, expandToSize);
        }
        else {
          // just want to see this happening; surely it does.
          CommonDebug.BreakPoint();
        }
      }
    }
    #endregion
    /// <summary>Single-line. No minimum. Goes above the base size, up to expandToSize, if it will fit.</summary>
    private static void ComputeSingleLineDrawingFontSize(this IAttributedString aString, SizeF available, int maxDepth, TextSizing ts = null, float expandToFontSize = float.NaN) { 
      ts = ts ?? WJOperatingSystem.TS;
      aString.SetDrawingFontSize(FloatAdditions.Max(aString.GetDrawingFontSize(true), expandToFontSize));
      Size size = ts.BoundingSize(aString, true, float.NaN).ToSize();
      bool ok = (size.Width <= available.Width && size.Height <= available.Height);
      if (!ok) {
        float widthRatio = size.Width / available.Width;
        float heightRatio = size.Height / available.Height;
        float maxRatio = Math.Max(widthRatio, heightRatio);
        float oldPointSize = aString.GetDrawingFontSize(true);
        float guess = oldPointSize.DivideRoundDown(maxRatio, true);
        aString.SetDrawingFontSize(guess);
        if (maxDepth > 1) {
          aString.ComputeSingleLineDrawingFontSize(available, maxDepth - 1, ts);
        } else {
          CommonDebug.BreakPoint();
        }
      }
    }


    /// DESTRUCTIVE -- alters drawing font size.  Binary search.
    private static int FontSizeThatFits(this IAttributedString aString, SizeF available, int maxDepth, 
      int badFontSize, int goodFontSize, int testFontSize, bool singleLine) {
      if (goodFontSize + 1 >= badFontSize) {
        return goodFontSize;
      }
      if (maxDepth == 0) {
        return goodFontSize;
      }
      TextSizing ts = WJOperatingSystem.TS;
      aString.SetDrawingFontSize(testFontSize);
      float inputWidth = singleLine ? float.NaN : available.Width;
      RectangleF rect = ts.BoundingRectangle(aString, true, inputWidth);
      SizeF size = rect.Size;
      int r;
      bool tooBig = (size.Width > available.Width || size.Height > available.Height);
      if (tooBig) {
        int midFontSize = (goodFontSize + testFontSize) / 2;
        r = aString.FontSizeThatFits(available, maxDepth - 1, testFontSize, goodFontSize, midFontSize, singleLine);
      } else {
        int midFontSize = (badFontSize + testFontSize) / 2;
        r = aString.FontSizeThatFits(available, maxDepth - 1, badFontSize, testFontSize, midFontSize, singleLine);
      }
      return r;
    }

    private static int MultilineFontSizeThatFitsBinarySearch(this IAttributedString aString, SizeF available, int maxDepth, 
      int badFontSize, int goodFontSize, int testFontSize, float singleLineShrinkRatio) {
      int r;
      if (goodFontSize > testFontSize) { // weird case that actually comes up on occasion
        r = testFontSize;
      } else {
        bool useMultiLineSize = false;
        CommonFont baseFont = aString.GetFont(false);
        float myFontSize = baseFont.GetPointSize();
        int minSizeForExtraReductionToSingleLine = (myFontSize * singleLineShrinkRatio).RoundToInt();
        if (singleLineShrinkRatio >= 1) {
          useMultiLineSize = true;
        } else {
          TextSizing ts = WJOperatingSystem.TS;
          string myText = aString.Text;
          aString.SetDrawingFontSize(minSizeForExtraReductionToSingleLine);
          RectangleF mySingleLineRectAtMinSize = ts.BoundingRectangle(aString, true);  // IMPORTANT: Must restore the size within this scope.
          float singleLineLengthAtMinSize = mySingleLineRectAtMinSize.Width;
          bool fitsSingleLine = (singleLineLengthAtMinSize <= available.Width);
          if (fitsSingleLine) {
            useMultiLineSize = false;
          } else {
            useMultiLineSize = true;
          }
          aString.SetDrawingFontSize(myFontSize); // restore the size from the cache or later calculations are screwed up.
        }

        r = aString.FontSizeThatFits(available, maxDepth, badFontSize, goodFontSize, testFontSize, !useMultiLineSize); 
      }
      return r;
    }

    /// <summary>Finds the largest font size that fits the available space, with one exception.  If writing at that size would require more than one line,
    /// and shrinking the original by less than the ratio would cause the text to fit on a single line, then we shrink further to become single-line.
    /// The "minimum font size" is a hard lower bound, with one exception: if the height of the font
    /// at the minimum size is greater than the height of the available region, we shrink the font
    /// so that the height of a line matches the height of the available region.</summary>
    private static int MultilineFontSizeThatFits(this IAttributedString aString, SizeF available, 
      int minimumFontSize, float singleLineShrinkRatio = 1f) {
      int r = 0;
      if (aString.GetFont(false).GetPointSize() > 0) {
        // First, check if the line height at the minimumFontSize is too small. If it is, then 
        // all we have to do is find a size that fits within the height.
        SizeF intAvailable = available.ToIntegralSizeF();
        TextSizing ts = WJOperatingSystem.TS;
        aString.SetDrawingFontSize(minimumFontSize);
        float lineHeight = ts.BoundingHeight(aString, true);
        if (lineHeight > intAvailable.Height) {
          // at the minimum size, we are too tall to fit on a single line. So we go below the minimum.
          SizeF bigAvailable = new SizeF(float.MaxValue, intAvailable.Height);
          r = aString.MultilineFontSizeThatFitsBinarySearch(bigAvailable, 10, minimumFontSize, 1, (int)(minimumFontSize / 2), 1f);
        } else {
          float baseFontSize = aString.GetFont(false).GetPointSize();
          int roundedFontSize = (int)baseFontSize;
          r = aString.MultilineFontSizeThatFitsBinarySearch(intAvailable, 10, roundedFontSize, 
            minimumFontSize, roundedFontSize, singleLineShrinkRatio);
        }
      }
      return r;
    }
  }
}

