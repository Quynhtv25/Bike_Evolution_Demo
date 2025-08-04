using IPS.Api.Ads;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IPS.Editor {
    [UnityEditor.CustomEditor(typeof(ISSettings))]
    public class IronSourceSettingsEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            UnityEditor.EditorGUILayout.Space();

            if (GUILayout.Button("Save", GUILayout.Height(30))) {
                SaveConfig();
            }
        }
        
        public static void SaveConfig() {
            var s = ISSettings.Instance;

            bool useIsSDK = s.UseIS;
            SaveConfig(useIsSDK, s.UseVersion770);
        }

        private static void SaveConfig(bool useISSdk, bool useVer770) {
#if ADS
            ScriptingDefineHelper.UpdateSymbols(new KeyValuePair<string, bool>("IS_770", useVer770));
            ScriptingDefineHelper.UpdateSymbols(new KeyValuePair<string, bool>("IS", useISSdk));
#else
            ScriptingDefineHelper.UpdateSymbols(new KeyValuePair<string, bool>("IS_770", false));
            ScriptingDefineHelper.UpdateSymbols(new KeyValuePair<string, bool>("IS", false));
#endif
            SaveISConfig();
        }

        public static void TurnOffISForTestAdLogic() {
            SaveISConfig();
            ISSettings.Instance.UseAOAAd = false;
            ISSettings.Instance.UseNativeAd = false;
            ISSettings.Instance.UseBannerAd = false;
            ISSettings.Instance.UseInterstitialAd = false;
            ISSettings.Instance.UseRewardedInterstitialAd = false;
            ISSettings.Instance.UseMRecAd = false;
            SaveConfig(false, false);
        }

        public static void SaveISConfig() {
#if ADS && IS
            var s = ISSettings.Instance;
            var settings = AssetDatabase.LoadAssetAtPath<IronSourceMediationSettings>(IronSourceMediationSettings.IRONSOURCE_SETTINGS_ASSET_PATH);
            if (settings != null) {
                settings.AndroidAppKey = s.AppID_Android;
                settings.IOSAppKey = s.AppID_iOS;

                EditorUtility.SetDirty(settings);
            }
#if IS_770
            var setting2 = AssetDatabase.LoadAssetAtPath<IronSourceMediatedNetworkSettings>(IronSourceMediatedNetworkSettings.MEDIATION_SETTINGS_ASSET_PATH);
            if (setting2 != null) {
                setting2.EnableAdmob = true;
                setting2.AdmobAndroidAppId = AdmobSettings.Instance.AppID_Android;
                setting2.AdmobIOSAppId = AdmobSettings.Instance.AppID_iOS;
                EditorUtility.SetDirty(setting2);
            }
#else
            LevelPlayEditor.ApplyMediationSettings();
#endif
            EditorUtility.SetDirty(s);
            AssetDatabase.SaveAssets();
#endif

        }

        [UnityEditor.MenuItem(ApiSettings.LIB_MENU + "/Api/Ads/IronSourceSettings")]
        public static void OpenIronSourceSettingsFile() {
            string path = $"Assets/{ApiSettings.LIB_FOLDER}/Api/Ads/IronSource/Settings/Resources/{typeof(ISSettings).Name}/";
            string assetPath = Path.Combine(path, $"{typeof(ISSettings).Name}.asset");
            if (ISSettings.Instance == null) {
                Directory.CreateDirectory(path);
                var ins = ScriptableObject.CreateInstance<ISSettings>();
                AssetDatabase.CreateAsset(ins, assetPath);
                AssetDatabase.SaveAssets();
            }

            UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<ISSettings>(assetPath);
        }

    }
}