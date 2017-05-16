#if DEBUG
#define COMMONDEBUG
#endif
#if __TESTDISTRO__
#define COMMONDEBUG
#endif
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

/* IMPORTANT: "Debug/" is in GitIgnore.  So we can't name our namespace "Debug". */
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Numerics;
using System.Threading.Tasks;
using System.Threading;
using Jockusch.Common.Notifications;

namespace Jockusch.Common.Debugging
{
  public static class CommonDebug
  {
    // typically set when something is wrong and more breakpoints and/or logging are needed.
    // This is not enforced, however.
    private static bool _Flag {
      get;
      set;
    }
    public static bool Flag {
      get;
      set;
    }

    /// <summary>Default value is the identity function.</summary>
    public static Func<object, object> ToDebugFriendlyObject { get; set; }

    public static Action FailTestAction { get; set; }

    [Conditional("DEBUG")]
    public static void FailTest() {
      Action fail = CommonDebug.FailTestAction;
      if (fail != null) {
        fail();
      }
    }

    [Conditional("DEBUG")]
    public static void ComplainToProgrammer() {
      CommonDebug.BreakPoint();
      CommonDebug.FailTest();
    }

    public static void SetFlagAndIgnore(bool value) {
      // this method simply provides a way to change CommonDebug.Flag without writing
      // text that will come up on a search for "CommonDebug.Flag".
      CommonDebug.Flag = value;
    }
    /// <summary>Logs iff the flag is set or any object in the array is flagged.</summary>
    public static void FlagLog(params object[] objs) {
      if (CommonDebug.Flag || CommonDebug.ObjectIsFlagged(objs.FirstOrDefault())) {
        CommonDebug.LogLine(objs);
      }
    }
    public const bool IsDebugBuild =
#if DEBUG
      true;
#else
      false;
#endif
    public static void LogAncestry<T>(T obj)
      where T : IGetParent<T> {
      IEnumerable<T> ancestors = obj.Ancestors<T, T>();
      CommonDebug.LogEnumerable(ancestors);
    }
    private static List<object> _FlaggedObjects { get; set; } = new List<object>();
    public static List<object> GetFlaggedObjects() {
      return _FlaggedObjects;
    }
    public static DebugLevel GetDebugLevel() {
#if DEBUG
      return DebugLevel.Debugger;
#elif COMMONDEBUG
      return DebugLevel.Console;
#else
      return DebugLevel.None;
#endif

    }
    public static void SetFlagObjectAndIgnore(object obj, bool flag = true) {
      FlagObject(obj, flag);
    }
    // If flag is false, removes flag
    public static void FlagObject(object obj, bool flag = true) {
#if DEBUG
      if (flag) {
        if (obj != null) {
          _FlaggedObjects.Add(obj);
        }
      } else {
        _FlaggedObjects.Remove(obj);
      }
#endif
    }
    public static void FlagTree(ITreeDescription obj) {
      CommonDebug.FlagObject(obj);
      foreach (object child in obj.GetTreeDescriptionChildren()) {
        if (child is ITreeDescription) {
          CommonDebug.FlagTree(child as ITreeDescription);
        } else {
          CommonDebug.FlagObject(child);
        }
      }
    }
    public static void UnflagObject(Object obj) {
      _FlaggedObjects.Remove(obj);
    }
    public static bool ObjectIsFlagged(Object obj) {
      // the two objects may be equal but not identical. In that case,
      // we want to return false.
      foreach (object flagged in _FlaggedObjects) {
        if (ReferenceEquals(flagged, obj)) {
          return true;
        }
      }
      return false;

    }
    public static bool RunningUnitTests { get; set; }
    /// <summary>For the stupid Binder.InvokeMember not found exception</summary>
    public static bool CompilerIsSick { get; set; }
    public const int RecentOperationsListSize =
#if DEBUG
      30;
#else
      30;
#endif
    public const int ShortOperationsListSize = 5;
    private static ConcurrentQueue<string> _RecentOperations { get; set; }
    public static ConcurrentQueue<string> RecentOperations {
      get {
        if (_RecentOperations == null) {
          _RecentOperations = new ConcurrentQueue<string>();
        }
        return _RecentOperations;
      }
    }
    private static ConcurrentQueue<string> _ShortOperations { get; set; }
    public static ConcurrentQueue<string> ShortOperations {
      get {
        if (_ShortOperations == null) {
          _ShortOperations = new ConcurrentQueue<string>();
        }
        return _ShortOperations;
      }
    }
    public static void RecordOperationOnShortQueue(params object[] objects) {
      CommonDebug.RecordOperationImplementation(false, CommonDebug.ShortOperations, ShortOperationsListSize, objects);
    }
    public static void RecordOperation(params object[] objects) {
      ConcurrentQueue<string> queue = CommonDebug.RecentOperations;
      int size = CommonDebug.RecentOperationsListSize;
      CommonDebug.RecordOperationImplementation(false, queue, size, objects);
    }
    public static void RecordOperationOnce(params object[] objects) {
      ConcurrentQueue<string> queue = CommonDebug.RecentOperations;
      int size = CommonDebug.RecentOperationsListSize;
      CommonDebug.RecordOperationImplementation(true, queue, size, objects);
    }
    public static void RecordTestOperationOnce(params object[] objects) {
      List<object> list = objects.ToList();
      list.Insert(0, "TestLog");
      object[] array = list.ToArray();
      CommonDebug.RecordOperationOnce(array);
    }
    ///<summary> If the string is already in the list, does nothing. </summary>
    private static void RecordOperationImplementation(bool onlyOnce, ConcurrentQueue<string> queue, int maxQueueSize, params object[] objects) {
      string text = CommonDebug.CombineDebugString(objects);
      bool alreadyThere = queue.Count > 0 && queue.Contains(text);
      if (!onlyOnce || !alreadyThere) {
        queue.Enqueue(text);
        string foo;
        while (queue.Count > maxQueueSize) {
          queue.TryDequeue(out foo);
        }
      }
    }
    /// <summary>In debug mode, repeatedly evaluates the function so you can see how it is screwing up.
    /// In release mode, is equivalent to just calling the function. Obviously, this assumes that repeated
    /// calls to the function are independent of each other.</summary>

