using System;
using Jockusch.Common;
using System.Drawing;
using Jockusch.Common.Debugging;
using System.Collections.Generic;
using System.Threading.Tasks;
using JockuschTests.Graphics;

namespace JockuschTests
{
  public class UnitTestOperatingSystem : WJOperatingSystem
  {

    internal static int CreationCount { get; set; }
    public UnitTestOperatingSystem() {
      UnitTestOperatingSystem.CreationCount++;
    }
    public static UnitTestOperatingSystem CurrentTest {
      get {
        return WJOperatingSystem.Current as UnitTestOperatingSystem;
      }
    }


    public override Screen CreateScreen(object _) {
      throw new BreakingException();
    }

 
    public override bool CanSaveToDisk() {
      return false;
    }
    public override bool ShouldDrawBitmapsWhenDrawingElements() {
      return false;
    }

    public override bool NeedsListViewItemHeightsBeforeRowCreation() {
      return true; // we sometimes look at the test elements without their native corresponding elements, so this needs to be true.
    }

    public override bool CanContextSwitchDuringDraw() {
      return true;
    }
    public override bool IsMainThread() {
      return true; // just don't worry about it for now
    }
    public override void SlightDelay(Action a, IGetNativeObject helper) {
      this.DelayedActions.Add(a); // We don't really have a concept of "the next runloop cycle". so test code has to manually call PerformDelayedActions().
    }
    public List<Action> DelayedActions = new List<Action>();
    public void PerformDelayedActions() {
      while (DelayedActions.Count > 0) {
        DelayedActions[0].Invoke();
        DelayedActions.RemoveAt(0);
      }
    }
    public static void PerformDelayed() {
      UnitTestOperatingSystem.CurrentTest.PerformDelayedActions();
    }
    public override float CursorPositioningFudgeFactor(float fontSize) {
      float r = (fontSize * 0.06f).RoundToFloat();
      return r;
    }
    public override bool IsTest() {
      return true;
    }

    protected override TextSizing CreateTextSizing() {
      return new TestTextSizing();
    }
    public override bool PerformOnMainThreadImplementation(Action a, object helper, bool complainOnFailure = true) {
      // we don't worry about threads in the test environment
      a();
      return true;
    }
    internal string CopyPasteBufferUICantSeeThis { get; set; }


  }
}

