using System;
using System.Collections;
#if FCM
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif
#endif

namespace IPS.Api.Notifications {
    internal interface INotify : IDisposable {
        IEnumerator Init(Action callback);
        void SendNoti(string title, string body, TimeSpan waiting, TimeSpan repeat = default, string chanelId = null);
        void CancelAll();
    }

#if !UNITY_EDITOR && UNITY_ANDROID && FCM
    internal class AndroidNotify : INotify {
        private const string ChannelId = "channel_id";

        public IEnumerator Init(Action callback) {
            var request = new PermissionRequest();
            while (request.Status == PermissionStatus.RequestPending) {
                yield return null;
            }

            if (!AndroidNotificationCenter.Initialize()) {
                Logs.LogError("[Notifications] AndroidNotificationCenter failed to Init!");
            }

            RegisterChannel();

            AndroidNotificationCenter.OnNotificationReceived += OnNotiReceived;

            if (callback != null) callback.Invoke();
        }

        public void Dispose() {
            AndroidNotificationCenter.OnNotificationReceived -= OnNotiReceived;
        }

        private void RegisterChannel() {
            var channel = new AndroidNotificationChannel() {
                Id = ChannelId,
                Name = "Default Channel",
                Importance = Importance.High,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }

        public void CancelAll() {
            AndroidNotificationCenter.CancelAllNotifications();
        }

        public void SendNoti(string title, string body, TimeSpan waiting, TimeSpan repeat = default, string chanelId = null) {
            if (IPSConfig.LogEnable) {
                Logs.Log($"[Notifications] Schedule after {waiting.TotalSeconds}s, repeat={repeat.TotalSeconds}s, title={title}");
            }

            AndroidNotificationCenter.SendNotification(NewNoti(title, body, waiting, repeat), !string.IsNullOrEmpty(chanelId) ? chanelId : ChannelId);
        }

        private AndroidNotification NewNoti(string title, string body, TimeSpan waiting, TimeSpan repeat) {
            var notification = new AndroidNotification();
            notification.Title = title;
            notification.Text = body;
            notification.FireTime = DateTime.Now.Add(waiting);
            notification.RepeatInterval = repeat.TotalMilliseconds > 0 ? repeat : null;
            return notification;
        }

        private void OnNotiReceived(AndroidNotificationIntentData data) {
            if (IPSConfig.LogEnable) {
                Logs.Log($"[Notifications] OnNotiReceived id={data.Id}, title={data.Notification.Title}, body={data.Notification.Text}");
            }
        }
    }
#endif // ANDROID

#if !UNITY_EDITOR && UNITY_IOS && FCM
    internal class iOSNotify : INotify {

        public IEnumerator Init(Action callback) {
            using (var request = new AuthorizationRequest(AuthorizationOption.Badge | AuthorizationOption.Sound | AuthorizationOption.Alert, false)) {
                while (!request.IsFinished) {
                    yield return null;
                }
            }

            iOSNotificationCenter.OnNotificationReceived += OnNotiCallback;

            if (callback != null) callback.Invoke();
        }

        public void Dispose() {
            iOSNotificationCenter.OnNotificationReceived -= OnNotiCallback;
        }

        public void CancelAll() {
            iOSNotificationCenter.RemoveAllDeliveredNotifications();
            iOSNotificationCenter.RemoveAllScheduledNotifications();
        }

        public void SendNoti(string title, string body, TimeSpan waiting, TimeSpan repeat = default, string chanelId = null) {
            if (IPSConfig.LogEnable) {
                Logs.Log($"[Notifications] Schedule after {waiting.TotalSeconds}s, repeat={repeat.TotalSeconds}s, title={title}");
            }
            iOSNotificationCenter.ScheduleNotification(NewNoti(title, body, waiting, repeat));
        }

        private iOSNotification NewNoti(string title, string body, TimeSpan waiting, TimeSpan repeat = default) {
            var notification = new iOSNotification();
            notification.Title = title;
            notification.Body = body;
            notification.Trigger = new iOSNotificationTimeIntervalTrigger() {
                TimeInterval = waiting,
                Repeats = repeat.TotalMilliseconds > 0 ? true : false
            };
            return notification;
        }

        private void OnNotiCallback(iOSNotification data) {
            if (IPSConfig.LogEnable) {
                Logs.Log($"[Notifications] OnNotiReceived id={data.Identifier}, title={data.Title}, body={data.Body}");
            }
        }
    }
#endif // IOS
}
