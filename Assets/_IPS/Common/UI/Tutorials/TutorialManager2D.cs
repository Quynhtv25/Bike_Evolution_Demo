using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IPS {
    public class TutorialManager2D : SingletonBehaviourResourcesDontDestroy<TutorialManager2D> {
        [SerializeField] Image maskBg;
        [SerializeField] RectTransform arrowIcon;        

        protected override void OnAwake() {

        }

        public void ShowMask<T, E>(Button target, Action onCompleted = null) where E : IEventParam {
            if (target == null) {
                Logs.LogError($"[Tutorial] ShowMask trigger but NULL target: type={typeof(T).Name}, event={typeof(E).Name}");
                return;
            }

            Logs.Log($"<color=magenta>[Tutorial] Show mask target={target.name}</color>");
            maskBg.gameObject.SetActive(true);

            var temp = Instantiate(target.gameObject, maskBg.transform).GetComponent<Button>();
            temp.transform.SetParent(maskBg.transform, false);
            (temp.transform as RectTransform).pivot = new Vector2(0.5f, .5f);
            temp.transform.position = target.transform.position;//.TransformPoint((target.transform as RectTransform).rect.center);

            arrowIcon.DOKill();
            arrowIcon.gameObject.SetActive(true);
            arrowIcon.transform.position = temp.transform.position + Vector3.up * 100;
            arrowIcon.DOAnchorPosY(arrowIcon.anchoredPosition.y + 50f, .5f).SetLoops(-1, LoopType.Yoyo);
            arrowIcon.transform.SetAsLastSibling();

            temp.onClick.AddListener(() => {
                onCompleted?.Invoke();
                arrowIcon.gameObject.SetActive(false);
                Destroy(temp.gameObject);
                maskBg.gameObject.SetActive(false);
                TutorialSystem.Instance.RemoveStep<T, E>();
            });
        }
    }
}