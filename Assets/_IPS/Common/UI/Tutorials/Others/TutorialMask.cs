using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IPS {
    public class TutorialMask : Frame {
        [SerializeField] RectTransform arrow;

        public void ShowMask(Button target) {
            Show(false);
            var temp = Instantiate(target.gameObject, transform).GetComponent<Button>();
            temp.transform.position = target.transform.position;

            arrow.DOKill();
            arrow.gameObject.SetActive(true);
            arrow.transform.position = temp.transform.position + Vector3.up * 60;
            arrow.DOLocalMoveY(arrow.transform.position.y - 50, .5f).SetLoops(-1, LoopType.Yoyo);

            temp.onClick.AddListener(() => {
                arrow.gameObject.SetActive(false);
                Destroy(temp.gameObject);
                Hide();
            });
        }

    }
}
