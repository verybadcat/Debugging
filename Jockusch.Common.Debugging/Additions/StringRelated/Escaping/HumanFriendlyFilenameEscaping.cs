using System;
using Jockusch.Common.Debugging;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jockusch.Common
{
  public class HumanFriendlyFilenameEscaping: CustomEscaping
  {
    public HumanFriendlyFilenameEscaping()
    {
    }


    private static IEnumerable<StringReplacement> _DoReplacements {
      get {
        return StringReplacements.FromPipeSeparatedStrings(
          "+|_p_",
          "[|_br_",
          ",|_cm_",
          "/|_d_",
          "!|_ft_",
          "*|_t_",
          "^|_pw_",
          "-|_m_",
          "]|_brc_",
          "\"|_q_",
          "\'|_sq_",
          @"/|_bk_",
          @"<|_lt_",
          @">|_gt_",
          $@"{StringConstants.LessThanOrEqualTo}|_leq_",
          $@"{StringConstants.GreaterThanOrEqualTo}|_geq_"
        );
      }
    }

    private static IEnumerable<StringReplacement> Replacements { get; } = HumanFriendlyFilenameEscaping._DoReplacements.ToList();

    private static IEnumerable<StringReplacement> _ReverseReplacements { get; set;}

    private static IEnumerable<StringReplacement> ReverseReplacements {
      get {
        if (_ReverseReplacements == null) { // no lock; multiple assignments would not hurt us.
          _ReverseReplacements = Replacements.ReverseReplacements();
        }
        return _ReverseReplacements;
      }
    }

    // Droid reserved: |\?*<":>+[]/'
    // Windows: \/:*?"<>|  -- a subset of the Droid list.

    protected override string DoEscape(string str)
    {
      str = str.Replace(" ", "");
      str = str.Replace("_", "");
      IEnumerable<StringReplacement> replacements = HumanFriendlyFilenameEscaping.Replacements;
      string r = replacements.ApplyAll(str);
      if(string.IsNullOrWhiteSpace(r)) {
        r = "nameless";
      }
      if (r.ToCharArray()[0].IsNumeric()) {
        r = "_" + r;
      }
      return r;
    }

    protected override string DoUnescape(string str)
    {
      if (str.StartsWithInvariant("_")) {
        str = str.Substring(1);
      }
      IEnumerable<StringReplacement> reverse = HumanFriendlyFilenameEscaping.Replacements.ReverseReplacements();
      string r = reverse.ApplyAll(str);
      return r;
    }
  }
}