    public static void ReleaseLogOnce(params object[] args) {
      string debug = CombineDebugString(args) + Environment.NewLine;
#if DEBUG
      CommonDebug.LogLineOnce(debug);
#else
      CommonDebug.AppendToDebugString(debug, true);
#endif
    }

    public static void ReleaseLog(params object[] args) {
      string debug = CombineDebugString(args) +Environment.NewLine;
#if DEBUG
      CommonDebug.LogLine(debug);
#else
      CommonDebug.AppendToDebugString(debug);
#endif
    }

    [Conditional("DEBUG")]
    public static void NoDebugger() {
      if (!Debugger.IsAttached) {
        CommonDebug.LogLineOnce("No debugger; can't break!");
      }
    }
    public static List<AttributedString> DebugConsoleLines { get; set; } = new List<AttributedString>();
    private static HashSet<string> _ShowingTags { get; set; }
    public static HashSet<string> ShowingTags {
      get {
        if (_ShowingTags == null) {
          _ShowingTags = new HashSet<string>();
          _ShowingTags.Add(null);
          _ShowingTags.Add("");
        }
        return _ShowingTags;
      }
      set {
        _ShowingTags = value;
      }
    }
    public static List<string> GetAllTags() {
      List<string> r = new List<string>();
      List<AttributedString> lines = CommonDebug.DebugConsoleLines;
      foreach (AttributedString line in lines) {
        string tag = line.Tag;
        if (!(r.Contains(tag))) {
          r.Add(tag);
        }
      }
      r.Sort();
      return r;
    }
    public static string RecentOperationsDebugText {
      get {
        string r = "Failed to get RecentOperationsDebugText";
        try {
          ConcurrentQueue<string> ops = CommonDebug.RecentOperations;
          List<string> opsList = ops.ToList<string>();
          ConcurrentQueue<string> shortOps = CommonDebug.ShortOperations;
          List<string> shortList = shortOps.ToList<string>();
          r = "Recent operations (most recent last):\n" + string.Join("\n", opsList);
          if (shortList != null) {
            if (shortList.Count > 0) {
              r += "*****" + ("short queue:\n" + string.Join("\n", shortList));
            }
          }
          if (ops.Count > 0) {
            r += "\n";
          }
        }
        catch {
          CommonDebug.Ignore(); // suppresses warning
        }
        return r;
      }
    }
    public static Func<string> GetStackTrace { get; set; }
    public static string StackTrace {
      get {
        string r;
        if (GetStackTrace == null) {
          r = "Stack trace unavailable; platform needs to define.";
        } else {
          r = GetStackTrace();
        }
        r = r.Replace("/Users/william/Documents/Calculator/Jockusch.Common/", "");
        r = r.Replace("/Users/william/Documents/Calculator/Jockusch.Calculator.Droid/", "");
        r = r.Replace("Jockusch.Common.", "");
        return r;
      }
    }
    private static Action<string> __WriteLine;
    public static Action<string> WriteLineIfPossible {
      get {
        if (__WriteLine == null) {
          return s => {
          };
        }
        return _WriteLine;
      }
    }
    private static Action<string> _WriteLine {
      get {
        if (__WriteLine == null) {
#if DEBUG
          CommonDebug.BreakPoint();
          throw new InvalidOperationException("Getting CommonDebug.WriteLine before setting CommonDebug.WriteLine is not allowed.");
#else
          return ActionFactory.TypedNoOp;
#endif
        }
        return __WriteLine;
      }
      set {
        __WriteLine = value;
      }
    }
    public static void WriteColored(Color color, string s, bool really = true) {
      CommonDebug.WriteTaggedColored(color, null, s, really);
    }
    public static void WriteTagged(string tag, string s, bool really = true) {
      Color color = DebugTagColors.GetColor(tag);
      CommonDebug.WriteTaggedColored(color, tag, s, really);
    }
    public static void LogStringsContainingTag(string tag) {
      List<AttributedString> debug = CommonDebug.DebugConsoleLines.Where(str => str.Tag == tag).ToList();
      foreach (AttributedString aString in debug) {
        if (aString.Tag == tag) {
          CommonDebug.WriteLine(aString.Text); // we write it without the tag, to avoid duplication within the tagged strings
        }
      }
    }
    public static Func<CommonFont> DebugFont = () => new CommonFont(AppEnvironment.OperatingSystem.BaseFontSize().MultiplyAndRound(1.1f));
    private static int WriteRecursionCount = 0;
    /// <summary>If really is false, adds the string to the list of logged strings, but does not log
    /// to the console.</summary>
    public static void WriteTaggedColored(Color color, string tag, string s, bool really = true) {
      if (WriteRecursionCount < 3) {
        try {
          WriteRecursionCount++;
#if DEBUG
          s = CommonDebug.IndentString + s;
#endif
          if (really) {
            _WriteLine(s);
          }
          AttributedString line = AttributedStrings.CreateOrUpdate(s, CommonDebug.DebugFont, color,
                                  HorizontalAlignmentEnum.Left, null);
          line.Tag = tag;
          CommonDebug.DebugConsoleLines.Add(line);
          CommonDebug.NotifyUpdates();
        }
        finally {
          WriteRecursionCount--;
        }
      }
    }
    public static void ClearDebugConsole() {
      CommonDebug.DebugConsoleLines.Clear();
      CommonDebug.NotifyUpdates();
    }
    private static void NotifyUpdates() {
      WJNotificationCenter.DefaultCenter.Post(NotificationKey.DebugTextLogged, null);
    }
    public static void WriteLine(string s) {
      CommonDebug.WriteColored(Color.Black, s);
    }
    private static HashSet<string> _LoggedStrings { get; set; } = new HashSet<string>();
    public static bool WriteLineOnce(string s, Color? color = null) {
      bool r = false;
      color = color ?? Color.Black;
#if DEBUG
      if (!_LoggedStrings.Contains(s)) {
        _LoggedStrings.Add(s);
        CommonDebug.WriteColored(color.Value, s);
        r = true;
      }
#endif
      return r;
    }
    public static void ClearOnceLogs() {
      _LoggedStrings.Clear();
    }
    public static bool WriteLineOnce(string s, int colorIndex) {
      Color color = DebugStatics.DebugColor(colorIndex);
      return CommonDebug.WriteLineOnce(s, color);

    }
    public static void SetWriteLine(Action<string> action) {
      _WriteLine = action;
    }
    public static void DoNothing(string someString) {
      // do nothing.  This is a way to avoid "unused variable" warnings.
    }
    public static void DoNothing() {
    }
    public static void Ignore() {
    }
    /// <summary>Use Ignore() for code that is expected to remain in place; not
    /// temporary debugging code.</summary>
    public static void Ignore(object someObject) {
    }
    public static void Ignore(object obj1, object obj2) {
    }
    public static void Ignore(object obj1, object obj2, object obj3) {
    }
    public static void DoNothing(object someObject) {
    }
    public static void DoNothing(object obj1, params object[] moreObjects) {
    }
    private static HashSet<string> OnceDone { get; } = new HashSet<string>();
    [Conditional("DEBUG")]
    public static void Once(object key, Action action) {
      string keyString = CommonDebug.GetDebugString(key);
      if (!(OnceDone.Contains(keyString))) {
        OnceDone.Add(keyString);
        action();
      }
    }
    public static int BreakCount;
    public static void IncrementBreakCount() {
#if DEBUG
      Interlocked.Increment(ref BreakCount);
#endif
    }
    public static void DecrementBreakCount() {
#if DEBUG
      Interlocked.Decrement(ref BreakCount);
#endif
    }
    public static bool AtBreakPoint() {
      CancellationTokenSource cancel = new CancellationTokenSource();
      CancellationToken token = cancel.Token;
      Task<int> task = Task.Run(() => 2, token);
      for (int i = 0; i < 1000000; i++) {
        if (task.IsCompleted) {
          return false;
        }
      }
      cancel.Cancel();
      return true;
    }
    /// <summary> only actually updated in debug version, but release version has BreakPointHitCount 
    /// as well to keep compiler happy, or in case we allow BreakPoint() to be not conditional.</summary>
    public static int BreakPointHitCount = 0;
    [Conditional("DEBUG")]
    public static void BreakPoint() {
      CommonDebug.BreakPointHitCount++; // we increment the hit count, regardless of whether or not we actually break.
      int nBreaks = CommonDebug.BreakCount; // to disable CommonDebug breakpoints for the current run only, change this to a positive value in your debugger
      if (nBreaks == 0) {
        if (Debugger.IsAttached) {
          CommonDebug.IncrementBreakCount();
          if (!CommonDebug.AtBreakPoint()) {
            Debugger.Break();
          }
          CommonDebug.BreakCount = nBreaks;  // set to nBreaks; NOT the using.
        } else {
          CommonDebug.NoDebugger();
        }
      }
    }
#if DEBUG
    public static bool PostStartupCompleted { get; set;}
#endif
    [Conditional("DEBUG")]
    public static void MaybeNullCheck(bool really, object checkMe) {
      if (really) {
        CommonDebug.NullCheck(checkMe);
      }
    }
    [Conditional("DEBUG")]
    public static void HardBreakPoint() {
      if (Debugger.IsAttached) {
        Debugger.Break();
      }
    }
    [Conditional("DEBUG")]
    public static void BreakPointIfFlag() {
      if (CommonDebug.Flag) {
        CommonDebug.BreakPoint();
      }
    }
    [Conditional("DEBUG")]
    public static void BreakPointIf(bool condition) {
      if (condition) {
        CommonDebug.BreakPoint();
      }
    }
#if DEBUG
    private static string _IndentString { get; set; }
    public static string IndentString {
      get {
        if (_IndentString == null) {
          _IndentString = "";
        }
        return _IndentString;
      }
      set {
        _IndentString = value;
      }
    }
#endif
    public static void Indent() {
#if DEBUG
      CommonDebug.IndentString += "  ";
#endif
    }
    public static void Outdent() {
#if DEBUG
      if (CommonDebug.IndentString.Length > 1) {
        CommonDebug.IndentString = CommonDebug.IndentString.Substring(2);
      }
#endif
    }
    [Conditional("DEBUG")]
    public static void NullCheck(this Object someObject, bool throwExceptionOnFailure = false) {
      if (someObject == null) {
        CommonDebug.BreakPointLog("Failed null check!");
        if (throwExceptionOnFailure) {
          throw new BreakingException();
        }
      }
    }
    public static void HardNullCheck(Object someObject) {
      if (someObject == null) {
        CommonDebug.LogLine("Failed null check");
        CommonDebug.HardBreakPoint();
      }
    }
    [Conditional("DEBUG")]
    public static void NullCheck(Object obj1, Object obj2, bool throwExceptionOnFailure = false) {
      CommonDebug.NullCheck(obj1, throwExceptionOnFailure);
      CommonDebug.NullCheck(obj2, throwExceptionOnFailure);
    }
    [Conditional("DEBUG")]
    public static void NullCheck(Object obj1, Object obj2, Object obj3, bool throwExceptionOnFailure = false) {
      CommonDebug.NullCheck(obj1, throwExceptionOnFailure);
      CommonDebug.NullCheck(obj2, obj3, throwExceptionOnFailure);
    }
    [Conditional("DEBUG")]
    public static void NullCheck(Object obj1, Object obj2, Object obj3, Object obj4, bool throwExceptionOnFailure = false) {
      CommonDebug.NullCheck(obj1, obj2, throwExceptionOnFailure);
      CommonDebug.NullCheck(obj3, obj4, throwExceptionOnFailure);
    }
    [Conditional("DEBUG")]
    public static void NullCheck(Object obj1, Object obj2, Object obj3, Object obj4, Object obj5, bool throwExceptionOnFailure = false) {
      CommonDebug.NullCheck(obj1, obj2, obj3, throwExceptionOnFailure);
      CommonDebug.NullCheck(obj4, obj5, throwExceptionOnFailure);
    }
    public static bool ReturningCountCheck<TSource>(IEnumerable<TSource> enumerable, int minCount) {
      bool r = false;
      int count = -1;
      if (enumerable != null) {
        count = enumerable.Count();
        r = (count >= minCount);
      }
      if (!r) {
        CommonDebug.BreakPoint();
      }
      return r;
    }
    /// <summary>Returns true iff the object is not null.</summary>
    public static bool ReturningNullCheck(Object someObject) {
      bool r = true;
      if (someObject == null) {
        CommonDebug.BreakPointLog("Failed null check!");
        r = false;
      }
      return r;
    }
    public static bool ReturningNullCheck(Object obj1, Object obj2) {
      bool r = CommonDebug.ReturningNullCheck(obj1) && CommonDebug.ReturningNullCheck(obj2);
      return r;
    }
    public static bool ReturningNullCheck(Object obj1, Object obj2, Object obj3) {
      bool r = CommonDebug.ReturningNullCheck(obj1, obj2) && CommonDebug.ReturningNullCheck(obj3);
      return r;
    }
    public const string Stars = "*************************************************************************************************************************************************************************************";

