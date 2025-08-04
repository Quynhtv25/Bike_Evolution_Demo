using UnityEngine;

namespace IPS {
    public class LoadingMask : MonoBehaviour {
        [SerializeField] GameObject mainUI;
        [SerializeField] Transform loadingIcon;
        [SerializeField] TMPro.TextMeshProUGUI contentText;

        private static LoadingMask instance;
        public static LoadingMask Instance {
            get {
                if (instance != null) return instance;
                var obj = Resources.Load<GameObject>($"{typeof(LoadingMask).Name}/{typeof(LoadingMask).Name}");
                if (obj != null ) {
                    instance = GameObject.Instantiate(obj.gameObject).GetComponent<LoadingMask>();
                }
                else {
                    Debug.LogError($"Find not found at Resources/{typeof(LoadingMask).Name}/{typeof(LoadingMask).Name}");
                }
                return instance;
            }
        }

        private void Awake() {
            if (instance != null && instance.GetInstanceID() != this.GetInstanceID()) {
                DestroyImmediate(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
            var canvas = GetComponentInParent<Canvas>();
            if (canvas) {
                canvas.sortingOrder = 900;
                Logs.Log("<color=magenta>Loading Mask set sorting order=900 for editor only. Don't worry in the mobile it is the highest value</color>");
            }
#endif
        }

        private void Update() {
            if (mainUI.activeInHierarchy) {
                loadingIcon.Rotate(0, 0, -8f);
            }
        }

        public void SetContent(string msg) {
            if (contentText) contentText.SetText(msg);
        }

        public void Show() {
            SetContent(string.Empty);
            mainUI.SetActive(true);
            Logs.Log($"[LoadingMask] Show loading mask with no content");
        }

        public void Show(string content) {
            SetContent(content);
            mainUI.SetActive(true);
            Logs.Log($"[LoadingMask] Show loading mask for {content}");
        }

        public void Hide() {
            mainUI.SetActive(false);
        }

    }
}