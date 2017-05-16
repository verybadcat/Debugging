using System;
using System.Collections;
using Jockusch.Common.Debugging;

namespace Jockusch.Common
{
  public static class ExceptionAdditions
  {
    private static string LinePrefix = "";
    public static string DataString(this Exception e) {
      IDictionary data = e.Data;
      ICollection keys = data.Keys;
     
      string r;
      if (keys.Count == 0) {
        r = "data: [empty dictionary]\n";
      } else {
        r = "data:\n";
        foreach (object key in keys) {
          string keyString = key.ToString();
          object value = data[key];
          string valueString = (value == null)?"null":value.ToString();
          r += (keyString + ":" + valueString + "\n");
        }
      }
      return r;
    }
    public static string EverythingDebugString(this Exception e) {
      string r = "";
      if (e is AggregateException) {
        AggregateException castE = e as AggregateException;
        r = castE.EverythingDebugString();
      } else {
        string message = e.Message;
        string rawStackTrace = e.StackTrace;
        string stackTrace = rawStackTrace?.Replace("(Jockusch.Calculator.", "");
        string data = e.DataString();
        string source = e.Source;
        string baseR = "Message:" + message + "\n" + stackTrace + data + "Source: " + source;
        string prefix = ExceptionAdditions.LinePrefix;
        r = prefix + baseR.Replace("\n", "\n" + prefix);
        if (r.EndsWithInvariant(prefix)) {
          r = r.Substring(0, r.Length - prefix.Length);
        }
        Exception inner = e.InnerException;
        if (inner != null) {
          ExceptionAdditions.LinePrefix = "  " + prefix;
          string innerString = "INNER EXCEPTION:\n" + inner.EverythingDebugString();
          r += innerString;
          ExceptionAdditions.LinePrefix = prefix;
        }
      }
      return r;
    }

    public static string EverythingDebugString(this AggregateException e) {
      var inner = e.InnerExceptions;
      int n = inner.Count;
      string r = "Aggregate Exception with " + n + " children:";
      for (int i=0; i<n; i++) {
        r += "Exception " + (i + 1);
        string childDebug = inner[i].EverythingDebugString();
        r += childDebug.IndentEachLine();
      }
      return r;
    }
    public static void LogAndIgnore(this Exception e) {
      string debug = e.EverythingDebugString();
      #if DEBUG
      CommonDebug.LogAndIgnore(debug);
      #else
      CommonDebug.RecordOperation("Caught and ignored exception" + debug);
      #endif
    }
    public static void LogAndBreak(this Exception e) {
      string debug = e.EverythingDebugString();
      #if DEBUG
      CommonDebug.LogLine(debug);
      CommonDebug.BreakPoint();
      #else
      CommonDebug.RecordOperation("Caught and ignored exception" + debug);
      #endif
    }
    public static void HandleViaOS(this Exception e) {
      WJOperatingSystem os = WJOperatingSystem.Current;
      os.Handle(e);
    }
  }
}

