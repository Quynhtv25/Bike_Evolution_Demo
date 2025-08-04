using UnityEngine;
using UnityEngine.UI;

namespace IPS {
    /// <summary>
    /// Create an empty game object in your game scene, then add this to it, drag the main canvas into "uiToHide"
    /// </summary>
    public class HideAllUI : MonoBehaviour {
        [Tooltip("[Optional] Create a new canvas as child of this with highest overlay sorting order")]
        [SerializeField] Button btnClickToHide;
        [SerializeField] int touchCountTrigger = 3;
        [SerializeField] float timeCapping = 1f;
        [SerializeField] GameObject[] uiToHide;
        private float lastTouchedTime;

        private void Awake() {
            if (!IPSConfig.CheatEnable) DestroyImmediate(gameObject);
            if (btnClickToHide) btnClickToHide.onClick.AddListener(ForceClickTrigger);
        }

        void Update() {
            if (forceTriggerCount > 0) return;

            Touch[] touches = Input.touches;
            if (touches.Length >= touchCountTrigger) {
                if (Time.time - lastTouchedTime >= timeCapping) {
                    foreach (var obj in uiToHide) {
                        obj.SetActive(!obj.activeSelf);
                    }
                }
                lastTouchedTime = Time.time;
            }
        }

        private int forceTriggerCount = 0;
        private void ForceClickTrigger() {
            if ((forceTriggerCount == 0 || (Time.time - lastTouchedTime) < timeCapping)) {
                lastTouchedTime = Time.time;
                forceTriggerCount++;
            }
            else {
                forceTriggerCount = 0;
                ForceClickTrigger();
                return;
            }

            if (forceTriggerCount >= touchCountTrigger) {
                forceTriggerCount = 0;
                foreach (var o in uiToHide) {
                    if (o) o.SetActive(!o.activeSelf);
#if ADS
                    if (!o.activeSelf) AdsManager.Instance.DestroyBanner();
#endif
                }
            }
        }
    }
}
