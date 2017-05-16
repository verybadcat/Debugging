using System;
using Jockusch.Common.Debugging;

namespace Jockusch.Common.Notifications {
  public class Notification {
    public string Name { get; private set; }
    public object Sender { get; private set; }
    public object Info { get; private set; }
    public Notification(string name, object sender, object info = null) {
      this.Name = name;
      this.Sender = sender;
      this.Info = info;
    }
  }
}

