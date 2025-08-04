using DG.Tweening;
using UnityEngine;

namespace IPS {
    public class BageIcon : MonoBehaviour {
        [SerializeField] BageCondition condition;

        private void OnEnable() {
            CheckCondition();
        }

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        public void CheckCondition() {
            if (condition == null || !condition.CanShowBage) {
                gameObject.SetActive(false);
                return;
            }

            transform.DOKill();
            transform.DOScale(1.1f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        }
    }
}