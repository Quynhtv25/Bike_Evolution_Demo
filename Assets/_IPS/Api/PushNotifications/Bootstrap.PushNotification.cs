#if FIREBASE && FCM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS {
    public partial class Bootstrap {
        [SerializeField] bool preloadCloudMessaging = true;
        partial void PreloadCloudMessaging() {
            //IPS.Api.Notifications.PushNotifications.Instance.Preload();
            if (preloadCloudMessaging) {
                IPS.Api.IPSFirebase.CloudMessaging.Instance.Preload();
            }
        }
    }
}
#endif