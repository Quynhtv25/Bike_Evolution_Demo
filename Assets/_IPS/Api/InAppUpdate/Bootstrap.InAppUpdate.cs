
#if IAD

namespace IPS {
    public partial class Bootstrap {
        public bool preloadInAppUpdate = true;

        partial void PreloadInAppUpdate() {
            if (preloadInAppUpdate) {
                IPSInAppUpdate.Instance.Preload();
            }
        }
    }
}

#endif