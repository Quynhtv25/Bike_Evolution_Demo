using IPS.Api.Ads;
using System.IO;
using UnityEngine;

namespace IPS {
    public class BootstrapConfig : ScriptableObject {
        [Header("Game Config")]
        public int TargetFrameRate = 60;
        public bool ScreenNeverSleep = true;

        [Tooltip("Set this to maxium number to play offline")]
        [SerializeField] private long levelInternetRequire = 0;
        public long LevelInternetRequire {
            get => levelInternetRequire;
            set => levelInternetRequire = value < 0 ? 0 : value > long.MaxValue ? long.MaxValue : value;
        }

        [Header("Debug config")]
        [Tooltip("Turn on for show all log in the IPS.Core (Singleton, Event dispatcher, Pooling")]
        public bool EnableCoreLog = false;
        public string[] DebugDeviceGAID = new string[] {"84674929-2d69-4035-bb9f-2db27482b5e9",
                                                        "818c764b-94fd-4ad1-8f33-b03e05314105",
                                                        "76850258-4fb2-4699-ae3b-09d059eafe62"
                                                        };

        public string VersionName => Application.version;

        [HideInInspector][SerializeField] public bool IsTester;
        [HideInInspector][SerializeField] public string MyGAID;
        [Header("Auto by Jenkins")]
        [SerializeField] public int VersionCode;
        [SerializeField] public bool IsAAB;

        #region Singleton
        private static BootstrapConfig _instance;
        public static BootstrapConfig Instance {
            get {
                if (_instance != null) return _instance;
                _instance = Resources.Load<BootstrapConfig>($"{typeof(BootstrapConfig).Name}/{typeof(BootstrapConfig).Name}");
                if (_instance == null) {
                    Debug.LogError($"[Ads] BootstrapConfig file not found at Resources/{typeof(BootstrapConfig).Name}/{typeof(BootstrapConfig).Name}");
                }
                return _instance;
            }
        }
        #endregion

#if UNITY_EDITOR
        [UnityEditor.MenuItem("IPS/Config/BootstrapConfig")]
        public static void OpenBootstrapSettingsFile() {
            if (BootstrapConfig.Instance == null) {
                string path = $"Assets/{ApiSettings.LIB_FOLDER}/Core/Resources/{typeof(BootstrapConfig).Name}/";
                Directory.CreateDirectory(path);
                var ins = ScriptableObject.CreateInstance<BootstrapConfig>();
                string assetPath = Path.Combine(path, $"{typeof(BootstrapConfig).Name}.asset");
                UnityEditor.AssetDatabase.CreateAsset(ins, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
            }

            UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<BootstrapConfig>($"Assets/{ApiSettings.LIB_FOLDER}/Core/Resources/{typeof(BootstrapConfig).Name}/{typeof(BootstrapConfig).Name}.asset");
        }
#endif
    }
}