using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IPS.Editor {
    public class ScriptingDefineHelper {
        public static void UpdateSymbols(params KeyValuePair<string, bool>[] symbols) {
            if (symbols == null || symbols.Length == 0) return;

            var list = GetCurrentSymbols();

            foreach(var item in symbols) { 
                UpdateFlag(list, item.Key, item.Value);
            }

            SaveSymbols(list);
        }

        public static List<string> GetCurrentSymbols() {
            string flagString = PlayerSettings.GetScriptingDefineSymbolsForGroup(Platform);
            return new List<string>(flagString.Split(';'));
        }

        public static void SaveSymbols(List<string> list) {
            string newFlags = string.Join(";", list);
            Debug.Log($"SetScriptingDefineSymbols: {newFlags}");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(Platform, newFlags);
        }

        public static void UpdateFlag(List<string> currentList, string flag, bool enable) {
            if (enable && !currentList.Contains(flag)) {
                currentList.Add(flag);
            }
            else if (!enable && currentList.Contains(flag)) {
                currentList.Remove(flag);
            }
        }

        private static BuildTargetGroup Platform {
            get {
#if UNITY_ANDROID
                return BuildTargetGroup.Android;
#elif UNITY_IOS
                return BuildTargetGroup.iOS;
#else
                return BuildTargetGroup.Standalone;
#endif
            }
        }
    }
}