#if APPMETRICA

using UnityEngine;
using IPS.Api.Analytics;
using UnityEditor;
using System.IO;

namespace IPS.Editor {
    public class IPSAppMetricaEditor {
 
#if UNITY_EDITOR
        [UnityEditor.MenuItem(ApiSettings.LIB_MENU + "/Api/Analytics/AppMetricaSettings")]
        public static void SelectSettings() {
            UnityEditor.Selection.activeObject = Resources.Load<AppMetricaSettings>("AppMetricaSettings/AppMetricaSettings");

            if (UnityEditor.Selection.activeObject == null) {
                string path = $"Assets/{ApiSettings.LIB_FOLDER}/Api/Analytics/IPSAppMetrica/Resources/{typeof(AppMetricaSettings).Name}/";
                Directory.CreateDirectory(path);
                var ins = ScriptableObject.CreateInstance<AppMetricaSettings>();
                string assetPath = Path.Combine(path, $"{typeof(AppMetricaSettings).Name}.asset");
                AssetDatabase.CreateAsset(ins, assetPath);
                AssetDatabase.SaveAssets();

                SelectSettings();
            }
        }
#endif
    }
}
#endif