    [Conditional("DEBUG")]
    public static void LogStars() {
      CommonDebug.WriteLine(CommonDebug.Stars);
    }
    /// <summary>Assertion that is only checked in debug mode.</summary>
    [Conditional("DEBUG")]
    public static void DebugAssert(bool condition) {
      if (!condition) {
        CommonDebug.BreakPointLog("Assertion failure!");
      }
    }
    public static bool Assert(bool condition, bool throwExceptionOnFailure) {
      if (!condition) {
        CommonDebug.BreakPointLog("Assertion failure!");
        if (throwExceptionOnFailure) {
          throw new InvalidOperationException();
        }
      }
      return condition;
    }
    public static bool HardAssert(bool condition) {
      if (!condition) {
        CommonDebug.HardBreakPoint();
      }
      return condition;
    }
    public static void UnexpectedInputBreakpoint() {
      CommonDebug.BreakPointLog("Unexpected input");
    }

    public static void InvalidOperationBreakpoint() {
      CommonDebug.BreakPointLog("Invalid operation");
    }
    [Conditional("DEBUG")]
    public static void BreakPointLog(string errorMessage) {
      if (CommonDebug.BreakCount == 0) {
        if (CommonDebug.WriteLineOnce(errorMessage)) {
          CommonDebug.BreakPoint();
        }
      }
    }

