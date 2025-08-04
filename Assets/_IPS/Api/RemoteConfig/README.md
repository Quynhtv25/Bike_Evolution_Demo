# IPS `Remote Config` PACKAGE
> Last Update: 25/04/2024

## 1. Installation
[*(Documentation)*](https://firebase.google.com/docs/remote-config?authuser=0)

- Install **`IPS.Analytics.Firebase`** first for require.
- Download the latest `.tgz` file in link below, paste into root project folder name `GooglePackages/`:
   - [Firebase Remote Config](https://developers.google.com/unity/archive#firebase_remote_config)


- Open file `remoteconfig.package.manifest.json` coppy all line inside *dependency* block into project file `Packages/manifest.json`
    - Replace the version number by the file downloaded.
- Open unity menu `IPS/Api/ApiSettings`, turn ON the option `Use Remote Config`, then `Save` 


## 2. CODE IMPLEMENTATION

Fetch the settings from Firebase Remote Config in the cloud (call once at the Awake/Start only):

           RemoteConfig.Instance.GetBoolAsync...
           RemoteConfig.Instance.GetLongAsync...
           RemoteConfig.Instance.GetStringAsync...
           
           
Set default value if cloud has not set:


           RemoteConfig.Instance.AddDefault...
  