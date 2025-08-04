using UnityEngine;
using UnityEngine.UI;

namespace IPS.Api.Ads {
    [RequireComponent(typeof(Button))]
    public class  AdsTestSuiteButton : Debuger {
        protected override void OnStart() {
#if ADS
            GetComponent<Button>().onClick.AddListener(AdsManager.Instance.ShowDebugger);
#endif
        }
    }
}
