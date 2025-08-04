using UnityEngine;
using UnityEngine.UI;

namespace IPS.Api.Ads {
    [RequireComponent(typeof(Button))]
    public class  AdsDebuggerButton : MonoBehaviour {

        private void Awake() {
            if (!IPSConfig.CheatEnable && !IPSConfig.LogEnable) {
                gameObject.SetActive(false);
                DestroyImmediate(gameObject);
            }
        }

        protected void Start() {
            GetComponent<Button>().onClick.AddListener(AdsManager.Instance.ShowDebugger);
        }
    }
}
