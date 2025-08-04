using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace IPS {
    public class ToggleEffect : ButtonEffect {
        [Header("Toggle")]
        [SerializeField] bool state = true;
        [SerializeField] RectTransform dotIcon;
        [SerializeField] float dotPadding = 0;
        [SerializeField] Color onColor = Color.white;
        [SerializeField] Color offColor = Color.white;
        [SerializeField] Sprite onSprite, offSprite;

        public bool IsOn => state;
        public bool State => state;

        private void OnValidate() {
            Button.image.overrideSprite = state ? onSprite : offSprite;
            Button.image.color = state ? onColor : offColor;
            if (dotIcon) dotIcon.anchoredPosition = new Vector2((dotIcon.sizeDelta.x * .6f - dotPadding) * (state ? 1 : -1), 0);
        }

        public void SetState(bool on) {
            this.state = on;
            Button.image.overrideSprite = state ? onSprite : offSprite;
            Button.image.color = state ? onColor : offColor;
            if (dotIcon) {
                dotIcon.DOKill();
                dotIcon.DOAnchorPosX((dotIcon.sizeDelta.x * .6f - dotPadding) * (state ? 1 : -1), .1f);
            }
        }

        protected override void OnClickButton() {
            //SetState(!state);
        }
    }
}