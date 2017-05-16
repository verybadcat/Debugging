using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Jockusch.Common.Timers;

namespace Jockusch.Common.Debugging
{
  public class DebugStopwatch
  {
    private DateTime _LastStartTime { get; set; }
    public int _IndentSpaces { get; set; }
    public void Indent() {
      _IndentSpaces += 2;
    }
    public void OutDent() {
      _IndentSpaces -= 2;
    }
    private bool CurrentLineIsEmpty { get; set; }
    public void MarkLine(string text) {
      bool running = this.IsRunning;
      if (running) {
        this.Mark(text);
        List<Mark> markList = this.Marks;
        int nMarks = markList.Count;
        Mark last = markList[nMarks - 1];
        last.CarriageReturn = true;
      }
    }
    public void MarkMaybeLine(bool line, params object[] objects) {
      if (line) {
        this.MarkLine(objects);
      } else {
        this.Mark(objects);
      }
    }
    public void MarkLine(params object[] objects) {
      bool running = this.IsRunning;
      if (running) {
        string text = string.Empty;
        bool first = true;
        foreach (object obj in objects) {
          string s = (obj is string) ? (obj as string) : ObjectNamer.NameForObject(obj);
          text += (first ? s : " " + s);
          first = false;
        }
        this.MarkLine(text);
      }
    }
    public static DebugStopwatch SharedInstance = new DebugStopwatch ();
    public static DebugStopwatch Spare = new DebugStopwatch ();
    protected List<Mark> Marks { get; set; }
    public Stopwatch Stopwatch { get; set; }
    public DebugStopwatch()
    {
      this.Reset();
    }
    private static Dictionary<int, DebugStopwatch> _InstanceDictionary;
    private static Dictionary<int, DebugStopwatch> InstanceDictionary {
      get {
        if (_InstanceDictionary == null) {
          _InstanceDictionary = new Dictionary<int, DebugStopwatch> ();
        }
        return _InstanceDictionary;
      }
    }
    // If minMillisecondsBetweenStarts is positive, will not start the watch if it has been started within the time period.
    public static DebugStopwatch GetInstance(int index, int startWithMilliseconds = int.MinValue, int minMillisecondsBetweenStarts = -1) {
      Dictionary<int, DebugStopwatch> dict = DebugStopwatch.InstanceDictionary;
      bool exists = dict.ContainsKey(index);
      DebugStopwatch r;
      if (exists) {
        r = dict[index];
      } else {
        r = new DebugStopwatch ();
        dict[index] = r;

      }
      if (startWithMilliseconds!=int.MinValue) {
        if (!r.IsRunning) {
          r.ResetAndStart(startWithMilliseconds, minMillisecondsBetweenStarts);
        }
      }
      return r;
    }

