using System;
using UnityEngine;

namespace IPS {
    [CreateAssetMenu(fileName = "GameData", menuName = GAME_MENU + "/GameData")]
    public partial class GameData : SingletonResourcesScriptable<GameData> {
        public const string GAME_FOLDER = "_GAME";
        public const string GAME_MENU = "GAME";

#if UNITY_EDITOR
        [UnityEditor.MenuItem(GameData.GAME_MENU + "/DATA/Open GameData")]
        private static void OpenGameDataMenu() {
            string path = $"Assets/{GAME_FOLDER}/Data/Resources/{typeof(GameData).Name}/";
            string assetPath = System.IO.Path.Combine(path, $"{typeof(GameData).Name}.asset");

            UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameData>(assetPath);

            if (UnityEditor.Selection.activeObject == null) {
                if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
                var ins = ScriptableObject.CreateInstance<GameData>();
                UnityEditor.AssetDatabase.CreateAsset(ins, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
                OpenGameDataMenu();
            }
        }
#endif

        protected override void Initialize() {
            Debug.Log($"{name} Initialized");
            OnInitialize();
        }

        public void UnlockAll() { DoUnlockAll(); }
        partial void DoUnlockAll();
        partial void OnInitialize();
    }

    public abstract class AHardData<T> : ScriptableObject {
        [SerializeField] protected T[] data;

        public T[] AllData => data;

        public T GetItem(int idx) {
            if (idx < 0 || idx >= data.Length) {
                Logs.LogError($"Cannot found item with idx={idx}, total data length={data.Length}");
            }
            return data[idx];
        }

        public bool GetRandomNotUnlocked(ref T result) {
            var locks = Array.FindAll(data, d => !IsUnlocked(d));
            if (locks == null || locks.Length == 0) { return false; }
            result = locks[UnityEngine.Random.Range(0, locks.Length)];
            return true;
        }

        public virtual bool IsUnlocked(T item) { return false; }
    }

}
