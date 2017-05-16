using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class AttributedStrings
  {
    public static AttributedString CreateOrUpdate(string text, Func<CommonFont> fontGetter, Color color, AttributedString recycleMe = null) {
      if (recycleMe == null) {
        recycleMe = new AttributedString(text, fontGetter, color);
      } else {
        recycleMe.Text = text;
        recycleMe.FontGetter = fontGetter;
        recycleMe.Color = color;
      }
      return recycleMe;
    }
    public static AttributedString Empty() {
      return AttributedString.TextOnly("");
    }
    public static AttributedString CreateOrUpdate(string text, Func<CommonFont> font, Color color, 
                                                  HorizontalAlignmentEnum horizontalAlignment,
                                                  VerticalAlignmentEnum? verticalAlignmentQ,
                                                  AttributedString recycleMe = null) {
      recycleMe = CreateOrUpdate(text, font, color, recycleMe);
      recycleMe.SetVerticalAlignmentQ(verticalAlignmentQ);
      recycleMe.SetHorizontalAlignmentQ(horizontalAlignment);
      return recycleMe;
    }

    public static AttributedString CreateOrUpdate(string text, CommonFont font, Color color,
                                                  HorizontalAlignmentEnum horizontalAlignment,
                                                  VerticalAlignmentEnum? verticalAlignmentQ,
                                                  AttributedString recycleMe = null) {
      recycleMe = CreateOrUpdate(text, () => font, color,
                                 horizontalAlignment, verticalAlignmentQ,
                                 recycleMe);
      return recycleMe;
    }

    /// <summary>Typically use when we are not going to be displaying the string to the user, i.e. it is for sizing purposes only.</summary>
    public static AttributedString SansColor(string text, CommonFont font, AttributedString recycleMe = null) {
      return CreateOrUpdate(text, font, Color.Black, recycleMe);
    }

    public static AttributedString CreateOrUpdate(string text, CommonFont font, Color color, AttributedString recycleMe = null) {
      return AttributedStrings.CreateOrUpdate(text, () => font, color, recycleMe);
    }

    /// <summary>Inserts spaces to align the columns as closely as possible</summary>
    public static List<AttributedString> FauxColumns(IEnumerable<IEnumerable<string>> entries, AttributedString work, HorizontalAlignmentEnum alignmentWithinColumns, int spacesBetweenColumns) {
      TextSizing ts = TextSizing.Current;
      List<List<float>> lengths = entries.NestedApply(str => {
        work.Text = str;
        float len = ts.BoundingWidth(work, false);
        return len;
      }).ToNestedList();
      List<float> columnWidths = lengths.NestedColumnMax(0);
      List<float> columnXCords = new List<float>();
      // columnXCoords are the anchor coords for the columns -- i.e, left, center, or right of the column, depending on the passed-in horizontal alignment.
      float x = 0;
      float mult = alignmentWithinColumns.Multiplier();
      work.Text = new String(' ', 10);
      float lengthOfTenSpaces = ts.BoundingWidth(work, false);
      if (lengthOfTenSpaces == 0) {
        work.Text = "." + work.Text + ".";
        float periods = ts.BoundingWidth(work, false);
        work.Text = "..";
        float noPeriodsLength = ts.BoundingWidth(work, false);
        lengthOfTenSpaces = periods - noPeriodsLength;
        if (lengthOfTenSpaces == 0) { // the os probably trims leading and trailing spaces.
          CommonDebug.BreakPoint();
          lengthOfTenSpaces = work.GetFont(false).GetPointSize() * 3;
        }
      }
      float spaceLength = lengthOfTenSpaces / 10;
      foreach (float width in columnWidths) {
        x += width * mult;
        columnXCords.Add(x);
        x += width * (1 - mult);
        x += spaceLength * spacesBetweenColumns;
      }
      // we are now ready to build our output
      List<AttributedString> r = new List<AttributedString>();
      int iRow = 0;
      foreach (IEnumerable<string> row in entries) {
        float rowWidth = 0;
        string rowText = "";
        int iColumn = 0;
        foreach (string entry in row) {
          float targetX = columnXCords[iColumn];
          float entryWidth = lengths[iRow][iColumn];
          float floatSpacesNeeded = (targetX - rowWidth - entryWidth * mult) / spaceLength;
          int spacesNeeded = floatSpacesNeeded.RoundToInt();
          rowText += new string(' ', spacesNeeded);
          rowText += entry;
          rowWidth += entryWidth;
          rowWidth += spacesNeeded * spaceLength;
          iColumn++;
        }
        AttributedString attributedRow = new AttributedString(work);
        r.Add(attributedRow);
        attributedRow.Text = rowText;
        iRow++;
      }
      return r;
    }
  }
}

