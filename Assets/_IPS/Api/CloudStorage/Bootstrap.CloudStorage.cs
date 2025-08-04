#if FIREBASE && FCS
using UnityEngine;

namespace IPS {
    public partial class Bootstrap {
        [SerializeField] private bool preloadCloudStorage = true;
        partial void PreloadCloudStorage() {
            if (preloadCloudStorage) {
                CloudStorage.Instance.Preload();
            }
        }
    }
}
#endif