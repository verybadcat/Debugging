using System;

namespace Jockusch.Common
{
  public interface IGetNativeObject
  {
    /// <summary>Returning null is OK. Expected use case is for thread switching -- Android, at least,
    /// wants a native View or Context to perform a thread switch.</summary>
    object GetNativeObject();
  }
  public static class IGetNativeObjectAdditions {
    public static bool PerformOnMainThread(this IGetNativeObject getNative, Action action, bool complainOnFailure = true) {
      WJOperatingSystem os = WJOperatingSystem.Current;
      bool r = os.PerformOnMainThread(action, getNative, complainOnFailure);
      return r;
    }
    public static void SlightDelay(this IGetNativeObject getNative, Action action) {
      WJOperatingSystem os = WJOperatingSystem.Current;
      os.SlightDelay(action, getNative);
    }
  }
}

