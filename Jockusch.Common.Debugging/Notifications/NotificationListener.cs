using System;
using Jockusch.Common;
using Jockusch.Common.Debugging;

namespace Jockusch.Common.Notifications {
  public class NotificationListener: IDebugString  {
    public NotificationListener(string name, object listener, Action<Notification> notify) {
      this.Name = name;
      this.Notify = notify;
      this.Killed = false;
      this.ListenerReference = new WeakReference<object>(listener);
    }

    public string Name { get; private set; }
    public bool Killed { private get; set; }
    private WeakReference<object> ListenerReference { get; set; }
    public object Listener {
      get { 
        return this.ListenerReference.TryGetTarget(); 
      }
    }
    /// <summary>
    /// A listener is considered dead if EITHER its listener is null, OR it has been explicitly killed.
    /// </summary>
    public bool IsLive { get { return this.Killed ? false : this.Listener != null; } }
    public Action<Notification> Notify { get; private set; }

    public string ToDebugString()
    {
      if (this.IsLive) {
        object listener = this.Listener;
        return "NL: => " + CommonDebug.GetDebugString(listener);
      } else {
        return "Dead NotificationListener";
      }
    }
  }
}

