using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jockusch.Common {
  public enum NumberParseHandling {
    Default, // empty string to NaN
    Coefficient, // empty string => 1, "-" => -1
    EmptyToZero,
  }
}
