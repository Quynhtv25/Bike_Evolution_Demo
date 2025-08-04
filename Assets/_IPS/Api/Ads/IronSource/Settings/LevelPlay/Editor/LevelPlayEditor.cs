using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlayEditor
{
#if ADS && IS
    public static void ApplyMediationSettings() {
        var setting2 = UnityEditor.AssetDatabase.LoadAssetAtPath<IronSourceMediatedNetworkSettings>(IronSourceMediatedNetworkSettings.MEDIATION_SETTINGS_ASSET_PATH);
        if (setting2 != null) {

            var gg = GoogleMobileAds.Editor.GoogleMobileAdsSettings.LoadInstance();

            setting2.EnableAdmob = true;
            setting2.AdmobAndroidAppId = gg.GoogleMobileAdsAndroidAppId;
            setting2.AdmobIOSAppId = gg.GoogleMobileAdsIOSAppId;
            UnityEditor.EditorUtility.SetDirty(setting2);
            UnityEditor.AssetDatabase.SaveAssets();
            Debug.Log($"SaveLevelPlay: admob_android_appId={setting2.AdmobAndroidAppId}, admob_iOS_appId={setting2.AdmobIOSAppId}");
        }
    }
#endif
}
