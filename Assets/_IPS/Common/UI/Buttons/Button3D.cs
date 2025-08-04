using UnityEngine;

namespace IPS {
    [RequireComponent(typeof(Collider))]
    public class Button3D : MonoBehaviour {
        [SerializeField] bool canClickThroughUI = false;
        [SerializeField] TMPro.TextMeshPro contentText;
        [SerializeField] GameObject disableMask;

        public bool interactable = true;
        public UnityEngine.Events.UnityEvent onClick;

        private void OnMouseUpAsButton() {
            if (!interactable) return;// || (!canClickThroughUI && InputCtrl.IsPointerOverUI())) return;
            if (onClick != null) onClick.Invoke();
        }

        public void SetContentText(string s) {
            if (contentText != null) contentText.text = s;
        }

        public void SetState(bool interact) {
            interactable = interact;
            if (disableMask != null) disableMask.SetActive(!interact);
        }
    }
}