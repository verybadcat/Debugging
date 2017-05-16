using System;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class DateTimeAdditions
  {
    public static double RawMillisecondsBeforeNow(this DateTime time) {
      DateTime now = DateTime.Now;
      TimeSpan elapsed = now.Subtract(time);
      double r = elapsed.TotalMilliseconds;
      return r;
    }
    public static int MillisecondsBeforeNow(this DateTime time)
    {
      double totalMS = RawMillisecondsBeforeNow(time);
      int r = totalMS.RoundDownToInt();
      return r;
    }
    public static double RawSecondsBeforeNow(this DateTime time) {
      DateTime now = DateTime.Now;
      TimeSpan elapsed = now.Subtract(time);
      double r = elapsed.TotalSeconds;
      return r;
    }
    public static int SecondsBeforeNow(this DateTime time) {
      double totalSec = time.RawSecondsBeforeNow();
      int r = totalSec.RoundDownToInt();
      return r;
    }
    public static int MinutesBeforeNow(this DateTime time)
    {
      DateTime now = DateTime.Now;
      TimeSpan elapsed = now.Subtract(time);
      double dMins = elapsed.TotalMinutes;
      int r = dMins.RoundDownToInt();
      return r;
    }
    public static int HoursBeforeNow(this DateTime time)
    {
      DateTime now = DateTime.Now;
      TimeSpan elapsed = now.Subtract(time);
      double totalHours = elapsed.TotalHours;
      int r = totalHours.RoundDownToInt();
      return r;
    }

  }
}

