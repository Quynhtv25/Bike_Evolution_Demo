#if APPSFLYER

using UnityEngine;
using UnityEditor;
using System.IO;

namespace IPS.Editor {
    public class IPSAppsFlyerEditor {
 
#if UNITY_EDITOR
        [UnityEditor.MenuItem(ApiSettings.LIB_MENU + "/Api/Analytics/AppsFlyerSettings")]
        public static void SelectSettings() {
            UnityEditor.Selection.activeObject = Resources.Load<IPS.Api.Analytics.AppsFlyerSettings>("AppsFlyerSettings");

            if (UnityEditor.Selection.activeObject == null) {
                string path = $"Assets/{ApiSettings.LIB_FOLDER}/Api/Analytics/IPSAppsFlyer/Resources/";
                Directory.CreateDirectory(path);
                var ins = ScriptableObject.CreateInstance<IPS.Api.Analytics.AppsFlyerSettings>();
                string assetPath = Path.Combine(path, $"{typeof(IPS.Api.Analytics.AppsFlyerSettings).Name}.asset");
                AssetDatabase.CreateAsset(ins, assetPath);
                AssetDatabase.SaveAssets();

                SelectSettings();
            }
        }
#endif
    }
}
#endif