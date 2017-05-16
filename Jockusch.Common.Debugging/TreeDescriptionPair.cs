using System;
using System.Collections.Generic;

namespace Jockusch.Common.Debugging
{
  public class TreeDescriptionPair: ITreeDescription
  {
    public string Key {get;set;}
    public ITreeDescription Value { get; set;}
    public TreeDescriptionPair(string key, ITreeDescription value)
    {
      this.Key = key;
      this.Value = value;
    }
      
    public string ToShortString() {
      string key = this.Key??"[NULL KEY]";
      string value = this.Value?.ToShortString() ?? "[NULL VALUE]";
      return key + ": " + value;
    }
    public IEnumerable<object> GetTreeDescriptionChildren() {
      return this.Value.GetTreeDescriptionChildren();
    }
    public bool RequiresMainThreadForTreeDescription() {
      return this.Value.RequiresMainThreadForTreeDescription();
    }
  }
}

