using UnityEngine;
using System;

namespace IPS.Api.Analytics {
    [CreateAssetMenu(fileName = "AdjustSettings", menuName = "IPS/Api/Analytics/AdjustSettings")]
    public class AdjustSettings : ScriptableObject {
        public string appToken;
        public bool launchDeferredDeeplink = true;
        public string defaultTracker; 
        [Header("iOS CONFIG")]
        public bool adServices = true;
        public bool idfaReading = true;
        public bool skanAttribution = true;
    }
}