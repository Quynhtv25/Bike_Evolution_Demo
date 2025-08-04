#if ADS
using UnityEngine;

namespace IPS {
    public partial class Bootstrap {
        public bool preloadAds = true;

        partial void PreloadAds() {
            if (preloadAds) {
                AdsManager.Instance.Preload();
            }
        }

    }

}
#endif