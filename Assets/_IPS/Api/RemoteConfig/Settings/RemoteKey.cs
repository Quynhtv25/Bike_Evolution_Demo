using System.IO;
using UnityEngine;

namespace IPS.Api.RemoteConfig {
    [CreateAssetMenu(fileName ="RemoteKey", menuName ="IPS/Api/RemoteConfig")]
    public partial class RemoteKey : SingletonResourcesScriptable<RemoteKey> {
        [Header("Other")]
        public string debug_devices = "debug_devices";

        protected override void Initialize() {

        }

        
#if UNITY_EDITOR
        [UnityEditor.MenuItem(ApiSettings.LIB_MENU + "/Api/RemoteConfigKey")]
        public static void OpenRemoteConfigKeyFile() {
            string path = $"Assets/{ApiSettings.LIB_FOLDER}/Api/RemoteConfig/Settings/Resources/{typeof(RemoteKey).Name}/";
            string assetPath = Path.Combine(path, $"{typeof(RemoteKey).Name}.asset");
            if (RemoteKey.Instance == null) {
                Directory.CreateDirectory(path);
                var ins = ScriptableObject.CreateInstance<RemoteKey>();
                UnityEditor.AssetDatabase.CreateAsset(ins, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<RemoteKey>(assetPath);
        }
#endif
    }
}