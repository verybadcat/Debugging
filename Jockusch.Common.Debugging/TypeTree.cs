using System;
using Jockusch.Common.Debugging;
using System.Collections.Generic;
using System.Reflection;

namespace Jockusch.Common
{
  public class TypeTree: ITreeDescription
  {
    public TypeTree(Type local)
    {
      this.LocalType = local;
      this.LocalTypeInfo = local.GetTypeInfo();
    }

    public TypeTree(TypeInfo info) {
      this.LocalTypeInfo = info;
    }

    public bool AddType(TypeInfo info) {
      bool r = false;
      TypeInfo localInfo = this.LocalTypeInfo;
      if (info.IsDescendantOf(localInfo)) {
        if (localInfo.FullName!=info.FullName) {
          Type baseType = info.BaseType;
          TypeInfo baseInfo = baseType.GetTypeInfo();
          this.AddType(baseInfo);
          List<TypeTree> children = this.ChildTypeTrees;
          foreach (TypeTree child in children) {
            if (child.LocalTypeInfo.FullName == info.FullName) {
              return false;
            }
            if (child.AddType(info)) {
              return true;
            }
          }
          if (baseInfo.FullName == localInfo.FullName) {
            TypeTree newChild = new TypeTree(info);
            this.ChildTypeTrees.Add(newChild);
            this.ChildTypeTrees.Sort(((tree1, tree2) => tree1.LocalTypeInfo.Name.CompareTo(tree2.LocalTypeInfo.Name)));
          }
          r = true;
        }
      }
      return r;
    }

    public Type LocalType {get;set;}
    public TypeInfo LocalTypeInfo {get;set;}
    public List<TypeTree> ChildTypeTrees {get;set;} = new List<TypeTree>();

    #region ITreeDescription implementation

    public string ToShortString() {
      TypeInfo info = this.LocalTypeInfo;
      string r = info.Name;
      return r;
    }

    public IEnumerable<object> GetTreeDescriptionChildren() {
      return this.ChildTypeTrees;
    }

    public bool RequiresMainThreadForTreeDescription() {
      return false;
    }

    #endregion
  }
}

