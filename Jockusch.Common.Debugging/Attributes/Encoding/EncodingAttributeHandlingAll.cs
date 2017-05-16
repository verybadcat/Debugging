using System;

namespace Jockusch.Common.EncodingAttributeHandlingsSpecific
{
  internal class EncodingAttributeHandlingAll: EncodingAttributeHandling
  {
    public override bool ShouldEncodeAttribute(string key, GenericAttribute attribute) {
      return true;
    }
  }
}

