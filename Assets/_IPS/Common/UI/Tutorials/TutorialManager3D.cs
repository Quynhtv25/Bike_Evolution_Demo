using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IPS {
    public class TutorialManager3D : SingletonBehaviourResourcesDontDestroy<TutorialManager3D> {
        [SerializeField] GameObject maskBg;
        [SerializeField] RectTransform unmaskRect;
        [SerializeField] Button unMaskButton;
        [SerializeField] RectTransform arrowIcon;


        protected override void OnAwake() {
        }

        private Action onStepCompleted;
        public void RunStep<T, E>(Renderer target, Action onStepInteract, Action onCompleted) where E : IEventParam {
            if (target == null) {
                Logs.LogError($"[Tutorial] ShowMask trigger but NULL target: type={typeof(T).Name}, event={typeof(E).Name}");
                return;
            }

            Logs.Log($"<color=magenta>[Tutorial] Show mask target={target.name}_{target.GetInstanceID()}</color>");
            maskBg.gameObject.SetActive(true);

            Vector2 targetScreenPos = Camera.main.WorldToScreenPoint(target.transform.position);
            Vector3 boundSize = target.bounds.size;

            Vector3 topRight = Camera.main.WorldToScreenPoint(target.transform.position + boundSize * .5f);
            Vector3 bottomLeft = Camera.main.WorldToScreenPoint(target.transform.position - boundSize * .5f);
            Vector2 size = new Vector2(Mathf.Abs(topRight.x - bottomLeft.x), Mathf.Abs(topRight.y - bottomLeft.y));

            unmaskRect.position = targetScreenPos;
            unmaskRect.sizeDelta = size;

            arrowIcon.DOKill();
            arrowIcon.gameObject.SetActive(true);
            arrowIcon.transform.position = unmaskRect.transform.position + Vector3.up * 50;
            arrowIcon.DOAnchorPosY(arrowIcon.anchoredPosition.y + 50f, .5f).SetLoops(-1, LoopType.Yoyo);
            arrowIcon.transform.SetAsLastSibling();

            unMaskButton.onClick.RemoveAllListeners();
            unMaskButton.onClick.AddListener(() => {
                onStepInteract?.Invoke();
            });

            this.onStepCompleted = onCompleted;

            EventDispatcher.Instance.AddListener<E>(DoneStep);
        }

        private void DoneStep<E>(E param) where E : IEventParam {
            onStepCompleted?.Invoke();
            onStepCompleted = null;
            maskBg.gameObject.SetActive(false);
            EventDispatcher.Instance.RemoveListener<E>(DoneStep);
        }
    }
}