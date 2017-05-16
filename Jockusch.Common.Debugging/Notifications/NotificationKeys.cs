using System;

namespace Jockusch.Common.Notifications {

  public enum NotificationKey {
    AvailableReferenceDownloadsChanged,
    AvailableReferencesChanged,
    AvailableSaveFilesChanged,
    AppVersionChanged,
    /// <summary>I.e, data for the overall window</summary>
    CalculatorDataReloaded,
    /// <summary>Includes reloads.</summary>
    CalculatorContextUILoadedDebugAndTestPurposesOnly,
    /// <summary>Any input operation in progress needs to wrap up. Send when a screen is about to pop, for example.</summary>
    CompleteUserInputRequest, 
    ConverterSettingsChanged,
    DebugLogToScreenRequest,
    /// <summary>DebugStringAppend is for debug stuff that can't be logged, i.e. in release mode.</summary>
    DebugStringAppend,
    DebugTextLogged,
    FontSizesChanged,
    GetMathrefsFailed,
    EquationsTabModeChanged,
    IsNotPro,
    MainKeyboardShouldRecreate,
		PurchaseRequest,
		PerformProUpgrade,
    PushScreenRequest,
    PopScreenRequest,
    PopToBookmarkRequest,
    ReportCrashRequest,
    ScrollerVisibilityChanged,
    SettingsCacheInvalidated,
    SettingsTabBarTypeChanged,
    StatsPlotTypeChanged,
    StressTestKeypressRequest,
    GlobalStoreLoadCompleted, // Stores which are not global should not be posting global notifications. Instead, pass in a completion action.
    TableSettingsChanged,
    TestingPurposesAngleModeRequest,
    UnitConverterModelChanged,
    UpgradePurchased,
    UserEquationChanged,
    UpdateTitleBarRequest,
    WindowContextIsReady,
  }
}
