#if IAP

using UnityEngine;

namespace IPS {
    public partial class Bootstrap {
        [SerializeField] private bool preloadIAP = true;

        partial void PreloadIAP() {
            if (preloadIAP) {
                IAP.Instance.Preload();
            }
        }
    }
}
#endif