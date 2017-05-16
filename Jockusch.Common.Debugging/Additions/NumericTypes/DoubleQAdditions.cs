using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using numericType = System.Double;
using numericTypeQ = System.Nullable<System.Double>;

namespace Jockusch.Common {
  public static class DoubleQAdditions {
    public static numericTypeQ TryParse(string s) {
      double dub;
      double? r = null;
      if (s?.Trim() == "") {
        r = 0;
      } else {
        bool success = numericType.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out dub);
        if (success) {
          r = dub;
        }
      }
      return r;
    }
  }
}
