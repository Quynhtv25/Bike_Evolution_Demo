# IPS `Cloud Storage` PACKAGE
> Last Update: 04/04/2024

## 1. Installation
[*(Documentation)*](https://firebase.google.com/docs/storage/unity/start)

- Install **`IPS.Analytics.Firebase`** first for require.
- Download the latest `.tgz` file in link below, paste into root project folder name `GooglePackages/`:
   - [Cloud Storage for Firebase](https://developers.google.com/unity/archive?hl=vi#cloud_storage_for_firebase)
   - [Firebase Authentication](https://developers.google.com/unity/archive#firebase_authentication)


- Open file `cloudstorage.package.manifest.json` coppy all line inside *dependency* block into project file `Packages/manifest.json`
    - Replace the version number by the file downloaded.
    

- Open unity menu `IPS/Api/ApiSettings`, turn ON the option `Use Cloud Storage`, then `Save` 


## 2. CODE IMPLEMENTATION

Download a file from Firebase Storage:

    CloudStorage.Instance.Download(childPath, ...)