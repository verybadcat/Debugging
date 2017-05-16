using System;

namespace Jockusch.Common
{
  /// <summary>Semi-hack to deal with the fact that OutputFormFunction is defined in a higher project.</summary>
  public interface IGetFunctionName
  {
    string FunctionName { get; }
  }
}
