using UnityEngine;
using System.Collections;

namespace IPS.Api.Analytics {
    [CreateAssetMenu(fileName = "AppMetricaSettings", menuName = "IPS/Api/Analytics/AppMetricaSettings")]
    public class AppMetricaSettings : ScriptableObject {
        [SerializeField] string apiKey;

        public string ApiKey => apiKey;

        //public string AppID {
        //    get {
        //        if (IPSConfig.IsAndroid) {
        //            return string.Empty;
        //        }
        //        else {
        //            return appId_iOS;
        //        }
        //    }
        //}
    }
}