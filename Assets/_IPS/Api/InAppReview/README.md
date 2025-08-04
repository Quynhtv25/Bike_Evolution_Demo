# IPS `InAppReview` PACKAGE
> Last Update: 10/04/2024

Documentation reference:
- https://developer.android.com/guide/playcore/in-app-review/unity?hl=vi

## 1. Installation

- Open unity menu `IPS/Api/ApiSettings`, turn ON the option `Use In App Review`, then `Save` 
- Download newest `.tgz` file in all links below, paste into root project folder name `GooglePackages/`:
   - [Play In-App Review](https://developers.google.com/unity/archive#play_in-app_review)
   - [Play Core](https://developers.google.com/unity/archive#play_core)
   - [Play Common](https://developers.google.com/unity/archive#play_common)
   - [Android App Bundle](https://developers.google.com/unity/archive#android_app_bundle)
   - [External Dependency Manager for Unity](https://developers.google.com/unity/archive#external_dependency_manager_for_unity)
   

- Open file `review.package.manifest.json` coppy all line inside *dependency* block into project file `Packages/manifest.json`
    - Replace the version number by the file downloaded.
    - Check all line, if has duplicate then keep the line which has newest version.


**For Android Only:**
- Go to *BuildSettings/PlayerSettings/Publishing*, check to turn on `Custom Proguard File`.
- Open file `review-proguard.txt`, coppy all text then paste into file `Assets/Plugins/Android/proguard-user.txt`
- Select unity menu `Assets/External Dependency Manager/Android Resolver/Force Resolve`

## 2. CODE IMPLEMENTATION

Call `RatePopup.Instance.ShowIfAvailable` at any where you want to show the popup.

Example: 


    public class UIManager : MonoBehavior {
        
        private void Start() {
            RatePopup.Instance.ShowIfAvailable(); 
        }
    }
   