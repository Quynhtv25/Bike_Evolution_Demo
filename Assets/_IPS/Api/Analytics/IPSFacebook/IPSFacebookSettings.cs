using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IPS.Api.Analytics {
    [CreateAssetMenu(fileName = "IPSFacebookSettings", menuName = "IPS/Api/Analytics/FacebookSettings")]
    public class IPSFacebookSettings : ScriptableObject {
        [Header("Android")]
        [SerializeField] string appName_android;
        [SerializeField] string appId_android;
        [SerializeField] string clientToken_android;
        [Header("iOS")]
        [SerializeField] string appName_iOS;
        [SerializeField] string appId_iOS;
        [SerializeField] string clientToken_iOS;

        public List<string> AppNames {
            get {
                var list = new List<string>() { appName_android };
                if (!string.IsNullOrEmpty(appName_iOS)) list.Add(appName_iOS);
                return list;
            }
        }

        public List<string> AppIds {
            get {
                var list = new List<string>() { appId_android };
                if (!string.IsNullOrEmpty(appId_iOS)) list.Add(appId_iOS);
                return list;
            }
        }

        public List<string> ClientTokens {
            get {
                var list = new List<string>() { clientToken_android };
                if (!string.IsNullOrEmpty(clientToken_iOS)) list.Add(clientToken_iOS);
                return list;
            }
        }

        public int ActiveIndex {
            get {
                if (!string.IsNullOrEmpty(appId_android) && !string.IsNullOrEmpty(appId_iOS)) return 1;
#if UNITY_IOS
#endif
                return 0;
            }
        }
    }
}