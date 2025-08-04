using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IPS {
    public class LoadingProgressBar : MonoBehaviour {
        [SerializeField] private Image bg;
        [SerializeField] private LoadingText loadingText;        
        [SerializeField] private Image progressFill;
        [SerializeField] private Image handlerIcon;
        [SerializeField] private RectTransform handerTf;

        private Image Background {
            get {
                if (bg != null) return bg;
                bg = GetComponent<Image>();
                return bg;
            }
        }

        public void Show(bool showText, bool fadeIn = true) {
            gameObject.SetActive(true);
            if (showText) {
                loadingText.Show();
            }
            else loadingText.Hide();
        }

        public void FadeIn(float duration) {
            loadingText.FadeIn(duration);
            progressFill.DOKill();
            if (handlerIcon) handlerIcon.DOKill();
            Background.DOKill();
            progressFill.DOFade(1, duration);
            if (handlerIcon) handlerIcon.DOFade(1, duration);
            Background.DOFade(1, duration);
        }

        public void FadeOut(float duration) {
            loadingText.FadeOut(duration);
            progressFill.DOKill();
            if (handlerIcon) handlerIcon.DOKill();
            Background.DOKill();
            progressFill.DOFade(0, duration);
            if (handlerIcon) handlerIcon.DOFade(0, duration);
            Background.DOFade(0, duration);
        }

        public void Hide() {
            gameObject.SetActive(false);
            loadingText.Hide();
            progressFill.fillAmount = 0;
        }

        public void SetProgress(float progress, bool showText) {
            if (!gameObject.activeSelf) {
                Show(showText);
            }
            progress = Mathf.Clamp(progress, 0, 1);
            progressFill.fillAmount = progress;
            UpdateHandleIcon();
        }

        public void DoFill(float progress, float duration) {
            progressFill.DOKill();
            progressFill.DOFillAmount(progress, duration);
            UpdateHandleIcon();
        }

        public void HideHandler() {
            if (handerTf) handerTf.gameObject.SetActive(false);
        }

        private void UpdateHandleIcon() {
            if (handerTf == null) handerTf = handlerIcon != null ? handlerIcon.rectTransform : null;

            if (handerTf == null) return;

            handerTf.gameObject.SetActive(true);
            handerTf.anchoredPosition = new Vector2(handerTf.rect.x + CurrentProgressOffset, handerTf.anchoredPosition.y);
        }

        public float CurrentProgress => progressFill != null ? progressFill.fillAmount : 0;

        private float CurrentProgressOffset => progressFill != null ? progressFill.fillAmount * progressFill.rectTransform.rect.width : 0;
    }
}