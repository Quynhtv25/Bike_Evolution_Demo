# IPS `AdsMediation` PACKAGE
> Last Update: 25/04/2024
> Description: This package contains API for using Admob Mediation or Max Mediation or IronSource Mediation (with Admob UMP): App Open Ad (AOA), Banner/MREC, Interstitial, RewardedVideo, RewardedInterstitial, Native

## 1. Installation
- Download needed `.unitypackage` file in all links below, then import into project
    - [`GoogleMobileAds`](https://github.com/googleads/googleads-mobile-unity/releases): Admob SDK package by Google.
    - [`GoogleMobile-NativeAds`](https://developers.google.com/admob/unity/native)
    - [`Adapter For Mediation`](https://bintray.com/google/mobile-ads-adapters-unity): Download adapter for mediation use by your project.

    - [`Max Mediation`](https://dash.applovin.com/documentation/mediation/unity/getting-started/integration)

    - [`IronSource Mediation`](https://developers.is.com/ironsource-mobile/unity/unity-plugin/)
    - [`IronSource Ad Quality SDK`](https://developers.is.com/ironsource-mobile/unity/sdk-integration-guides/)



- Open unity menu `IPS/Api/ApiSettings`, turn ON the option `Use Ads`, then `Save` 
- Open unity menu `IPS/Api/AdsSettings` then fill all field, remenber to click **Save**, then you can click *Open Google Settings* to make sure your config was successfully. 
    - **`IMPORTANT`:** Remember to disable two field: `UseTestID` & `EnableLog` for the **RELEASE**



- For Android: Select unity menu `Assets/External Dependency Manager/Android Resolver/Force Resolve`

## 2. CODE IMPLEMENTATION

- **Initialize:** On the first scene of the Game, create an empty object then add this script `Bootstrap` into it, then enable the **preloadAds** field, the script `AdsManager` will be initialize automatically. Or an other way you can call `AdsManager.Instance.Preload()` to manual init if you don't want to use the Bootstrap.

- **Usage:** Use `AdsManager.cs` for manage and control all ads in your game.

## EXTENDS
- **RewardAdButton** is a commponent which can auto listening when the reward video is available. You can use it for any **watch video button**

## NOTE
- Banner already had listener by SafeArea, so do not create a fake banner when building UX layout in your game.
- Interstitial/RewardedVideo/RewardedInterstitial already check has ad, so you do not need to check whether has ad or not, just call Show ads.
- Almost event about ad already contains in this lib, so do not create other tracking event for ad handler (Ask Ms.ThuongLTT before create new ad tracking)
