#if ADJUST

using UnityEngine;
using IPS.Api.Analytics;
using UnityEditor;
using System.IO;

namespace IPS.Editor {
    public class IPSAdjustEditor {
 
#if UNITY_EDITOR
        [UnityEditor.MenuItem(ApiSettings.LIB_MENU + "/Api/Analytics/AdjustSettings")]
        public static void SelectSettings() {
            UnityEditor.Selection.activeObject = Resources.Load<AdjustSettings>($"{typeof(AdjustSettings).Name}/{typeof(AdjustSettings).Name}");

            if (UnityEditor.Selection.activeObject == null) {
                string path = $"Assets/{ApiSettings.LIB_FOLDER}/Api/Analytics/IPSAdjust/Resources/{typeof(AdjustSettings).Name}/";
                Directory.CreateDirectory(path);
                var ins = ScriptableObject.CreateInstance<AdjustSettings>();
                string assetPath = Path.Combine(path, $"{typeof(AdjustSettings).Name}.asset");
                AssetDatabase.CreateAsset(ins, assetPath);
                AssetDatabase.SaveAssets();

                SelectSettings();
            }
        }
#endif
    }
}
#endif