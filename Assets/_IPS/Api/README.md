# IPS `Api` PACKAGE
> Last Update: 26/03/2025
--------------------------

### Menu IPS/ApiSettings

- Turn on all 3rd which project will be used
- Download newest dependencies packages from [Google Packages](https://developers.google.com/unity/archive#external_dependency_manager_for_unity) then paste into project folder/Google Packages

    + Firebase docs: https://firebase.google.com/docs/unity/setup


### Packages/manifest.json

- Add into dependency
        "com.google.external-dependency-manager": "file:../GooglePackages/com.google.external-dependency-manager-1.2.185.tgz",
        "com.google.firebase.analytics": "file:../GooglePackages/com.google.firebase.analytics-12.6.0.tgz",
        "com.google.firebase.app": "file:../GooglePackages/com.google.firebase.app-12.6.0.tgz",
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
        "com.applovin.mediation.ads": "8.1.0",


- Add into Scope:
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


--------------------------
# CHANGE LOGS

27/05/2024:
- [Api.Ads] Change adType from string to enum AdSlotFormat, add placement, adType for allmost ad callback
- [Api.Ads] Fix banner mediation fallback if admob not available.
- [Api.Ads] Add script `AdTestSuiteButton`: Show TestSuite for Ironsource
- [Api.Ads] Add `BannerHeightEstimate` for prepare region UI for banner, add OnAdFailedToLoad for all adType
- [Api.Analytics] Add FalconCustomTrackingEvent
- [Api.Analytics] Add ByteBrew analytics
- [Api.Analytics] Add event OnLowMemory

08/05/2024:
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

25/04/2024:
- [Api.Ads] New banner `collapsible` logic with pop-up interval, control by remote config
- [Api.Ads] Mediation fallback logic for banner, mrec, interstitial, rewarded video.
- [Api.Ads] Use script `NativeAdAutoSpawn` in any where need to show native.

--------------------------
