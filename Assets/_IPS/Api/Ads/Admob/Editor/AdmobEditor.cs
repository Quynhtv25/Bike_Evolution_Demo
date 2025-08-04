using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IPS.Api.Ads;

#if ADS
namespace IPS.Editor {
    public class AdmobEditor {
        [UnityEditor.Callbacks.PostProcessBuildAttribute(1)]
        public static void OnPostprocessBuild(UnityEditor.BuildTarget target, string pathToBuiltProject) {
            if (!CheckAdmobConfig()) {
                throw new UnityEditor.Build.BuildFailedException("Admob AppID was not config. Goto menu 'IPS/Api/AdsSettings' to setup it");
            }
        }

        private static bool IsAdmobConfiged {
            get {
#if ADMOB
                if (!GoogleMobileAds.Editor.GoogleMobileAdsSettings.LoadInstance()) {
                    Debug.LogError("NULL GoogleMobileAdsSettings");
                    return false;
                }

#if UNITY_ANDROID
                if (string.IsNullOrEmpty(GoogleMobileAds.Editor.GoogleMobileAdsSettings.LoadInstance().GoogleMobileAdsAndroidAppId)
                    || !GoogleMobileAds.Editor.GoogleMobileAdsSettings.LoadInstance().GoogleMobileAdsAndroidAppId.Equals(AdmobSettings.Instance.AppID_Android)) {
                    Debug.LogError("GoogleMobileAdsAndroidAppId is not equals to IPS.AdmobSettings.AppID_Android");
                    return false;
                }
#else
                if (string.IsNullOrEmpty(GoogleMobileAds.Editor.GoogleMobileAdsSettings.LoadInstance().GoogleMobileAdsIOSAppId)
                    || !GoogleMobileAds.Editor.GoogleMobileAdsSettings.LoadInstance().GoogleMobileAdsIOSAppId.Equals(AdmobSettings.Instance.AppID_iOS)) {
                    Debug.LogError("GoogleMobileAdsIOSAppId is not equals to IPS.AdmobSettings.AppID_iOS");
                    return false;
                }
#endif

#endif
                return true;
            }
        }

        //[UnityEditor.Callbacks.DidReloadScripts()]
        public static bool CheckAdmobConfig () {
            if (!IsAdmobConfiged) {
                SaveAdmobConfig();

                if (!IsAdmobConfiged) {
                    Debug.LogError("[Ads] AdmobConfig: You need to enableTest or fill the field 'appID'.");
                    return false;
                }
                else {
                    Debug.Log("[Ads] AdmobConfig: Apply to gooogle setting successfully");
                }
            }
            return true;
        }

        public static void SaveAdmobConfig() {
#if ADMOB
            var gg = GoogleMobileAds.Editor.GoogleMobileAdsSettings.LoadInstance();
            gg.GoogleMobileAdsAndroidAppId = AdmobSettings.Instance.AppID_Android;
            gg.GoogleMobileAdsIOSAppId = AdmobSettings.Instance.AppID_iOS;
            gg.UserTrackingUsageDescription = AdmobSettings.Instance.iOSATTMessage;
            UnityEditor.EditorUtility.SetDirty(gg);
            UnityEditor.AssetDatabase.SaveAssets();
#endif

            MaxSettingsEditor.SaveConfig();
            IronSourceSettingsEditor.SaveConfig();
        }

        public static void SaveConfigForAdTest() {
            AdmobSettings.Instance.EnableUMP = false;
            AdmobSettings.Instance.UseTestAd = true;
            AdmobSettings.Instance.EnableLog = true;
#if MAX
            AdmobSettings.Instance.UseBannerAd = MaxSettings.Instance.UseBannerAd;
            AdmobSettings.Instance.ShowBannerOnBottom = MaxSettings.Instance.ShowBannerOnBottom;
            AdmobSettings.Instance.UseInterstitialAd = MaxSettings.Instance.UseInterstitialAd;
            AdmobSettings.Instance.UseRewardedVideoAd = MaxSettings.Instance.UseRewardedVideoAd;
#elif IS

            AdmobSettings.Instance.UseBannerAd = ISSettings.Instance.UseBannerAd;
            AdmobSettings.Instance.ShowBannerOnBottom = ISSettings.Instance.ShowBannerOnBottom;
            AdmobSettings.Instance.UseInterstitialAd = ISSettings.Instance.UseInterstitialAd;
            AdmobSettings.Instance.UseRewardedVideoAd = ISSettings.Instance.UseRewardedVideoAd;
#endif

            MaxSettingsEditor.TurnOffMaxForTestAdLogic();
            IronSourceSettingsEditor.TurnOffISForTestAdLogic();

            UnityEditor.EditorUtility.SetDirty(AdmobSettings.Instance);
            UnityEditor.AssetDatabase.SaveAssets();
            SaveAdmobConfig();
        }


        //void OnValidate() {
        //    Reset();
        //}
                
        //void Reset() {
        //    CheckAdmobConfig();
        //}
    }

    [UnityEditor.CustomEditor(typeof(AdmobSettings))]
    public class AdmobConfigEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            //UnityEditor.EditorGUILayout.BeginHorizontal();

            UnityEditor.EditorGUILayout.Space();

            if (GUILayout.Button("Save", GUILayout.Height(30))) {
                var setting = (target as AdmobSettings);
#if ADS
                AdmobSettings.Instance.iOSATTMessage = AdmobSettings.Instance.iOSATTMessage.Replace("Project", Application.productName);
                ScriptingDefineHelper.UpdateSymbols(new KeyValuePair<string, bool>("ADMOB_NATIVE", setting.UseNativeAd && setting.UseNativeAd_3D));
                ScriptingDefineHelper.UpdateSymbols(new KeyValuePair<string, bool>("ADMOB_NATIVE_UI", setting.UseNativeAd && setting.UseNativeAd_UI));
#else
                ScriptingDefineHelper.UpdateSymbols(new KeyValuePair<string, bool>("ADMOB_NATIVE", false));
                ScriptingDefineHelper.UpdateSymbols(new KeyValuePair<string, bool>("ADMOB_NATIVE_UI", false));
#endif
                AdmobEditor.SaveAdmobConfig();
            }

            UnityEditor.EditorGUILayout.Space();

#if ADMOB
            if (GUILayout.Button("Verify Google Setting", GUILayout.Height(25))) {
                UnityEditor.Selection.activeObject = GoogleMobileAds.Editor.GoogleMobileAdsSettings.LoadInstance();
            }

            UnityEditor.EditorGUILayout.Space();

            if (GUILayout.Button("Verify Build", GUILayout.Height(25))) {
                Debug.Log("Admob Settings verify: " + AdmobEditor.CheckAdmobConfig());
            }
#endif

            //UnityEditor.EditorGUILayout.EndHorizontal();
        }
    }
}
#endif