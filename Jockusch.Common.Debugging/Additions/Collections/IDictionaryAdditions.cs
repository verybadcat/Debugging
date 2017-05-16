using System;
using System.Collections.Generic;
using Jockusch.Common.Debugging;
using System.Linq;

namespace Jockusch.Common
{
  public static class IDictionaryAdditions
  {
    /// <summary>Should be null-safe, (but not thread-safe)</summary> 
    public static void SetValue<T>(this IDictionary<string, T> dict, string key, T value, bool removeIfDefault = true) {
      if (dict != null && key != null) {
        if (removeIfDefault && AnyType.EqualsDefault(value)) {
          dict.RemoveIfPresent(key);
        } else {
          dict[key] = value;
        }
      }
    } 
    public static bool RemoveIfPresent<TValue>(this IDictionary<string, TValue> dict, string key) {
      // As long as we are on a single thread, this method is safe.  It should never crash.
      bool r = false;
      if (key != null) {
        if (dict.ContainsKey(key)) {
          r = dict.Remove(key);
        }
      }
      return r;
    }
    /// <summary>Does nothing if the dictionary already contains a value for the key.  Returns true if something was added.</summary>
    public static bool AddIfAbsent<TValue>(this IDictionary<string, TValue> dict, string key, TValue value) {
      bool r = !(dict.ContainsKey(key));
      if (r) {
        dict.Add(key, value);
      }
      return r;
    }
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key) {
      TValue r = default(TValue);
      if (source!=null && key!=null && source.ContainsKey(key)) {
        r = source[key];
      }
      return r;
    }
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, 
                                                         ErrorHandling handling) {
      TValue r = default(TValue);
      if (source == null || key == null) {
        handling.HandleError();
      } else {
        if (source.ContainsKey(key)) {
          r = source[key];
        }
      }
      return r;
    }

    public static string StringForKey<TValue>(this IDictionary<string, TValue> dict, string key, string defaultValue = null) {
      string r = null;
      if (dict.ContainsKey(key)) {
        TValue value = dict[key];
        r = value.ToString();
      }
      return r;
    }



    public static object ObjectForKey<TValue>(this IDictionary<string, TValue> dict, string key, object defaultValue = null) {
      object r = defaultValue;
      if (dict.ContainsKey(key)) {
        TValue value = dict[key];
        object valueObject = value as object;
        if (valueObject != null) {
          r = valueObject;
        }
      }
      return r;
    }

    /// <summary>Encodes an empty dictionary as an empty string.  So the context needs to "know" that a dictionary is
    /// expected.<summary> 
    public static string EncodeAttributeDictionary(this IDictionary<string, string> attributeDictionary) {
      string r = "";
      bool first = true;
      foreach (KeyValuePair<string, string> pair in attributeDictionary) {
        string key = pair.Key;
        string value = pair.Value;
        r += (key + "=" + value);
        if (first) {
          first = false;
        } else {
          r += ", ";
        }
      } 
      return r;
    }

    public static Dictionary<string, object> ToStringObjectDict<T>(this Dictionary<string, T> dictionary) {
      return dictionary.ToDictionary(pair => pair.Key, pair => (object)pair.Value);
    }
  }
}

