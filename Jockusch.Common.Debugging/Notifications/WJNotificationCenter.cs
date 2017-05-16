using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jockusch.Common.Debugging;

/* This class is not even remotely thread-safe. 
 * IMPORTANT: If you don't want to get any more notifications in your object foo, call WJNotificationCenter.DefaultCenter.RemoveObserver(foo).
 * Otherwise, foo will continue getting notifications, and bad stuff will happen.
 */
using System.Collections.Concurrent;

namespace Jockusch.Common.Notifications
{
  //Xamarin.Mac has a namespace "NotificationCenter" . . .
  public class WJNotificationCenter
  {
    /// <summary>Where most notifications go</summary>
    private static WJNotificationCenter _DefaultCenter = new WJNotificationCenter();
    public static WJNotificationCenter DefaultCenter { get { return _DefaultCenter; } }
    public WJNotificationCenter()
    {
    }

    private ConcurrentDictionary<string, HashSet<NotificationListener>> ListenerDictionary { get; } = new ConcurrentDictionary<string, HashSet<NotificationListener>>();

    public void Post(System.Object key, object sender, object info = null, bool complainIfNotHeard = false) {
      CommonDebug.NullCheck(key);
      string keyString = key.ToString();
      this.Post(keyString, sender, info, complainIfNotHeard);
    }
    public void Post(string name, object sender, object info = null, bool complainIfNotHeard = false) {
      Notification n = new Notification(name, sender, info);
      this.Post(n, complainIfNotHeard);
    }
    public void AddObserver(object observer, System.Object key, Action<Notification> action) {
      this.AddObserver(observer, key.ToString(), action);
    }
    /// <summary>Only adds if we don't already have an observer for the object and key.</summary>
    public void ConditionallyAddObserver(object observer, string key, Action<Notification> action)
    {
      if (!this.HasObserver(observer, key)) {
        this.AddObserver(observer, key, action);
      }
    }
    /// <summary>Only adds if we don't already have an observer for the object and key.</summary>
    public void ConditionallyAddObserver(object observer, System.Object key, Action<Notification> action) {
      this.ConditionallyAddObserver(observer, key.ToString(), action);
    }

    public void ConditionallyAddObserver(object observer, System.Object key, Action action) {
      this.ConditionallyAddObserver(observer, key, n => action());
    }

    public void ConditionallyAddObserver(object observer, string key, Action action) {
      this.ConditionallyAddObserver(observer, key, n => action());
    }

    public void AddObserver(object observer, string name, Action<Notification> action) {
      this.GetListenersForName(name).Add(new NotificationListener(name, observer, action));
      #if DEBUG
      int n = this.NListeners(false);
      if (n % 500 == 0) {
        // Not the end of the world, but let's take a look.
        this.NListeners(true);
        if (!WJOperatingSystem.Current.IsTest()) {
          CommonDebug.BreakPoint();
        }
      }
      #endif
    }
    public void AddObserver(object observer, string name, Action action) {
      this.AddObserver(observer, name, n => action());
    }
    public void AddObserver(object observer, System.Object name, Action action) {
      this.AddObserver(observer, name, n => action());
    }
    public void RemoveObserver(object observer) {
      ConcurrentDictionary <string, HashSet<NotificationListener>> listenerDictionary = this.ListenerDictionary;
      //Saw a weird crash I didn't understand in the ToList() call on iOS. But if I take it out, and
      // work directly on the dictionary, I get a mutation error in tests.
      //Trying .ToArray() instead of .ToList per SO: http://stackoverflow.com/questions/11692389/getting-argument-exception-in-concurrent-dictionary-when-sorting-and-displaying
      KeyValuePair<string, HashSet<NotificationListener>>[] array = listenerDictionary.ToArray();
      foreach (KeyValuePair<string, HashSet<NotificationListener>> pair in array) {
        HashSet<NotificationListener> value = pair.Value;
        foreach (NotificationListener notificationListener in value.Where(nl => nl.Listener == observer).ToList()) {
          //We need this in case the post method is in progress.
          notificationListener.Killed = true;
        }
        string key = pair.Key;
        this.RemoveDeadListeners(key);
      }
    }
    /// <summary>
    /// Only removes the observer if both the object and the name match
    /// </summary>
    /// <param name="observer">Observer.</param>
    /// <param name="name">Name.</param>
    public void RemoveObserver(object observer, string name) {
      if (this.ListenerDictionary.ContainsKey(name)) {
        HashSet<NotificationListener> listenerSet = this.ListenerDictionary[name];
        foreach (var notificationListener in listenerSet.Where(nl => nl.Listener == observer)) {
          //We need this in case the post method is in progress.
          notificationListener.Killed = true;
        }
        this.RemoveDeadListeners(name);
      }
    }
    public void RemoveObserver(object observer, System.Object key) {
      this.RemoveObserver(observer, key.ToString());
    }
    private static bool ListenerHasObserver(HashSet<NotificationListener> listeners, object possibleObserver, int maxAllowedRetries = 5) {
      if (listeners != null) {
        try {
          foreach (var notificationListener in listeners.Where(nl => nl.Listener == possibleObserver)) {
            if (notificationListener.IsLive) {
              return true;
            }
          }
        } catch {
          if (maxAllowedRetries == 0) {
            throw new BreakingException();
          } else {
            return ListenerHasObserver(listeners, possibleObserver, maxAllowedRetries - 1);
          }
        }
      }
      return false;
    }
    public bool HasObserver(object possibleObserver) {
      foreach (var kvp in this.ListenerDictionary.ToList()) {
        HashSet<NotificationListener> value = kvp.Value;
        if (WJNotificationCenter.ListenerHasObserver(value, possibleObserver)) {
          return true;
        }
      }
      return false;
    }
    public bool HasObserver(object possibleObserver, string key) {
      HashSet<NotificationListener> listeners = this.ListenerDictionary.GetValueOrDefault(key);
      bool r = WJNotificationCenter.ListenerHasObserver(listeners, possibleObserver);
      return r;
    }

