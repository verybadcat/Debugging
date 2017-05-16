using System;
using System.Collections;
using System.Collections.Generic;

namespace Jockusch.Common.Debugging
{
  public static class ObjectNamer
  {
    #if DEBUG
    public static Dictionary<string, int> NamedObjectCountDictionary = new Dictionary<string, int>();
    // number of objects of the type that have ever been named
    public static Dictionary<string, int> ExistingObjectCountDictionary = new Dictionary<string, int>();
    // number of objects of the type of whose current existence the namer is aware.
    private static Dictionary<string, string> nameDictionary = new Dictionary<string, string>();
    #endif
    
    public static int CountOfName(string name) {
      #if DEBUG
      Dictionary <string, int> dict = ObjectNamer.NamedObjectCountDictionary;
      int r;
      if (dict.ContainsKey(name)) {
        r = dict[name];
      } else {
        r = 0;
      }
      return r;
      #else
			return 0;
      #endif
    }
     
    private static int ChangeCountOfName(string name, Dictionary<string, int> countDictionary, int delta) {
      #if DEBUG
      int r = delta;
      try {
        if (countDictionary.ContainsKey(name)) {
          int previous = countDictionary[name];
          r = previous + delta;
          countDictionary[name] = r;
        } else {
          countDictionary[name] = delta;
        }
      } catch (Exception) {
        // code is unsafe and will occasionally throw exceptions, but we don't really care as it's only debug mode.
        r = 6666;
      }
      return r;
      #else
			return 0;
      #endif
    }

    public static int IncrementNamedCountOfName(string name) {
      #if DEBUG
      int r = ObjectNamer.ChangeCountOfName(name, ObjectNamer.NamedObjectCountDictionary, 1);
      return r;
      #else
      return 0;
      #endif
    }

    public static int IncrementExistenceCountOfName(string name) {
      #if DEBUG
      int r = ObjectNamer.ChangeCountOfName(name, ObjectNamer.ExistingObjectCountDictionary, 1);
      return r;
      #else
      return 0;
      #endif
    }

    public static int DecrementExistenceCountOfName(string name) {
      #if DEBUG
      int r = ObjectNamer.ChangeCountOfName(name, ObjectNamer.ExistingObjectCountDictionary, -1);
      return r;
      #else 
      return 0;
      #endif
    }

    public static string GetTypeNameWithoutBacktick(object someObject) {
      Type type = someObject.GetType();
      string typeName = type.Name;
      int backtickIndex = typeName.IndexOf('`');
      if (backtickIndex > 0) {
        typeName = typeName.Substring(0, backtickIndex);
      }
      return typeName;
    }

    public static string CreateNameForObject(object someObject) {
      if (someObject == null) {
        return "null";
      }
      string typeName = ObjectNamer.GetTypeNameWithoutBacktick(someObject);
      #if DEBUG
      int count = ObjectNamer.IncrementNamedCountOfName(typeName);
      string r = typeName + count.ToString();
      int hash = someObject.GetHashCode();
      string dictKey = typeName + hash;
      Dictionary<string, string> nameDict = ObjectNamer.nameDictionary;
      lock (nameDict) {
        if (nameDict.ContainsKey(dictKey)) {
          CommonDebug.LogLine("Already have a name for key ", dictKey);
        } else {
          nameDict.Add(dictKey, r);
        }
      }
      #else
			string r = typeName;
      #endif
      return r;
    }
    public static string ExistingNameForObject(object someObject) {
      #if DEBUG
      try {
        if (someObject == null) {
          return "null";
        }
        string typeName = ObjectNamer.GetTypeNameWithoutBacktick(someObject);
        int code = someObject.GetHashCode();
        string key = typeName + code;
        Dictionary<string, string> dict = ObjectNamer.nameDictionary;
        string r = null;
        lock (dict) {
          if (dict.ContainsKey(key)) {
            r = dict[key];
          }
        }
        return r;
      } catch (Exception) {
        return "Failed to get name";
      }
      #else
			return null;
      #endif
    }
    public static string NameForObject(object someObject) {
      if (someObject == null) {
        return "null";
      }
      string r = ObjectNamer.ExistingNameForObject(someObject);
      if (r == null) {
        r = ObjectNamer.CreateNameForObject(someObject);
      }
      return r;
    }

    public static int NameIndexOfObject(object someObject) {
      #if DEBUG
      string name = ObjectNamer.NameForObject(someObject);
      string typeName = ObjectNamer.GetTypeNameWithoutBacktick(someObject);
      if (typeName.Length > name.Length) {
        CommonDebug.BreakPoint();
      }
      string indexString = name.Substring(typeName.Length);
      int r;
      if (!(int.TryParse(indexString, out r))) {
        r = 0;
      }
      return r;
      #else
      return 1;
      #endif
    }

    public static int IncrementExistCount(object someObject) {
      string typeName = ObjectNamer.GetTypeNameWithoutBacktick(someObject);
      int r = ObjectNamer.IncrementExistenceCountOfName(typeName);
      return r;
    }

    public static int DecrementExistCount(object someObject) {
      string typeName = ObjectNamer.GetTypeNameWithoutBacktick(someObject);
      int r = ObjectNamer.DecrementExistenceCountOfName(typeName);
      return r;
    }

    public static void LogExistences() {
      #if DEBUG
      foreach (KeyValuePair<string, int> pair in ObjectNamer.ExistingObjectCountDictionary) {
        CommonDebug.LogLine(pair.Key, pair.Value.ToString());
      }
      #endif
    }
  }
}