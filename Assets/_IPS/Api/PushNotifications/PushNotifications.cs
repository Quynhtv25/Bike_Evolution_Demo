using System;
using UnityEngine;

namespace IPS.Api.Notifications {
    public class PushNotifications : SingletonBehaviourDontDestroy<PushNotifications> {
        private INotify notifyPlatform;

        bool initialized;

        protected override void OnAwake() {
#if FCM && !UNITY_EDITOR
#if ADS
            AdsManager.PauseInsideApp = true;
#endif
#if UNITY_ANDROID
            notifyPlatform = new AndroidNotify();
#elif UNITY_IOS
            notifyPlatform = new iOSNotify();
#endif
            Debug.Log("[Notifications] Initializing...");
            StartCoroutine(notifyPlatform.Init(OnInitialized));
#endif
        }

        private void OnInitialized() {
            Debug.Log("[Notifications] Init successfully");
            initialized = true;
        }

        private void OnApplicationFocus(bool focus) {
            if (!initialized) return;
#if FCM && !UNITY_EDITOR
            if (!focus) {
                if (LocalNotifyData.Instance != null && LocalNotifyData.Instance.Data.Length > 0) {
                    foreach (var noti in LocalNotifyData.Instance.Data) {
                        SendNoti(noti);
                    }
                }
            }
            else {
                Logs.Log("[Notifications] Cancel all on focus.");
                notifyPlatform.CancelAll();
            }
#endif
        }

        protected override void OnDestroy() {
#if FCM && !UNITY_EDITOR
            notifyPlatform.Dispose();
#endif
            base.OnDestroy();
        }

        public void SendNoti(NotifyInfo noti) {
            if (!initialized) return;
#if FCM
            if (notifyPlatform == null || !noti.IsNotNull) return;

            if (noti.exactlyTime.ToTimeSpan().TotalSeconds > 0) {
                var target = noti.exactlyTime.ToTimeSpan();
                var now = TimeSpan.FromTicks(DateTime.Now.Ticks);
                if (TimeSpan.Compare(target, now) >= 0) {
                    noti.waitingTime = new Time(target - now);
                }
                else {
                    if (noti.repeatTime.days > 0) {
                        noti.waitingTime = new Time(target + noti.repeatTime.ToTimeSpan() - now);
                    }
                }
            }

            var content = noti.GetRandomContent();
            notifyPlatform.SendNoti(content.title, content.message, 
                                    noti.waitingTime.ToTimeSpan(),
                                    noti.repeatTime.ToTimeSpan());
#endif
        }
    }
}