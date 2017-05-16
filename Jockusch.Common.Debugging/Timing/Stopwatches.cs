using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jockusch.Common.Debugging
{
  public static class Stopwatches
  {
    public static Dictionary<string, Stopwatch> Watches
    {
      get; set;
    } = new Dictionary<string, Stopwatch>();
    public static Stopwatch GetInstance(object index)
    {
      string str = index.ToString();
      Stopwatch r;
      if (Watches.ContainsKey(str))
      {
        r = Watches[str];
      } else
      {
        r = new Stopwatch();
        Watches[str] = r;
      }
      return r;
    }
    public static void Start(object index)
    {
      Stopwatch watch = Stopwatches.GetInstance(index);
      watch.Start();
    }
    public static void Stop(object index)
    {
      Stopwatch watch = Stopwatches.GetInstance(index);
      watch.Stop();
    }
    public static int GetMs(object index)
    {
      Stopwatch watch = Stopwatches.GetInstance(index);
      int r = watch.ElapsedMilliseconds.ToInt();
      return r;
    }

    public static void Log(object index)
    {
      Stopwatch watch = Stopwatches.GetInstance(index);
      watch.Log(index.ToString());
    }
    public static void Log(this Stopwatch watch, string key)
    {
      long ms = watch.ElapsedMilliseconds;
      CommonDebug.LogLine(key, ms);
    }
  }
}
