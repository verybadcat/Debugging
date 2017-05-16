using System;
using Jockusch.Common;
using Jockusch.Common.Debugging;

using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Jockusch.Common
{
  public abstract class WJOperatingSystem 
  {

    public virtual object ToDebugFriendlyObject(object debugMe, bool longForm) {
      return debugMe;
    }
    public virtual bool ShouldDealWithAccessibility() {
      return false;
    }
    public virtual bool MakeAccessibilityAnnouncement(string announcement) {
      return false;
    }
    /// <summary>The vast majority of code does and should not call this. But there are a few layout quirks where it really is the best way.</summary>
    public virtual bool IsApple() {
      return false;
    }
    public virtual bool IsMac() {
      return false;
    }
    public virtual bool IsIOS() {
      return false;
    }
    public virtual bool IsDroid() {
      return false;
    }

    /// <summary>Specifically Microsoft Windows, not just any windowed os.</summary>
    public virtual bool IsMSWindows() {
      return false;
    }


    public static WJOperatingSystem Current {
      get {
        return AppEnvironment.OperatingSystem;
      }
    }
    public WJOperatingSystem() {

    }
    public virtual bool UsesGraphcalcFormat() {
      return false;
    }

    public Task InitialLoadTask { get; set; }
    #region names
    public virtual string DeviceBrand() {
      return StringConstants.Unknown;
    }
    public virtual string DeviceVersion() {
      return StringConstants.Unknown;
    }

    public virtual string OSName() {
      return StringConstants.Unknown;
    }
    public virtual string OSVersion() {
      return "0";
    }
    public virtual bool CanSwitchToMainThread(IGetNativeObject helper) {
      return true;
    }
    public bool PerformOnMainThread(Action a, IGetNativeObject helper, bool complainOnFailure = true) {
      object nativeObject = helper?.GetNativeObject();
      return this.PerformOnMainThreadImplementation(a, nativeObject, complainOnFailure);
    }
    /// <summary>E.g. on Apple operating systems, a "slight delay" waits for the next iteration of the run loop.
    /// The os should make sure the action happens on the UI thread.</summary>
    public abstract void SlightDelay(Action a, IGetNativeObject helper);
    public void PerformOnMainThreadSlightDelay(Action a, IGetNativeObject helper) {
      this.SlightDelay(() => this.PerformOnMainThread(a, helper), helper);
    }
    /// <summary>This should attempt to switch to the main thread and perform the action. The returned value
    /// should indicate whether or not it expects to succeed (or isn't sure).</summary>
    public virtual bool PerformOnMainThreadImplementation(
      Action a, object nativeHelper, bool complainOnFailure = true) {
      CommonDebug.BreakPointIf(complainOnFailure);
      return false;
    }
    public virtual string InfoStringWithNewlines() {
      string brand = this.DeviceBrand();
      string model = this.DeviceVersion();
      string os = this.OSNameAndVersion();
      string newline = StringConstants.Newline;
      string r = "Brand: " + brand + newline + "Model: " + model + newline + os;
      return r;
      //       Brand:" + brandString + "\n Product: " + productString + "\n Model:" + modelString + "\n Android Version: " + androidVersionString +
    }
    public virtual string OSNameAndVersion() {
      return this.OSName() + " " + this.OSVersion();
    }
    public virtual string DeviceAndOSDescriptionWithArticles() {
      string device = this.DeviceBrandAndVersion();
      string os = this.OSNameAndVersion();
      string deviceArticle = device.GetArticle();
      string r = deviceArticle + " " + device + " running " + os;
      return r;
    }
    public virtual string DeviceBrandAndVersion() {
      return this.DeviceBrand() + " " + this.DeviceVersion();
    }
    #endregion
    public virtual bool IsMainThread() {
      throw new BreakingException("Unimplemented method!");
    }

    public virtual void Handle(Exception e) {
      e.LogAndBreak();
    }



    public virtual float BaseFontSize() {
      return 16;
    }
    public virtual bool CanContextSwitchDuringDraw() {
      return true;
    }

    public virtual bool ShouldAdjustFontsForScreenSize() {
      return true;
    }
    /// <summary>Calls to various native OS frequently return a text bounds that is actually slightly larger than the 
    /// rendered text. This causes a problem for cursor positioning; you can't just base it on the OS-returned text bounds.
    /// So each OS gets to have a fudge factor. The cursor is moved to the right by the returned amount. </summary>
    public virtual float CursorPositioningFudgeFactor(float fontSize) {
      return 0;
    }
    /// <summary>I have yet to run into an OS where NeedListViewItemHeights... is true and CanGetHeightOf... is false. The layout code depends on
    /// this, and I have no clue how it could possibly work if an OS ever does both of those things. In the case of iOS, CanGetHeightOf... is true.
    /// In the case of Android, NeedsListViewItemHeights... is false. The code deals with either of those situations.</summary>
    public virtual bool CanGetHeightOfNativelySizedElementWithoutNativeElement() {  // true on iOS; false on Android
      return this.NeedsListViewItemHeightsBeforeRowCreation();
    }
    public virtual bool NeedsListViewItemHeightsBeforeRowCreation() {  // true on iOS; false on Android
      return false;
    }
    public virtual bool IsUnhappyDrawingBlackAndWhiteTextAfterGraphics() {
      return false;
    }
    public virtual bool ShouldDrawBitmapsWhenDrawingElements() {
      return true;
    }


    public virtual bool CanSaveToDisk() {
      return true;
    }
    public virtual string GetCalculatorAdText() {
      CommonDebug.BreakPoint();
      return "";
    }
    /// <summary>If you don't need to do this, an empty implementation is fine.</summary>
    public virtual void DebugLogViewHierarchy() {

    }
    public virtual bool IsTest() {
      return false;
    }

    public virtual bool SuggestGC() {
      return false;
    }
    protected internal virtual bool DoPersistUpgradeState(string upgradeName, bool toValue) {
      return false;
    }
    public virtual FuzzyBool GetUpgradeState(string upgradeName) {
      // Some OS (i.e. Apple) know the upgrade state  from platform code. Others (i.e. Droid) don't.
      return FuzzyBool.Maybe;
    }
    /// <summary>Returns true if the os knows how to update the idle timer.</summary>
    protected virtual TextSizing CreateTextSizing() {
      throw new BreakingException(); // override
    }
    private TextSizing _TextSizing { get; set; }
    public TextSizing TextSizing {
      get {
        if (_TextSizing == null) {
          _TextSizing = this.CreateTextSizing();
        }
        return _TextSizing;
      }
    }
    public static TextSizing TS {
      get {
        WJOperatingSystem os = WJOperatingSystem.Current;
        TextSizing r = os.TextSizing;
        return r;
      }
    }
    #region startup

    public abstract Screen CreateScreen(object platformContext);
    /// <summary>Some platforms can startup things without any context object. Others (hello, Android) are
    /// more annoying and need one. </summary> 

    public virtual bool CanDrawOffScreen() {
      return true;
    }
    /// <summary>A null return value indicates that the OS does not know how
    /// to draw off-screen. Calls to this MUST be paired with
    /// a call to EndOffscreenDrawing or the native OS may become
    /// very confused. You'll want it anyway to get your image.</summary>


    #endregion // startup

    public virtual FuzzyBool ExternalKeyboardIsAttached() {
      return FuzzyBool.Maybe;
    }

    public virtual FuzzyBool IsMobileFuzzy() {
      return FuzzyBool.Maybe;
    }

    /// <summary>If it can, the factory should cause the native view to receive touches
    /// anywhere within the given thickness of its boundary. Pass in ThicknessF.NaN
    /// to return to the initial touch region. Pass in a negative thickness to shrink the touch region.</summary>

    /// <summary>For non-reference stuff</summary>
    public virtual string GenericCssPath() {
      return null;
    }

    /// <summary>iOS is smart about scroll touches and we can handle touches more or less normally inside of
    /// scroll views. Android is not. If we write our own scroll view touch smartness, we may be able to get rid of this.</summary>
    public virtual bool IsSmartAboutScrollTouches() {
      return false;
    }
    public virtual bool SetExcludeFromBackup(string path, bool exclude) {
      return false;
    }
    protected virtual string CapitalizedTapVerb() {
      return "Tap";
    }
    ///  

    public string TapVerb(bool capitalized) {
      string r = this.CapitalizedTapVerb();
      if (!capitalized) {
        r = r.ToLowerInvariant();
      }
      return r;
    }
    public virtual bool GenerallyUsesMouse() {
      return false;
    }
    public virtual bool CanReadFilesSynchronously() {
      return false;
    }
    /// <summary>For now, this could return null. IDK if it should stay that way
    /// or not.</summary>
    public virtual object GetActiveWindow() {
      return null;
    }

    public virtual bool ShowingFilePathsToUsersIsKosher() {
      return true;
    }

    public virtual bool CanDisplayMenuTooltips() {
      return true;
    }

  }
  public static class OperatingSystemAdditions {
    public static bool PersistUpgradeState(this WJOperatingSystem os, string upgradeName, bool toValue)
    {
      bool r = false;
#if DEBUG
      r = os.DoPersistUpgradeState(upgradeName, toValue);
#else
      if (toValue) {
        r = os.DoPersistUpgradeState(upgradeName, toValue);
      }
#endif
      return r;
    }


    public static bool IsDefinitelyMobile(this WJOperatingSystem os) {
      return os.IsMobileFuzzy().IsTrue();
    }
  }
}

