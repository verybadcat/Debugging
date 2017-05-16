using System;

namespace Jockusch.Common
{
  public static class TimeSpanAdditions
  {
    public static string ToHumanFriendlyString(this TimeSpan timeSpan)
    {
      double ms = timeSpan.TotalMilliseconds;
      if (ms < 1E-4) {
        return "just now";
      } else if (ms < 1000) {
        return IntAdditions.SingularOrPluralString((int)ms, "ms", "ms");
      } else {
        double seconds = ms / 1000;
        if (seconds < 60) {
          return IntAdditions.SingularOrPluralString((int)seconds, "second");
        } else {
          double minutes = seconds / 60;
          if (minutes < 60) {
            return IntAdditions.SingularOrPluralString((int)minutes, "minute");
          }
          else {
            double hours = minutes / 60;
            if (hours < 24) {
              return IntAdditions.SingularOrPluralString((int)hours, "hour");
            } else {
              double days = hours / 24;
              return IntAdditions.SingularOrPluralString((int)days, "day");
            }
          }
        }
      }
    }
  }
}