    [Conditional("DEBUG")]
    public static void BreakPoint(object responsibleObject) {
      CommonDebug.LogEvents(responsibleObject);
      CommonDebug.BreakPoint();
    }

    public static void NotFoundBreakPoint() {
      CommonDebug.BreakPointLog("Not found!");
    }

    public static void ShouldNeverHappenBreakpoint() {
      CommonDebug.BreakPointLog("Should never happen!");
    }
    [Conditional("DEBUG")]
    public static void DeadCodeCheck() {
      CommonDebug.BreakPointLog("Not dead code!");
    }
    /// <summary>
    /// Execution needs to end, due to a problem.  First, try a breakpoint.  Throw an exception afterwards.
    /// </summary>
    public static void BreakPoint(string errorMessage, Exception exception = null) {
      CommonDebug.BreakPointLog(errorMessage);
      if (exception != null) {
        throw exception;
      }
    }
    public static string GetDebugString(Object rawObject, bool longForm = false) {
      string r;
      Object fromObject;
      WJOperatingSystem os = AppEnvironment.OperatingSystemNullOk();
      if (os!= null) {
        fromObject = os.ToDebugFriendlyObject(rawObject, longForm);
      } else {
        fromObject = rawObject;
      }
      if (fromObject == null) {
        r = "null";
      } else {
        r = fromObject as string;
        if (r == null) {
          if (fromObject is int || fromObject is float || fromObject is double || fromObject is long || fromObject is byte
            || fromObject is uint || fromObject is ulong 
              || fromObject is bool || fromObject is Color || fromObject is Enum || fromObject is BigInteger
              || fromObject is TimeSpan) {
            r = fromObject.ToString();
          } else if (fromObject is RectangleF) {
            r = ((RectangleF)fromObject).ToShortString();
          } else if (fromObject is Rectangle) {
            r = ((Rectangle)fromObject).ToRectangleF().ToShortString();
          } else if (fromObject is SizeF) {
            r = ((SizeF)fromObject).ToDebugString();
          } else if (fromObject is Size) {
            r = ((Size)fromObject).Width.ToString() + " " + ((Size)fromObject).Height.ToString();
          } else if (fromObject is Point) {
            r = ((Point)fromObject).X.ToString() + " " + ((Point)fromObject).Y.ToString();
          } else if (fromObject is PointF) {
            r = ((PointF)fromObject).X.ToString() + " " + ((PointF)fromObject).Y.ToString();
          } else if (fromObject is Complex) {
            r = ComplexAdditions.ToString((Complex)fromObject, false);
          } else if (fromObject is Task) {
            Task t = ((Task)fromObject);
            r = "Task " + t.Status;
            if (t.IsFaulted) {
              r += " " + ObjectNamer.ExistingNameForObject (t.Exception);
            }
          } else {
            IDebugString debugObject = fromObject as IDebugString;
            if (debugObject != null) {
              try {
                r = debugObject.ToDebugString();
              }
              catch {
                r = ObjectNamer.NameForObject(debugObject);
              }
              r = debugObject.ToDebugString();
            } else {
                string name = ObjectNamer.NameForObject(fromObject);
                r = name;
            }
          }
        }
      }
      if (fromObject != rawObject) {
        r = "_" + r;
      }
      return r;
    }
    public static string CombineDebugString(params object[] args) {
      string r = "";
      bool first = true;
      foreach (Object obj in args) {
        if (first) {
          first = false;
        } else {
          r += " ";
        }
        string debug = CommonDebug.GetDebugString(obj);
        r += debug;
      }
      return r;
    }
    [Conditional("DEBUG")]
    public static void LogStackTrace() {
      string trace = CommonDebug.StackTrace;
      CommonDebug.LogLine(trace);
    }
    [Conditional("DEBUG")]
    public static void LogSilentlyTagged(string tag, params Object[] args) {
      Color color = DebugTagColors.GetColor(tag);
      CommonDebug.LogSilentlyTaggedColored(color, tag, args);
    }
    [Conditional("DEBUG")]
    public static void LogSilentlyTaggedColored(Color color, string tag, params Object[] args) {
      string logMe = CommonDebug.CombineDebugString(args);
      CommonDebug.WriteTaggedColored(color, tag, logMe, false);
    }
    [Conditional("DEBUG")]
    public static void LogSilentlyTaggedColored(int colorIndex, string tag, params Object[] args) {
      Color color = DebugStatics.DebugColor(colorIndex);
      CommonDebug.LogSilentlyTaggedColored(color, tag, args);
    }
    [Conditional("DEBUG")]
    public static void LogLine(params Object[] args) {
      string logMe = CommonDebug.CombineDebugString(args);
      CommonDebug.WriteLine(logMe);
    }
    public static void LogIndentAllLines(params Object[] args) {
      string logMe = CommonDebug.CombineDebugString(args);
      string[] breakLog = logMe.SplitIntoLines();
      foreach(string log in breakLog) {
        CommonDebug.LogLine(log);
      }
    }
    public static void LogColored(int colorIndex, params Object[] args) {
      CommonDebug.LogColoredIndex(colorIndex, args);
    }
    /// <summary>Same as LogColored, but assemblies that don't know about
    /// "Color" won't barf when calling this</summary>
    public static void LogColoredIndex(int colorIndex, params Object[] args) {
      // compiler fails to find this method if we call LogColored from an assembly that does not know about System.Drawing.
      Color color = DebugStatics.DebugColor(colorIndex);
      CommonDebug.LogColored(color, args);
    }
    public static void LogColoredOnce(int colorIndex, params Object[] args) {
      // This is very slow on Droid.
      string logMe = CommonDebug.CombineDebugString(args);
      CommonDebug.WriteLineOnce(logMe, colorIndex);
    }
    public static void LogEnumerableColored<T>(int colorIndex, IEnumerable<T> enumerable) {
      Color color = DebugStatics.DebugColor(colorIndex);
      CommonDebug.LogEnumerableColored(color, enumerable);
    }
    public static void LogEnumerable<T>(IEnumerable<T> enumerable) {
      CommonDebug.LogEnumerableColored(Color.Black, enumerable);
    }
    public static void LogEnumerable<T>(IEnumerable<T> enumerable, params object[] headers) {
      CommonDebug.LogLine(headers);
      CommonDebug.LogEnumerable(enumerable);
    }
    public static void LogEnumerableTitled<T>(object title, IEnumerable<T> enumerable) {
      CommonDebug.LogLine(title);
      CommonDebug.Indent();
      CommonDebug.LogEnumerable(enumerable);
      CommonDebug.Outdent();
    }
    public static void LogEnumerableTagged<T>(string tag, IEnumerable<T> enumerable) {
      CommonDebug.Indent();
      foreach (T t in enumerable) {
        CommonDebug.LogSilentlyTagged(tag, t);
      }
      CommonDebug.Outdent();
    }
    public static void LogEnumerableColored<T>(Color color, IEnumerable<T> enumerable) {
      CommonDebug.Indent();
      if (enumerable == null) {
        CommonDebug.LogColored(color, "[null enumerable]");
      } else if (enumerable.IsEmptyEnumerable()) {
        CommonDebug.LogColored(color, "[Empty enumerable]");
      } else {
        foreach (T t in enumerable) {
          CommonDebug.LogColored(color, t);
        }
      }
      CommonDebug.Outdent();
    }
    public static string CombineEnumerableDebugStrings<T>(IEnumerable<T> enumerable, string separator = null) {
      if (separator == null) {
        separator = CommonDebug.NewLine;
      }
      string r = string.Join(separator, enumerable.Select(item => CommonDebug.GetDebugString(item)));
      return r;
    }
    public static void LogTagged(string tag, params Object[] args) {
      string combineArgs = CommonDebug.CombineDebugString(args);
      CommonDebug.WriteTagged(tag, combineArgs);
    }
    public static void LogColored(Color color, params Object[] args) {
      string logMe = CommonDebug.CombineDebugString(args);
      CommonDebug.WriteColored(color, logMe);
    }
    /// <summary>Exactly the same as LogLine. Purpose is so that we can
    /// use this for logs that we are never going to want to clean up.</summary>
    [Conditional("DEBUG")]
    public static void LogAndIgnore(params Object[] args) {
      string logMe = CommonDebug.CombineDebugString(args);
      Color color = DebugStatics.DebugColor(1);
      CommonDebug.WriteColored(color, logMe);
    }
#if DEBUG
    private static DateTime _LastLogPerSecondDateTime { get; set; }
#endif
    public static void LogColoredPerSecond(int colorIndex, params Object[] args) {
#if DEBUG
      if (_LastLogPerSecondDateTime.MillisecondsBeforeNow() > 1000) {
        CommonDebug.LogColored(colorIndex, args);
        _LastLogPerSecondDateTime = DateTime.Now;
      }
#endif
    }
    private static Dictionary<string, string> _LastLogsForKeys { get; } = new Dictionary<string, string>();
    public static void LogIfChanged(object keyGenerator, params object[] args) {
      try {
        string key = GetDebugString(keyGenerator);
        string logMe = CommonDebug.CombineDebugString(args);
        bool really = true;
        if (_LastLogsForKeys.ContainsKey(key) && _LastLogsForKeys[key] == logMe) {
            really = false;
        }
        if (really) {
          _LastLogsForKeys[key] = logMe;
          CommonDebug.LogLine(key, logMe);
        }
      } catch {

      }
    }
    public static void LogLinePerSecond(params Object[] args) {
      CommonDebug.LogColoredPerSecond(-1, args);
    }
    public static void LogLineIf(bool condition, params Object[] args) {
      if (condition) {
        CommonDebug.LogLine(args);
      }
    }
    /// <summary>Identical to calling CommonDebug.LogLine.
    /// Reason for this method is that sometimes we want to set a 
    /// breakpoint in CommonDebug.LogLine, but we want some calls not to hit it.</summary>
    [Conditional("DEBUG")]
    public static void WriteAndIgnore(params object[] args) {
      string combineArgs = CommonDebug.CombineDebugString(args);
      if (CommonDebug._WriteLine != null) {
        CommonDebug._WriteLine(combineArgs);
      }
    }
    public static bool LogLineOnce(params Object[] args) {
      bool r = false;
#if DEBUG
      string logMe = CommonDebug.CombineDebugString(args);
      r = CommonDebug.WriteLineOnce(logMe);
#endif
      return r;
    }
    public static bool CheckCoordinates(params float[] list) {
      bool allOK = FloatAdditions.AllFinite(list);
      if (allOK) {
        return true;
      } else {
        string errorMessage = "Invalid coordinates:";
        foreach (float x in list) {
          errorMessage += (" " + x.ToString());
        }
        CommonDebug.BreakPointLog(errorMessage);
        return false;
      }
    }
    /// <summary>If you need a debug-related timer to stick around, put it here.</summary>
    public static List<Jockusch.Common.Timers.Timer> Timers { get; set; } = new List<Jockusch.Common.Timers.Timer>();
    [Conditional("DEBUG")]
    public static void Assert(bool condition) {
      if (!condition) {
        CommonDebug.BreakPoint();
      }
    }
    public static bool ReturningAssert(bool condition) {
      if (!condition) {
        CommonDebug.BreakPoint();
      }
      return condition;
    }
    public static void Assert(Func<bool> function) {
      bool ok = function();
      if (!ok) {
        bool tryAgain = false; // change this in the debugger to evaluate the function again.
        CommonDebug.BreakPoint();
        if (tryAgain) {
          CommonDebug.Assert(function);
        }
      }
    }
    public static void AssertLessThan(int x, int y, int z) {
      CommonDebug.Assert(x < y);
      CommonDebug.Assert(y < z);
    }
    public static void AssertLessThanOrEqual(int x, int y, int z) {
      CommonDebug.Assert(x <= y);
      CommonDebug.Assert(y <= z);
    }
    public static string DebugString = "";
    public static void AppendToDebugString(string appendMe, bool onlyOnce = false) {
      bool reallyAppend = !onlyOnce || !CommonDebug.DebugString.EndsWithInvariant(appendMe);
      if (reallyAppend) {
        CommonDebug.DebugString = CommonDebug.DebugString + appendMe;
        CommonDebug.LogLine(appendMe);
        WJNotificationCenter.DefaultCenter.Post(NotificationKey.DebugStringAppend, null, appendMe);
      }
    }
    public static void DebugLogToScreen(string text) {
      WJNotificationCenter center = WJNotificationCenter.DefaultCenter;
      center.Post(NotificationKey.DebugLogToScreenRequest, text, text);
    }

