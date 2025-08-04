using IPS.Api.Ads;
using System;
using UnityEngine;

namespace IPS {
    public class AOAOpenTrigger : MonoBehaviour {
        [SerializeField] float delayFadeoutAfterAOA = 0.1f;
        public bool autoFadeOut = true;
        public ShowBannerType showBanner = ShowBannerType.EveryReload;

        private static bool firstSession = true;
        public static Action onFadeoutCallback;

        public enum ShowBannerType { NotShow, EveryReload, AfterAoaOnly }

        private bool CanShowAoaFirstInstall => UserData.FirstInstall && AdsManager.Instance.AoaOpenFirstInstallEnable;
        private bool CanShowAoaAppOpen => !UserData.FirstInstall && AdsManager.Instance.AoaOpenEnable;

        private void Start () {
#if ADS
            if (firstSession) {
                if ((CanShowAoaAppOpen || CanShowAoaFirstInstall) && AdsManager.Instance.HasAOA) {
                    AdsManager.Instance.ShowAOA("AppOpen", () => {
                        if (delayFadeoutAfterAOA > 0) {
                            Excutor.Schedule(FadeOutForAOA, delayFadeoutAfterAOA);
                        }
                        else FadeOutForAOA();
                    });
                }
                else FadeOut();
            }
            else FadeOut();
#else
            FadeOut();
#endif
            firstSession = false;
        }

        private void FadeOutForAOA() {
            Logs.Log($"[{typeof(AOAOpenTrigger).Name}] FadeOutWithAOA");
            if (onFadeoutCallback != null) {
                onFadeoutCallback.Invoke();
                onFadeoutCallback = null;
                Logs.Log($"[{typeof(AOAOpenTrigger).Name}] callback on FadeOutWithAOA");
            }

            if (autoFadeOut) {
                Transition.Instance.FadeOut(() => {
                    if (showBanner != ShowBannerType.NotShow) AdsManager.Instance.ShowBanner("home");
                });
            }
        }

        private void FadeOut() {
            Logs.Log($"[{typeof(AOAOpenTrigger).Name}] FadeOut No AOA");
            if (onFadeoutCallback != null) {
                onFadeoutCallback.Invoke();
                onFadeoutCallback = null;
                Logs.Log($"[{typeof(AOAOpenTrigger).Name}] callback on FadeOut No AOA");
            }

            if (autoFadeOut) {
                Transition.Instance.FadeOut(() => {
                    if (showBanner == ShowBannerType.EveryReload) {
                        AdsManager.Instance.ShowBanner("home");
                    }
                });
            }
        }
    }
}