    public bool HasObserver(object possibleObserver, System.Object key) {
      return this.HasObserver(possibleObserver, key.ToString());
    }
    private int NListeners(bool logCounts) {
      ConcurrentDictionary<string, HashSet<NotificationListener>> dict = this.ListenerDictionary;
      int r = 0;
      foreach (KeyValuePair<string, HashSet<NotificationListener>> pair in dict) {
        int count = pair.Value.Count;
        r += count;
        if (logCounts) {
          CommonDebug.LogLine(pair.Value.Count, "listeners for key", pair.Key);
          if (count > 100) {
            CommonDebug.LogEnumerable(pair.Value);
          }
        }
      }
      return r;
    }
    #if DEBUG
    private int _ExceptionIgnoreCount = 0;
    #endif
    private void HandleException() {
      #if DEBUG
      _ExceptionIgnoreCount++;
      CommonDebug.BreakPointIf(_ExceptionIgnoreCount % 5 == 0);
      #endif
    }
    private void RemoveDeadListeners(string name) {
      try {
        ConcurrentDictionary<string, HashSet<NotificationListener>> listenerDict = this.ListenerDictionary;
        HashSet<NotificationListener> listeners = listenerDict.GetValueOrDefault(name);
        if (listeners != null) {
          listeners.RemoveWhere(n => !n.IsLive);
          if (!listeners.Any()) {
            HashSet<NotificationListener> killed;
            listenerDict.TryRemove(name, out killed);
          }
        }
      } catch {
        // Probably a concurrency problem, We can just ignore it; nothing urgent about removing dead listeners. In debug mode, we have a check to see if this is happening a lot.
        this.HandleException();
      }
    }
    private HashSet<NotificationListener> GetListenersForName(string name) {
      HashSet<NotificationListener> r;
      this.RemoveDeadListeners(name);
      var listeners = this.ListenerDictionary;
      if (listeners.ContainsKey(name)) {
        r = listeners[name];
      } else {
        r = new HashSet<NotificationListener>();
        listeners[name] = r;
      }
      return r;
    }
    public void Post(Notification notification, bool complainIfNotHeard = false) {
      string name = notification.Name;
      bool complain = complainIfNotHeard;
      if (this.ListenerDictionary.ContainsKey(name)) {
        List<NotificationListener> listeners = this.ListenerDictionary[name].ToList();
        foreach (NotificationListener listener in listeners) {
          //We need to check if it's live in case a previous notification killed it
          if (listener.IsLive) {
            listener.Notify(notification);
            complain = false;
          }
        } 
      }
      if (complain) {
        CommonDebug.BreakPoint();
      }
    }
//    private void PostOnMainThreadDebugOnly(Notification notification) {
//      // At least on Android, any type of centralized thread-switching (such as this) carries a risk of crashes.
//      this.Post(NotificationKey.PostOnMainThreadDebuggingPurposesOnly, this, notification);
//    }
//    public void PostOnMainThreadDebugOnly(System.Object key, object sender, object info) {
//      string keyString = key.ToString();
//      Notification n = new Notification(keyString, sender, info);
//      this.PostOnMainThreadDebugOnly(n);
//    }
    // internalsVisibleTo is not working, so made this public.
    public void RemoveAllObserversTestingPurposesOnly() {
      this.ListenerDictionary.Clear();
    }
  }
}