    public static void WithoutBreaking(Action a) {
#if DEBUG
      CommonDebug.IncrementBreakCount();
      a();
      CommonDebug.DecrementBreakCount();
#else
      a();
#endif
    }

#region event recording

    /// <summary> Keys are object names.  Values are events for the object. </summary>
    public static Dictionary<string, string> ObjectEventDictionary { get; set; } = new Dictionary<string, string>();
    [Conditional("DEBUG")]
    public static void RecordObjectEvent(object forObject, params object[] notes) {
      string eventText = CommonDebug.CombineDebugString(notes);
      Dictionary<string, string> dict = CommonDebug.ObjectEventDictionary;
      string key = ObjectNamer.NameForObject(forObject);
      string existingValue = dict.GetValueOrDefault(key);
      string newValue = (existingValue == null) ? eventText : existingValue + "\n" + eventText;
      dict[key] = newValue;
    }
    [Conditional("DEBUG")]
    public static void LogEvents(object forObject) {
      string name = ObjectNamer.NameForObject(forObject);
      string value = CommonDebug.ObjectEventDictionary.GetValueOrDefault(name);
      if (value.IsNonempty()) {
        CommonDebug.LogLine("Events for", CommonDebug.GetDebugString(forObject)+":");
        CommonDebug.Indent();
        CommonDebug.LogIndentAllLines(value);
        CommonDebug.Outdent();
      } else {
        CommonDebug.LogLine("No events for", forObject);
      }
    }
    public static void NoteException(Exception e) {
      try {
        string debug = e.EverythingDebugString();
#if DEBUG
        CommonDebug.BreakPoint(debug);
#else
        CommonDebug.RecordOperation(debug);
#endif
      }
      catch {
        // app may no longer be running . . .
      }
    }

#endregion


#region reflection

