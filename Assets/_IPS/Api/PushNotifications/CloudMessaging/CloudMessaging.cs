
#if FIREBASE && FCM
using IPS.Api.Notifications;
using System;
#endif

using UnityEngine;

namespace IPS {
    public partial class UserData {
        public static string FCMToken {
            get => GetString("FCMToken");
            set => SetString("FCMToken", value);
        }
    }
}

namespace IPS.Api.IPSFirebase {
    public class CloudMessaging : SingletonBehaviourDontDestroy<CloudMessaging> {
        protected override void OnAwake() {
#if FIREBASE && FCM
            IPSFirebaseCore.Instance.InitModule(InitFirebaseMessaging);
#else
            Debug.Log("Flag FIREBASE & FCM was not set for Firebase Cloud Messaging");
#endif
        }

        private void InitFirebaseMessaging() {
#if FIREBASE && FCM
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
#endif
        }

#if FIREBASE && FCM
        private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
            Debug.Log("[FIREBASE][FCM] Received Registration Token: " + token.Token);
            UserData.FCMToken = token.Token;
        }

        private void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
            var notification = e.Message.Notification;

            if (notification == null) return;

            string chanelID = "channel_id";

            string title = notification.Title;
            string body = notification.Body;
            if (notification.Android != null) {
                chanelID = notification.Android.ChannelId;
            }

            Debug.Log($"[FIREBASE][FCM] Received a new message from: {e.Message.From}, title={title}, body={body}");
            Tracking.Instance.LogEvent("fcm_received", "title", title, service: Tracking.Service.ByteBrew);

            Notifications.PushNotifications.Instance.SendNoti(new Notifications.NotifyInfo(new NotiContent(title, body)));
        }
#endif
    }
}