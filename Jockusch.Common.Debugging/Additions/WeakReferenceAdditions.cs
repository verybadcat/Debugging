using System;

namespace Jockusch.Common
{
  public static class WeakReferenceAdditions {
    /// <summary> returns null if target is not available. Safe to call, even if the reference is null. </summary>
    public static TTarget TryGetTarget<TTarget> (this WeakReference<TTarget> reference) where TTarget: class 
    {
      TTarget r = null;
      if (reference != null) {
        reference.TryGetTarget(out r);
      }
      return r;
    }
    public static WeakReference<TTarget> CreateOrUpdate<TTarget>(TTarget target, WeakReference<TTarget> recycleMe = null) 
      where TTarget: class {
      if (recycleMe == null) {
        recycleMe = new WeakReference<TTarget>(target);
      } else {
        recycleMe.SetTarget(target);
      }
      return recycleMe;
    }
  }
}