    public static void LogWJTypeTree(Type root) {
      TypeTree tree = new TypeTree(root);
      List<Assembly> assemblies = CommonDebug.WJAssemblies;
      foreach (Assembly assembly in assemblies) {
        IEnumerable<TypeInfo> infos = assembly.DefinedTypes;
        foreach (TypeInfo info in infos) {
          tree.AddType(info);
        }
      }
      tree.LogStringTree(null, "", false);
    }
    private static List<Assembly> _WJAssemblies { get; set; } = new List<Assembly>();
    public static List<Assembly> WJAssemblies {
      get {
        CommonDebug.RegisterWJAssemblyContainingType(typeof(CommonDebug));
        return _WJAssemblies;
      }
    }
    public static void RegisterWJAssembly(Assembly assembly) {
      if (!_WJAssemblies.Contains(assembly)) {
        _WJAssemblies.Add(assembly);
      }
    }
    public static void RegisterWJAssemblyContainingType(Type type) {
      Assembly assembly = type.GetTypeInfo().Assembly;
      CommonDebug.RegisterWJAssembly(assembly);
    }

#endregion
    /// <summary>Calling Environment.NewLine sometimes hangs the debugger . . . </summary>
    public const string NewLine = StringConstants.Newline;
    // just a way to keep strong references around.
    public static List<Timer> DebugTimers { get; } = new List<Timer>();
    private static ConcurrentDictionary<string, int> MethodEnterExitCounts { get; } = new ConcurrentDictionary<string, int>();
    [Conditional("DEBUG")]
    public static void EnterMethod(object sender = null,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "") {
      string key = sourceFilePath + "." + memberName;
      int value = MethodEnterExitCounts.AddOrUpdate(key, 1, (_, i) => i + 1);
      CommonDebug.LogLine(sender ?? "", memberName, value);
    }
    [Conditional("DEBUG")]
    public static void ExitMethod(object sender = null,
              [System.Runtime.CompilerServices.CallerMemberName]
    string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "") {
      string key = sourceFilePath + "." + memberName;
      int count = MethodEnterExitCounts.AddOrUpdate(key, -1, (_, i) => i - 1);
      CommonDebug.LogLine(sender ?? "", "-"+memberName, count);
    }
  }

  public static class CommonDebugAdditions
  {
    public static string GetLastLog() {
      return CommonDebug.DebugConsoleLines.Last()?.Text;
    }
    public static string GetAllLogs() {
      string r = "";
      bool first = true;
      foreach (AttributedString line in CommonDebug.DebugConsoleLines) {
        if (first) {
          first = false;
        } else {
          r += CommonDebug.NewLine;
        }
        r += line;
      }
      return r;
    }
  }
}

