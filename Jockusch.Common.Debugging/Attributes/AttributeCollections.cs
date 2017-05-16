using System;
namespace Jockusch.Common
{
  public static class AttributeCollections
  {
    public static AttributeCollection FromString (string text) {
      return new AttributeCollection(text);
    }
  }
}
