using System;
using System.Linq;
using System.Collections.Generic;

namespace Jockusch.Common
{
  public class ReplacementListBasedCustomEscaping: CustomEscaping
  {
    public List<StringReplacement> Replacements { get; set;}
    public List<StringReplacement> ReversedReplacements { get; set;}
    public ReplacementListBasedCustomEscaping(IEnumerable<StringReplacement> replacements)
    {
      this.Replacements = replacements.ToList();
      this.ReversedReplacements = replacements.ReverseReplacements().ToList();
    }
    protected override string DoEscape(string str)
    {
      string r = this.Replacements.ApplyAll(str);
      return r;
    }
    protected override string DoUnescape(string str)
    {
      string r = this.ReversedReplacements.ApplyAll(str);
      return r;
    }
  }
}
