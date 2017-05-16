using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Jockusch.Common.Debugging;
using System.Diagnostics;

namespace Jockusch.Common.Timers {
  public sealed class Timer : CancellationTokenSource {
    private static int CreationCount;
    private static int DisposeCount;
    public Timer(Action callback, int millisecondsDueTime, int millisecondsPeriod = int.MaxValue,
      bool waitForCallbackBeforeNextPeriod = false): this((o) => callback(), null,
        millisecondsDueTime, millisecondsPeriod, waitForCallbackBeforeNextPeriod) {}
    [DebuggerNonUserCode]
    public Timer(Action<object> callback, object state, int millisecondsDueTime, 
      int millisecondsPeriod = int.MaxValue, bool waitForCallbackBeforeNextPeriod = false) {
      CreationCount++;
      if (CreationCount > 1) { // the first timer is created before logging is setup
//        CommonDebug.LogLine("Timer", CreationCount, DisposeCount);
      }
      Task.Delay(millisecondsDueTime, Token).ContinueWith(async (t, s) => {
        var tuple = (Tuple<Action<object>, object>) s;
        while (!IsCancellationRequested) {
          if (waitForCallbackBeforeNextPeriod)
            tuple.Item1(tuple.Item2);
          else
            #pragma warning disable 4014
            Task.Run(() => {
              #pragma warning restore 4014
              tuple.Item1(tuple.Item2);
            });
          await Task.Delay(millisecondsPeriod, Token).ConfigureAwait(false);
        }
      }, Tuple.Create(callback, state), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
    }

    private bool DisposeNoted { get; set;}

    protected override void Dispose(bool disposing) {
      if (!DisposeNoted) {
        DisposeCount++;
        DisposeNoted = true;
      }
      if (disposing)
        Cancel();
      base.Dispose(disposing);
    }
  }
}
