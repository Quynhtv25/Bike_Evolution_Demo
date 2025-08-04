using System.IO;
using UnityEngine;

namespace IPS.Api.Analytics {
    [CreateAssetMenu(fileName = "TrackingSettings", menuName = "IPS/Api/Analytics/TrackingSettings")]
    public class TrackingSettings : SingletonResourcesScriptable<TrackingSettings> {
        public TrackingEvent eventName;
        public CustomService customService;

        protected override void Initialize() {

        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem(ApiSettings.LIB_MENU + "/Api/Analytics/TrackingSettings")]
        public static void OpenTrackingSettingsFile() {
            string path = $"Assets/{ApiSettings.LIB_FOLDER}/Api/Analytics/Settings/Resources/{typeof(TrackingSettings).Name}/";
            string assetPath = Path.Combine(path, $"{typeof(TrackingSettings).Name}.asset");
            if (TrackingSettings.Instance == null) {
                Directory.CreateDirectory(path);
                var ins = ScriptableObject.CreateInstance<TrackingSettings>();
                UnityEditor.AssetDatabase.CreateAsset(ins, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<TrackingSettings>(assetPath);
        }
#endif
    }
}