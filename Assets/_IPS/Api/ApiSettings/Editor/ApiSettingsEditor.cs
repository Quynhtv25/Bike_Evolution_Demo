using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace IPS.Editor {
    [UnityEditor.CustomEditor(typeof(ApiSettings))]
    internal class ApiSettingsEditor : UnityEditor.Editor {

        [UnityEditor.MenuItem(ApiSettings.LIB_MENU + "/Api/ApiSettings")]
        public static void OpenSettingsFile() {
            UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<ApiSettings>($"Assets/{ApiSettings.LIB_FOLDER}/Api/ApiSettings/{typeof(ApiSettings).Name}.asset");
            if (UnityEditor.Selection.activeObject == null) {
                string path = $"Assets/{ApiSettings.LIB_FOLDER}/Api/ApiSettings/";
                Directory.CreateDirectory(path);
                var ins = ScriptableObject.CreateInstance<ApiSettings>();
                string assetPath = Path.Combine(path, $"{typeof(ApiSettings).Name}.asset");
                AssetDatabase.CreateAsset(ins, assetPath);
                AssetDatabase.SaveAssets();

                OpenSettingsFile();
            }
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GUILayout.Space(20);
            if (GUILayout.Button("Save")) {
                SaveSetting();
            }
        }

        private void SaveSetting() {
            var setting = target as ApiSettings;

            if (setting.useRemoteConfig || setting.useCloudMessaging || setting.useCloudStorage) setting.useFirebase = true;

#if UNITY_IOS
            BootstrapConfig.Instance.VersionCode = PlayerSettings.Android.bundleVersionCode;
            BootstrapConfig.Instance.IsAAB = true;
            setting.productionBuild = true;
#endif

            ScriptingDefineHelper.UpdateSymbols(
                new KeyValuePair<string, bool>(ApiSettings.GDPRDefine, setting.useGDPR),
                new KeyValuePair<string, bool>(ApiSettings.AdsDefine, setting.useAds),
                new KeyValuePair<string, bool>(ApiSettings.InAppPurchaseDefine, setting.useInAppPurchase),
                new KeyValuePair<string, bool>(ApiSettings.InAppReviewDefine, setting.useInAppReview),
                new KeyValuePair<string, bool>(ApiSettings.InAppUpdateDefine, setting.useInAppUpdate),
                new KeyValuePair<string, bool>(ApiSettings.RemoteDefine, setting.useRemoteConfig),
                new KeyValuePair<string, bool>(ApiSettings.CloudMessagingDefine, setting.useCloudMessaging),
                new KeyValuePair<string, bool>(ApiSettings.CloudStorageDefine, setting.useCloudStorage),
                new KeyValuePair<string, bool>(ApiSettings.FirebaseDefine, setting.useFirebase),
                new KeyValuePair<string, bool>(ApiSettings.FacebookDefine, setting.useFacebook),
                new KeyValuePair<string, bool>(ApiSettings.AppsFlyerDefine, setting.useAppsFlyer),
                new KeyValuePair<string, bool>(ApiSettings.AppsFlyerROIDefine, setting.useAppsFlyerROI),
                new KeyValuePair<string, bool>(ApiSettings.ByteBrewDefine, setting.useByteBrew),
                new KeyValuePair<string, bool>(ApiSettings.AppMetricaDefine, setting.useAppmetrica),
                new KeyValuePair<string, bool>(ApiSettings.AdjustDefine, setting.useAdjust),
                new KeyValuePair<string, bool>(ApiSettings.FalconSDKDefine, setting.useFalconSDK),
                new KeyValuePair<string, bool>(ApiSettings.LionSDKDefine, setting.useLionSDK),
                new KeyValuePair<string, bool>(ApiSettings.ProductionDefine, setting.productionBuild)
            );

            if (setting.useFacebook) {
#if FACEBOOK
                IPSFacebookEditor.UpdateSelectedAppIndex();
#endif
            }

            UnityEditor.EditorUtility.SetDirty(UnityEditor.Selection.activeObject);
            UnityEditor.AssetDatabase.SaveAssets();
        }

#if UNITY_IOS
        // Automatically set the "Always Embed Swift Standard Libraries" option to "No" in UnityFramework Target in XCode
        [PostProcessBuildAttribute(Int32.MaxValue)] //We want this code to run last!
        public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuildProject) {
            if (buildTarget != BuildTarget.iOS) return; // Make sure its iOS build
            
            // Getting access to the xcode project file
            string projectPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            UnityEditor.iOS.Xcode.PBXProject pbxProject = new UnityEditor.iOS.Xcode.PBXProject();
            pbxProject.ReadFromFile(projectPath);
            
            // Getting the UnityFramework Target and changing build settings
            string target = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

            // After we're done editing the build settings we save it 
            pbxProject.WriteToFile(projectPath);
        }
#endif

    }
}