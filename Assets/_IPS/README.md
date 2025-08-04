# IPS FULL Package
> Last Update: `0.0.3 - 27/03/2025`
--------------------------

# GUIDELINE

## 1. API Config by menu `IPS/ApiSettings`

- Turn on all 3rd which project will be used

## 2. Import 3rd SDK
- Download newest dependencies (`.tgz` file) from [Google Packages](https://developers.google.com/unity/archive#external_dependency_manager_for_unity)
- Create a folder inside project and name it to `GooglePackages`, coppy all downloaded file above into this folder.
- Open file Packages/manifest.json, add all these lines into `dependency` block (`rename version related to the file downloaded by previous step`):

```
    "dependencies": {
        "com.google.external-dependency-manager": "file:../GooglePackages/com.google.external-dependency-manager-1.2.185.tgz",
        "com.google.firebase.app": "file:../GooglePackages/com.google.firebase.app-12.6.0.tgz",
        "com.google.firebase.analytics": "file:../GooglePackages/com.google.firebase.analytics-12.6.0.tgz",
        "com.google.firebase.crashlytics": "file:../GooglePackages/com.google.firebase.crashlytics-12.6.0.tgz",
        "com.google.firebase.messaging": "file:../GooglePackages/com.google.firebase.messaging-12.6.0.tgz",
        "com.google.firebase.remote-config": "file:../GooglePackages/com.google.firebase.remote-config-12.6.0.tgz",
        "com.google.android.appbundle": "file:../GooglePackages/com.google.android.appbundle-1.9.0.tgz",
        "com.google.play.appupdate": "file:../GooglePackages/com.google.play.appupdate-1.8.3.tgz",
        "com.google.play.assetdelivery": "file:../GooglePackages/com.google.play.assetdelivery-1.9.2.tgz",
        "com.google.play.common": "file:../GooglePackages/com.google.play.common-1.9.2.tgz",
        "com.google.play.core": "file:../GooglePackages/com.google.play.core-1.8.5.tgz",
        "com.google.play.review": "file:../GooglePackages/com.google.play.review-1.8.3.tgz",
        "appsflyer-unity-plugin": "https://github.com/AppsFlyerSDK/appsflyer-unity-plugin.git#upm",
    }
```

## 3. Import Ads SDK
- Download lastest [Google Mobile Ads Packages](https://github.com/googleads/googleads-mobile-unity/releases) then import into project. `Uncheck folder **External Dependency** and **Packages** because we have imported by .tgz file at previous step`.
- [Optional] If use Max (Applovin) mediation, continue add this line into `dependency` block:

```
        "com.applovin.mediation.ads": "8.1.0",
```

then continue add these lines into Scope (outsite `dependency` block, add `,` if need becauseof json format):

```
    "scopedRegistries": [
        {
            "name": "AppLovin MAX Unity",
            "url": "https://unity.packages.applovin.com/",
            "scopes": [
                "com.applovin.mediation.ads",
                "com.applovin.mediation.adapters",
                "com.applovin.mediation.dsp"
            ]
        }
    ]
```

## 4. SDK Key/Id Config

- Go to `IPS` from editor menu then select each 3rd to fill value config.

## 5. Finally

- Force resolve from menu `Assets/ExtenalDependency/Android/Force Resolve`
- Build via option from menu `IPS/Build`
- Test to make sure: all ads show, all events tracking dispatch, ...

------------------------

# CONTENTS:

## 1. Core
- [**`Core`**](./Core)
------------------------

## 2. Common

- [**`Common`**](./Common)
----------------------------

## 3. Third-party
### **Full Api**
- [**`Api`**](./Api)

*Require normally:*
- [**`Ads`**](./Ads)
- [**`Analytics`**](./Analytics)
- [**`RemoteConfig`**](./RemoteConfig)
- [**`PushNotifications`**](./PushNotifications)
- [**`In-app Review`**](./InAppReview)
- [**`In-app Update`**](./InAppUpdate)
- [**`In-app Purchase`**](./InAppPurchase)

*Ads [Optional]*
- [**`Admob Unity SDK Docs`**](https://developers.google.com/admob/unity/quick-start)  
- [**`Max Unity SDK Docs (Applovin)`**](https://developers.applovin.com/en/max/unity/overview/integration/)  
- [**`IronSource Unity SDK Docs`**](https://developers.is.com/ironsource-mobile/unity/unity-plugin/)  

*Other Features*
- [**`CloudStorage`**](https://github.com/IronPirateStudio/IPSLib/tree/master/_Export/CloudStorage)
- *Updating...*


-----------------------
# CHANGE LOGS
0.0.3 - 27/03/2025:
- [Api.Analytics] Add AF_ROI, Adjust and more FTUE tracking
- [Api.Ads] Add MRec Max, AOA Max, add more ad event callback and tracking. More ad remote config: banner reload capping, inter from seconds, ...
- [Api.InAppUpdate] Add In app update - IAD
- [Api.RemoteConfig] Add key config for each publisher
- [Common] Add AddressableLoader, add Button3D, update HUD with custom adjust ad banner, add SettingUI, AdsBreakUI, SFX with SoundEvent, Multiscreen, etc..
- [Core] Add BootstrapConfig, add BuildVersionInfo, Add build addressable method for BuildScript.

0.0.2 - 27/05/2024:
- [Api.Ads] Change adType from string to enum AdSlotFormat, add placement, adType for allmost ad callback
- [Api.Ads] Fix banner mediation fallback if admob not available.
- [Api.Ads] Add script `AdTestSuiteButton`: Show TestSuite for Ironsource
- [Api.Ads] Add `BannerHeightEstimate` for prepare region UI for banner, add OnAdFailedToLoad for all adType
- [Api.Analytics] Add FalconCustomTrackingEvent
- [Api.Analytics] Add ByteBrew analytics
- [Api.Analytics] Add event OnLowMemory
- [Common] Add MySortingOrder.
- [Common] Minor update HUD bgAlpha, update SafeArea onBannerHeightChanged, update NoticeText color for normal and error

0.0.1 - 08/05/2024:
- [Api.Ads] Update UMP: pause game for waiting UMP consent
- [Api.Ads] Update NativeAd: Option preload, resize prefab, show loading icon when ad not ready
- [Api.Ads] Fix ad resume show event other ad showing, hide banner on top when showing AOA
- [Api.Ads] Update remove ads: remove all except rewarded video
- [Api.Ads] Adjust banner size for adaptive or banner320x50
- [Api.Analytics] Custom event name for each publisher
- [Api.Analytics] Custom log service for special publisher's tool

04/05/2024:
- [Api.Ads] Prevent show ad resume when click to banner & when showing push notify permission request
- [Api.Ads] Request banner when call ShowBanner if autorequest=false
- [Api.Ads] Add remote config adsResumeFromPlayedTimes
- [Api.Ads] Add tracking ad show count for reward inter.
- [Common] Fix Safearea anchorMin & anchorMax in case has banner

25/04/2024:
- [Api.Ads] New banner `collapsible` logic with pop-up interval, control by remote config
- [Api.Ads] Mediation fallback logic for banner, mrec, interstitial, rewarded video.
- [Api.Ads] Use script `NativeAdAutoSpawn` in any where need to show native.
- [Common] Update SafeArea with option create mask or not, callback for banner showing or hiding
- [Common] Update `IngameLogcat`
- [Core] Update Excutor wait for realtime instead of game timeScale.
- [Core] Update Editor BuildScripts, add more menu item for fast build from local

------------------------
