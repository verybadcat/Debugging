using System;
using System.Collections;
using System.Collections.Generic;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class BitArrayAdditions {
    public static void GrowAndSet(this BitArray bitArray, int index, bool toOn) {
      int length = bitArray.Length;
      if (length < index + 1) {
        bitArray.Length = index + 1;
      }
      bitArray.Set (index, toOn);
    }
    /// <summary>Returns false if the bit array is null, or if every entry is off.</summary> 
    public static bool HasOnEntry(this BitArray bitArray) {
      bool r = false;
      if (bitArray!=null) {
        r = bitArray.GetRanges().IsNonempty();
      }
      return r;
    }
    /// <summary>If the index is above the size of the bitArray, grows the array by adding "false" entries.  Then returns the value.</summary>
    public static bool GrowAndGet(this BitArray bitArray, int index) {
      int oldLength = bitArray.Length;
      if (oldLength <= index) {
        bitArray.Length = index+1;
      }
      bool r = bitArray[index];
      return r;
    }
    private static int ParseString(string s) {
      int r;
      bool success = Int32.TryParse (s, out r);
      if (!success) {
        CommonDebug.UnexpectedInputBreakpoint ();
        r = -1;
      }
      return r;
    }
    /// <summary>Expects an input string like "4" or "5-10" or "3 5-10 12".  If the inputString
    /// is not parsed successfully, will throw an exception. If the InputString is null, returns null.</summary>
    public static BitArray Decode(string s) {
      BitArray r;
      if (s == null) {
        r = null;
      } else {
        string[] whitespace = { " " };
        string[] splits = s.Split(whitespace, StringSplitOptions.RemoveEmptyEntries);
        r = new BitArray(0);
        foreach (string split in splits) {
          int hyphenIndex = split.IndexOf('-');
          if (hyphenIndex == -1) {
            int parseSplit = BitArrayAdditions.ParseString(split);
            if (parseSplit != -1) {
              r.GrowAndSet(parseSplit, true);
            }
          } else {
            string beforeHyphenString = split.Substring(0, hyphenIndex);
            int beforeHyphen = int.Parse(beforeHyphenString);
            string afterHyphenString = split.Substring(hyphenIndex + 1);
            int afterHyphen = int.Parse(afterHyphenString);
            if (beforeHyphen >= 0 && afterHyphen >= 0) {
              for (int i = afterHyphen; i >= beforeHyphen; i--) {
                // we iterate in reverse order so that we only have to grow once.
                r.GrowAndSet(i, true);
              }
            }
          }
        }
      }
      return r;
    }
    /// <summary>The length of the BitArray is not encoded. In other words, the BitArray {true, false} will
    /// roundtrip to the BitArray {true}.</summary>
    public static string GetLocalEncoding(this BitArray ba) {
      string r = "";
      IEnumerable<Tuple<int, int>> ranges = ba.GetRanges();
      bool first = true;
      foreach(Tuple<int, int> range in ranges) {
        if (first) {
          first = false;
        } else {
          r += " ";
        }
        int start = range.Item1;
        int end = range.Item2;
        r += start.ToString();
        if (start != end) {
          r += "-";
          r += end.ToString();
        }
      }
      return r;
    }
    public static BitArray FromBoolList(List<bool> list) {
      BitArray r = new BitArray(list.ToArray());
      return r;
    }
    public static List<bool> ToBoolList(this BitArray array) {
      List<bool> r = new List<bool>();
      foreach (bool b in array) {
        r.Add(b);
      }
      return r;
    }
    public static int OnCount(this BitArray bits) {
      int r = 0;
      foreach (bool bit in bits) {
        if (bit) {
          r++;
        }
      }
      return r;
    }
    /// <summary>Returns a random index whose value in the array is True.  If all values are false, returns -1.</summary>
    public static int RandomOnIndex(this BitArray bits) {
      int onCount = bits.OnCount ();
      int r = -1;
      if (onCount > 0) {
        int i = 0;
        if (onCount > 1) {
          Random rand = new Random ();
          i = rand.Next () % onCount;
        }
        int found = 0;
        while (found <= i) {
          r++;
          if (bits [r]) {
            found++;
          }

        }
      }
      return r;
    }
		public static int FirstIndex(this BitArray bits, bool value) {
			int length = bits.Length;
			for (int i=0; i<length; i++) {
				if (bits[i]==value) {
					return i;
				}
			}
			return -1;
		}
    /// <summary>Ranges are in the form {startIndex, endIndex}</summary>
    public static IEnumerable<Tuple<int, int>> GetRanges(this BitArray bits) {
      int length = bits.Length;
      int rangeStart = -1;
      for (int i=0; i<=length; i++) {
        if (i<length && bits[i]) { // IMPORTANT: i<length has to be before bits[i] here, or we will crash
          if (rangeStart == -1) {
            rangeStart = i;
          }
        } else {
          if (rangeStart!=-1) {
            yield return new Tuple<int, int>(rangeStart, i - 1);
            rangeStart = -1;
          }
        }
      }
    }
    public static BitArray CopyOrNull(BitArray otherArray) {
      BitArray r = null;
      if (otherArray!=null) {
        r = new BitArray(otherArray);
      }
      return r;
    }
    public static bool EqualsBitArray(this BitArray ba1, BitArray ba2) {
      bool r = (ba1 == ba2);
      if (!r) {
        int length1 = ba1.Length;
        int length2 = ba2.Length;
        if (length1 == length2) {
          r = true;
          for (int i=0; i<length1; i++) {
            if (ba1[i]!=ba2[i]) {
              r = false;
              break;
            }
          }
        }
      }
      return r;
    }
  }
}
