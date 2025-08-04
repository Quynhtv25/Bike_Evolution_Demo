using IPS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace IPS.Cheat {

    public class DebugSetLevel : Debuger {
        [SerializeField] TMPro.TMP_InputField levelInput;
        [SerializeField] Button playButton, forceWinButton, forceLoseButton;

        public UnityAction forceSetLevelCallback, forceWin, forceLose;

        protected override void OnStart() {
            levelInput.text = UserData.CurrentLevel.ToString();
            playButton.onClick.AddListener(ForceSetLevel);
            forceWinButton.onClick.AddListener(ForceWin);
            forceLoseButton.onClick.AddListener(ForceLose);
            Invoke(nameof(UpdateButtonState), .2f);
        }

        private void OnEnable() {
            UpdateButtonState();
        }

        private void UpdateButtonState() {
            forceWinButton.gameObject.SetActive(forceWin != null);
            forceLoseButton.gameObject.SetActive(forceLose != null);
        }

        private void ForceSetLevel() {
            var value = levelInput.text;
            if (int.TryParse(value, out int level)) {
                if (level > 0) {
                    #if FIREBASE
                    Tracking.Instance.LogLevelCompleted(false, skip: true, reason: "cheat");
#endif
                    UserData.SetLevel(level);
                    if (forceSetLevelCallback != null) forceSetLevelCallback.Invoke();
                    //Transition.Instance.LoadCurrentScene(true, onComplete: () => {
                    //    if (forceSetLevelCallback != null) forceSetLevelCallback.Invoke();
                    //});
                }
                else NoticeText.Instance.ShowNotice("Enter level > 0");
            }
        }

        private void ForceWin() {
            if (forceWin != null) forceWin.Invoke();
        }

        private void ForceLose() {
            if (forceLose != null) forceLose.Invoke();
        }
    }
}