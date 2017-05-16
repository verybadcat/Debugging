using System;
using System.Reflection;

namespace Jockusch.Common
{
  public static class TypeInfoAdditions
  {
    public static bool IsDescendantOf(this TypeInfo info, TypeInfo possibleAncestor) {
      if (info == possibleAncestor) {
        return true;
      } else {
        Type baseType = info.BaseType;
        TypeInfo baseInfo = baseType?.GetTypeInfo();
        if (baseInfo == info) {
          return false;
        }
        if (baseInfo == null) {
          return false;
        }
        bool r = baseInfo.IsDescendantOf(possibleAncestor);
        return r;
      }
    }
  }
}

