# IPS `Analytics` PACKAGE
> Last Update: 19/04/2024

## 1. Installation

### **`FOR ALL:`**  
 - Download the latest `.tgz` file in links below, paste into root project folder name `GooglePackages/`:
    - [External Dependency Manager for Unity](https://developers.google.com/unity/archive#external_dependency_manager_for_unity)
    

- When finish all of setup, select unity menu `Assets/External Dependency Manager/Android Resolver/Force Resolve`

    
### *1.1 Firebase*
[*(Documentation)*](https://firebase.google.com/docs/analytics/unity/start?authuser=0)

- Asking PM to get file `google-service.json` (for Android) & `GoogleService-Info.plist` (for iOS), place all under `Assets/` folder.
- Download the latest `.tgz` file in all links below, paste into root project folder name `GooglePackages/`:
   - [Firebase App Core](https://developers.google.com/unity/archive#firebase_app_core)
   - [Firebase Crashlytics](https://developers.google.com/unity/archive#firebase_crashlytics)
   - [Google Analytics for Firebase](https://developers.google.com/unity/archive#google_analytics_for_firebase)
   

- Open file `firebase.package.manifest.json` coppy all line inside *dependency* block into project file `Packages/manifest.json`
    - Replace the version number by the file downloaded.
    - Check all line, if has duplicate then keep the line which has newest version.
- Open unity menu `IPS/Api/ApiSettings`, turn ON the option `Use Firebase`, then `Save` 

`Note`: If your console show the error about "Could not resolve GoogleIOSResolver...", then return to UnityHub, install `iOS` support feature (You can ignore this error messsage in the editor, but need to fix for the release version).


### *1.2 AppsFlyer*
[*(Documentation)*](https://dev.appsflyer.com/hc/docs/installation)

- Open file `appsflyer.package.manifest.json` coppy all line inside *dependency* block into project file `Packages/manifest.json`
- Open unity menu `IPS/Api/ApiSettings`, turn ON the option `Use AppsFlyer`, then `Save` 
- Open menu `IPS/Api/Analytics/AppsFlyerSettings`, fill the selected object.

### *1.3 Facebook* 
[*(Documentation)*](https://developers.facebook.com/docs/unity/)

- Download the latest version in the link below, then import into project (`Don't select "ExternalDependencyManager" and "Packages" when importing`)
    - [Facebook Unity SDK](https://developers.facebook.com/docs/unity/downloads)


- Open unity menu `IPS/Api/ApiSettings`, turn ON the option `Use Facebook`, then `Save` 
- Open menu `IPS/Api/Analytics/FacebookSettings` or `Facebook/EditSettings`, fill all field of selected object, then click `Regenerate Manifest`


[Android Only]
- Go to *BuildSettings/PlayerSettings/Publishing*, check to turn on `Custom Proguard File`.
- Open file `facebook-proguard.txt`, coppy all text then paste into file `Assets/Plugins/Android/proguard-user.txt`


## 2. CODE IMPLEMENTATION

- Call `Tracking.Instance.LogEvent` at any where you want to push your custom event to cloud.
- Create new script with partial class of `Tracking` if you want to write more custom LogEvent.
 
  