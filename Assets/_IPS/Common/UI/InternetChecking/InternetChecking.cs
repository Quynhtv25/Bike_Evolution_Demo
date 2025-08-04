using IPS.Api.RemoteConfig;
using System.Collections;
using UnityEngine;

namespace IPS.Api.RemoteConfig {
    public partial class RemoteKey {
        [Header("Internet Config")]
        public string level_internet_require = "level_internet_require";
    }
}

namespace IPS {

    public class InternetChecking : SingletonBehaviourDontDestroy<InternetChecking> {
        const float loopSeconds = 10;
        private float prevTimeScale = 1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeFirstScene() {
            InternetChecking.Instance.Preload();
        }

        protected override void OnAwake() {
            return;
#if REMOTE
            RemoteConfig.Instance.AddDefault(RemoteKey.Instance.level_internet_require, BootstrapConfig.Instance.LevelInternetRequire);
            RemoteConfig.Instance.GetLongAsync(RemoteKey.Instance.level_internet_require, (result) => {
                BootstrapConfig.Instance.LevelInternetRequire = result;
                if (result <= UserData.CurrentLevel) Excutor.Schedule(LoopChecking);
            });
#else
            LoopChecking();
#endif
        }

        private void LoopChecking() {
            if (Application.internetReachability == NetworkReachability.NotReachable) {
                StartCoroutine(IEWaitForInternet());
            }
            else {
                Invoke(nameof(LoopChecking), loopSeconds);
            }
        }

        private void OnNoInternetTriggered() {
            prevTimeScale = Time.timeScale;
            Time.timeScale = 0;
            LoadingMask.Instance.Show("NO INTERNET..");
        }

        private IEnumerator IEWaitForInternet() {
            OnNoInternetTriggered();
            yield return new WaitUntil(() => Application.internetReachability != NetworkReachability.NotReachable);
            Time.timeScale = prevTimeScale;
            LoadingMask.Instance.Hide();
            Invoke(nameof(LoopChecking), loopSeconds);
        }
    }
}