using DG.Tweening;
using UnityEngine;

namespace IPS {
    public class BadgeIcon : MonoBehaviour {
        [SerializeField] BadgeCondition condition;

        public BadgeCondition Condition => condition;

        private void Start() {
            condition.Init(this, UpdateStatus);
            UpdateStatus();
        }

        private void OnEnable() {
            UpdateStatus();
        }

        public void UpdateStatus() {
            if (condition == null || !condition.CanShowBadge) {
                gameObject.SetActive(false);
                return;
            }

            transform.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }
}