    public void Start() {
      this._LastStartTime = DateTime.Now;
      Stopwatch watch = new Stopwatch ();
      watch.Start();
      this.Stopwatch = watch;
    }
    public bool IsRunning {
      get {
        Stopwatch watch = this.Stopwatch;
        if (watch != null) {
          bool r = watch.IsRunning;
          return r;
        }
        return false;
      }
    }
    public virtual void MarkIf(bool condition, params object[] mark) {
      if (condition) {
        this.Mark(mark);
      }
    }
    public virtual void Mark(string text) {
      bool running = this.IsRunning;
      if (running) {
        Stopwatch watch = this.Stopwatch;
        if (watch != null) {
          bool empty = this.CurrentLineIsEmpty;
          if (empty) {
            for (int i = 0; i < _IndentSpaces; i++) {
              text = " " + text;
            }
          }
          int ms = (int)watch.ElapsedMilliseconds;
          Mark mark = new Mark (text, ms);
          this.Marks.Add(mark);
        }
      }
    }
    public void Mark(params object[] objects) {
      bool running = this.IsRunning;
      if (running) {
        string text = CommonDebug.CombineDebugString(objects);
        this.Mark(text);
      }
    }
    public int MarkCount() {
      return this.Marks.Count;
    }
    public void LineMark(params object[] objects) {
      bool running = this.IsRunning;
      if (running) {
        if (this.Marks.Count > 0) {
          this.Marks.Last().CarriageReturn = true;
        }
        this.Mark(objects);
      }
    }
    public List<string> ToDebugStrings() {
      List<string> r = new List<string> ();
      List<Mark> marks = this.Marks;
      bool cloned = false;
      int attempts = 0;
      List<Mark> clone = null;
      while (!cloned) {
        try {// prevent mutation crash if another thread changes list.
          clone = new List<Mark> (marks); 
          cloned = true;
        } catch {
          if (attempts > 10000) {
            CommonDebug.BreakPoint();
          }
        }
      }
      string s = "";
      bool first = true;
      int prevMS = 0;
      foreach (Mark mark in clone) {
        if (first) {
          first = false;
        } else {
          s += "     ";
        }
        string markString = mark.ToString(prevMS);
        prevMS = mark.Milliseconds;
        s += markString;
      }
      string totalString = this.GetTotalTimeString();
      s += totalString;
      r.Add(s);
      return r;
    }
    public string GetTotalTimeString() {
      List<Mark> marks = this.Marks;
      if (marks.IsEmptyEnumerable()) {
        return " (total: 0)";
      } else {
        return " (total: " + marks.Last().Milliseconds.ToString() + ")";
      }
    }
    public List<string> LogAndReset() {
      if (this.IsRunning) {
        List<string> logMe = this.ToDebugStrings();
        foreach (string s in logMe) {
          CommonDebug.LogColored(1, s);
        }
        this.Reset();
        return logMe;
      } else {
        return new List<string>();
      }
    }
    public List<string> LogToScreenAndReset() {
      List<string> logMe = this.ToDebugStrings();
      foreach (string s in logMe) {
        CommonDebug.DebugLogToScreen(s);
      }
      this.Reset();
      return logMe;
    }
    public string LogBriefAndReset() {
      List<string> strings = this.ToDebugStrings();
      string r = strings.Last();
      CommonDebug.WriteLine(r);
      return r;
    }
    private List<Mark> SummarizedMarks {
      get {
        int prevMS = 0;
        List<Mark> r = new List<Mark> ();
        List<Mark> myMarks = this.Marks;
        foreach (Mark rawMark in myMarks) {
          int ms = rawMark.Milliseconds;
          int delta = ms - prevMS;
          prevMS = ms;
          string text = rawMark.Text.Trim();
          bool found = false;
          foreach (Mark mark in r) {
            if (mark.Text == text) {
              mark.Milliseconds += delta;
              found = true;
              break;
            }
          }
          if (!found) {
            r.Add(new Mark (text, delta));
          }
        }
        return r;
      }
    }
    public void LogSummary(int ignoreMS) {
      CommonDebug.WriteLine("*****************************************************************************************SUMMARY:");
      List<Mark> summarized = this.SummarizedMarks;
      List<Mark> sortedSummarized = summarized.OrderBy(m => m.Text).ToList();
      foreach (Mark mark in sortedSummarized) {
        if (mark.Milliseconds > ignoreMS) {
          CommonDebug.WriteLine(mark.ToString());
        }
      }
      string total = this.GetTotalTimeString();
      CommonDebug.WriteLine(total);
      CommonDebug.WriteLine("*************************************************************************************************");
    }
    public void LogAndResetWithSummary(int ignoreMS) {
      this.LogSummary(ignoreMS);
      this.LogAndReset();
    }
    ///<summary>Does NOT reset the last start time.</summary> 
    public void Reset() {
      this.Marks = new List<Mark> ();
      Stopwatch inner = this.Stopwatch;
      if (inner != null) {
        if (inner.IsRunning) {
          inner.Stop();
        }
      }
    }
    public void ResetAndStart() {
      this.Reset();
      this.Start();
    }
    private Timer LogTimer;
    private void LogAndReset(object ignored) {
      this.LogAndReset();
      this.LogTimer.Cancel();
      this.LogTimer.Dispose();
      this.LogTimer = null;
    }
    private void LogBriefAndReset(object ignored) {
      this.LogBriefAndReset();
      this.LogTimer.Cancel();
      this.LogTimer.Dispose();
      this.LogTimer = null;
    }

    private bool ShouldStart(int minMillisecondsBetweenStarts) {
      DateTime last = this._LastStartTime;
      bool r = (minMillisecondsBetweenStarts <= 0 || last.MillisecondsBeforeNow() > minMillisecondsBetweenStarts);
      return r;
    }
    
    public void ResetAndStart(int millisecondsUntilLog, int minMillisecondsBetweenStarts) {
      if (this.ShouldStart(minMillisecondsBetweenStarts)) { 
        this.ResetAndStart(millisecondsUntilLog);
      }
    }
    /// <summary>If the stopwatch is already running, does nothing.  If not, starts it and it will log after the delay.
    /// If the logDelay is negative, will use its absolute value, but makes a brief log instead of a full one.
    /// If a logAction is passed in, it ignores the above and uses the passed-in action instead.</summary>
    public void ResetAndStart(int millisecondsUntilLog, Action logAction = null) {
      bool running = this.IsRunning;
      if (!running) {
        Action<object> timerAction;
        this.ResetAndStart();
        if (logAction == null) {
          if (millisecondsUntilLog < 0) {
            timerAction = (obj) => this.LogBriefAndReset(obj);
            millisecondsUntilLog *= -1;
          } else {
            timerAction = (obj) => this.LogAndReset();
          }
        } else {
          timerAction = obj => logAction();
          millisecondsUntilLog = Math.Abs(millisecondsUntilLog);
        }
        Timer timer = new Timer (timerAction, null, millisecondsUntilLog, int.MaxValue);
        this.LogTimer = timer;
      }
    }
  }
}

