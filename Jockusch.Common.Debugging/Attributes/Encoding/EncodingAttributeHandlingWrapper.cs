using System;

namespace Jockusch.Common.EncodingAttributeHandlingsSpecific
{
  public class EncodingAttributeHandlingWrapper: EncodingAttributeHandling
  {
    public override bool ShouldEncodeAttribute(string key, GenericAttribute attribute) {
      switch (key) {
        case AttributeKeys.DISPLAY_STRING_KEY:
          return true;
        default:
          return false;
      }
    }
    public EncodingAttributeHandlingWrapper()
    {
    }
  }
}

