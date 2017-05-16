using System;

namespace Jockusch.Common.EncodingAttributeHandlingsSpecific
{
  internal class EncodingAttributeHandlingNone: EncodingAttributeHandling
  {
    public override bool ShouldEncodeAttribute(string key, GenericAttribute attribute) {
      return false;
    }
  }
}

