#if FIREBASE && REMOTE
using IPS.Api.RemoteConfig;
using System.Linq;
using UnityEngine;

namespace IPS {
    public partial class Bootstrap {
        [SerializeField] private bool preloadRemoteConfig = true;
        partial void PreloadRemoteConfig() {
            if (preloadRemoteConfig) {
                RemoteConfig.Instance.Preload();
                try {
                    FetchRemoteConfigDebug();
                } catch { }
                
                try {
                    FetchRemoteConfigAds();
                } catch { }
                
                try {
                    FetchRemoteConfigIap();
                }catch { }

                try {
                    FetchRemoteConfigGames();
                }catch{ }
            }
        }

        partial void FetchRemoteConfigAds();
        partial void FetchRemoteConfigIap();
        partial void FetchRemoteConfigGames();

        private void FetchRemoteConfigDebug() {
            if (BootstrapConfig.Instance.IsTester) return;

            RemoteConfig.Instance.GetStringAsync(RemoteKey.Instance.debug_devices, value => {
                if (!string.IsNullOrEmpty(value)) {
                    string[] list = value.Trim().Split(',');
                    if (list != null && list.Length > 0) {
                        if (!string.IsNullOrEmpty(BootstrapConfig.Instance.MyGAID) && list.Contains(BootstrapConfig.Instance.MyGAID)
                            || (list.Contains(SystemInfo.deviceUniqueIdentifier))) {
                            BootstrapConfig.Instance.IsTester = true;
                            TurnOnFullLog();
                        }
                    }
                }
            });
        }
    }
}
#endif