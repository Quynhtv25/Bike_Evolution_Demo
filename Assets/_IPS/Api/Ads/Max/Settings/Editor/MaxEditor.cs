using IPS.Api.Ads;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IPS.Editor {
    [UnityEditor.CustomEditor(typeof(MaxSettings))]
    public class MaxSettingsEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            UnityEditor.EditorGUILayout.Space();

            if (GUILayout.Button("Save", GUILayout.Height(30))) {
                SaveConfig();
            }
        }

        public static void SaveConfig() {
            var s = MaxSettings.Instance;

            bool useMaxSdk = s.UseAOAAd || s.UseNativeAd || s.UseBannerAd || s.UseInterstitialAd || s.UseRewardedVideoAd || s.UseMRecAd;
            SaveConfig(useMaxSdk);

        }

        private static void SaveConfig(bool useMaxSdk) {

#if ADS
            ScriptingDefineHelper.UpdateSymbols(new KeyValuePair<string, bool>("MAX", useMaxSdk));
#else
            ScriptingDefineHelper.UpdateSymbols(new KeyValuePair<string, bool>("MAX", false));
#endif
            SaveMaxConfig();
        }

        public static void TurnOffMaxForTestAdLogic() {
            SaveMaxConfig();
            MaxSettings.Instance.UseAOAAd = false;
            MaxSettings.Instance.UseNativeAd = false;
            MaxSettings.Instance.UseBannerAd = false;
            MaxSettings.Instance.UseInterstitialAd = false;
            MaxSettings.Instance.UseRewardedInterstitialAd = false;
            MaxSettings.Instance.UseMRecAd = false;
            SaveConfig(false);
        }

        public static void SaveMaxConfig() {
#if ADS && MAX
                AppLovinSettings.Instance.SdkKey = MaxSettings.Instance.AppID;
                AppLovinSettings.Instance.AdMobAndroidAppId = AdmobSettings.Instance.AppID_Android;
                AppLovinSettings.Instance.AdMobIosAppId = AdmobSettings.Instance.AppID_iOS;
                EditorUtility.SetDirty(MaxSettings.Instance);
                EditorUtility.SetDirty(AppLovinSettings.Instance);
                AssetDatabase.SaveAssets();
#endif
        }

        [UnityEditor.MenuItem(ApiSettings.LIB_MENU + "/Api/Ads/MaxSettings")]
        public static void OpenMaxSettingsFile() {
            string path = $"Assets/{ApiSettings.LIB_FOLDER}/Api/Ads/Max/Settings/Resources/{typeof(MaxSettings).Name}/";
            string assetPath = Path.Combine(path, $"{typeof(MaxSettings).Name}.asset");
            if (MaxSettings.Instance == null) {
                Directory.CreateDirectory(path);
                var ins = ScriptableObject.CreateInstance<MaxSettings>();
                AssetDatabase.CreateAsset(ins, assetPath);
                AssetDatabase.SaveAssets();
            }
            UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<MaxSettings>(assetPath);
        }

    }
}