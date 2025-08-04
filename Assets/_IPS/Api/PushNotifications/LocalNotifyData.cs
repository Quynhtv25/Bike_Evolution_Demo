using System;
using System.IO;
using UnityEngine;

namespace IPS.Api.Notifications {
    [CreateAssetMenu(fileName ="LocalNotifyData", menuName = ApiSettings.LIB_MENU + "/Api/LocalNotifyData")]
    public class LocalNotifyData : ScriptableObject {
        [SerializeField] private NotifyInfo[] data;
        public NotifyInfo[] Data => data;

        private static LocalNotifyData _ins;
        public static LocalNotifyData Instance {
            get {
                if (_ins != null) return _ins;
                _ins = Resources.Load<LocalNotifyData>(string.Format("{0}/{1}", typeof(LocalNotifyData).Name, typeof(LocalNotifyData).Name));
                if (_ins == null) {
                    Debug.LogError($"File not found at Resources/{typeof(LocalNotifyData).Name}/{typeof(LocalNotifyData).Name}");
                }
                return _ins;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem(ApiSettings.LIB_MENU + "/Api/Local Notifications Settings")]
        public static void OpenSettingsFile() {
            if (LocalNotifyData.Instance == null) {
                string path = $"Assets/{ApiSettings.LIB_FOLDER}/Api/PushNotifications/Resources/{typeof(LocalNotifyData).Name}/";
                Directory.CreateDirectory(path);
                var ins = ScriptableObject.CreateInstance<LocalNotifyData>();
                string assetPath = Path.Combine(path, $"{typeof(LocalNotifyData).Name}.asset");
                UnityEditor.AssetDatabase.CreateAsset(ins, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            UnityEditor.Selection.activeObject = LocalNotifyData.Instance;// UnityEditor.AssetDatabase.LoadAssetAtPath<LocalNotifyData>($"Assets/{ApiSettings.LIB_FOLDER}/Api/PushNotifications/Resources/LocalNotifyData/LocalNotifyData.asset");
        }
#endif

    }


    [Serializable]
    public struct NotifyInfo {
        [Tooltip("[Optional]")]
        public int id;
        [Tooltip("Exactly time use local device time zone")]
        public Time exactlyTime;
        [Tooltip("Delay time to push this noti, start count delay from session start")]
        public Time waitingTime;
        [Tooltip("The durations to loop this noti")]
        public Time repeatTime;
        [Tooltip("Item will be pick random to push")]
        public NotiContent[] contents;

        public bool IsNotNull => contents!= null && contents.Length > 0 && !string.IsNullOrEmpty(contents[0].title);

        public NotifyInfo(NotiContent content) {
            id = 0;
            exactlyTime = default;
            waitingTime = default;
            repeatTime = default;
            contents = new NotiContent[] { content };
        }

        public NotiContent GetRandomContent() {
            if (contents == null || contents.Length == 0) return default;
            if (contents.Length == 1) return contents[0];
            return contents[UnityEngine.Random.Range(0, contents.Length)];
        }
    }

    [Serializable]
    public struct NotiContent {
        public string title;
        public string message;

        public NotiContent(string title, string message) {
            this.title = title;
            this.message = message;
        }
    }

    [Serializable]
    public struct Time {
        public int days;
        public int hours;
        public int minutes;
        public int seconds;

        public Time(TimeSpan timeSpan) {
            days = timeSpan.Days;
            hours = timeSpan.Hours;
            minutes = timeSpan.Minutes;
            seconds = timeSpan.Seconds;
        }

        public TimeSpan ToTimeSpan() {
            return new TimeSpan(days, hours, minutes, seconds);
        }
    }
}
