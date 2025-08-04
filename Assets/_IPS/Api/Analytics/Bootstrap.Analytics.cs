using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS {
    public partial class Bootstrap {
        public bool preloadAnalytics = true;

        partial void PreloadAnalytics() {
            if (preloadAnalytics) {
                Tracking.Instance.Preload();
            }
        }
    }
}
