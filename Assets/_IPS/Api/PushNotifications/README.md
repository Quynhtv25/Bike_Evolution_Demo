# IPS `PushNotifications` PACKAGE
> Last Update: 10/04/2024

## 1. Installation
[*(Documentation)*](https://firebase.google.com/docs/cloud-messaging?authuser=0)

- Install **`IPS.Analytics.Firebase`** first for require.
- Download the latest `.tgz` file in link below, paste into root project folder name `GooglePackages/`:
   - [Firebase Cloud Messaging](https://developers.google.com/unity/archive?hl=vi#firebase_cloud_messaging)


- Open file `pushnotify.packages.manifest.json` coppy all line inside *dependency* block into project file `Packages/manifest.json`
    - Replace the version number by the file downloaded.
- Open unity menu `IPS/Api/ApiSettings`, turn ON the option `Use Cloud Messaging`, then `Save` 
- Open unity menu `Edit/ProjectSettings`, Select tab `Mobile Notification`:


**For Android tab:**
   - Turn on *Reschedule on Device Restart*
   - Turn on *Use Custom Activity*
   - Paste this string into field *Custom Activity Name*: `com.google.firebase.MessagingUnityPlayerActivity`


**For iOS tab:**
   - Turn on *Request Authorization on App Launch*
   - Turn on *Enable Push Notifications*
   - Turn on *Register for Push Notifications on App Launch*

 
- **For Android**: Select unity menu `Assets/External Dependency Manager/Android Resolver/Force Resolve`

## 2. Code Implementation
- **Initialize:** On the first scene of the Game, create an empty object then add this script `Bootstrap` into it, then enable **`Preload Analytics`** and **`Preload Cloud Messaging`** field.


## 3. Setup Push Message
- **`For Local push:`** Open unity menu `IPS/Api/Local Notifications Settings`, fill message data to be push offline.
- **`For remote push:`** Go to website of Firebase console > `Firebase Cloud Messaging`, config all cloud message here to push from remote.
