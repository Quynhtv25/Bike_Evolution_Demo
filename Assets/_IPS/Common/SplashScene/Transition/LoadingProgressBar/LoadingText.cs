using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IPS {
    public class LoadingText : MonoBehaviour {
        [SerializeField] TextMeshProUGUI text;
        int idx = 0;

        string[] message;

        void Start() {
        }

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        public void FadeIn(float duration, float toAlpha=1) {
            text.DOKill();
            text.DOFade(toAlpha, duration);
        }

        public void FadeOut(float duration) {
            text.DOKill();
            text.DOFade(0, duration);
        }

        private void OnEnable() {
            if (message == null || message.Length == 0) {
                message = new string[4];
                message[0] = "Loading";
                message[1] = "Loading.";
                message[2] = "Loading..";
                message[3] = "Loading...";
            }

            idx = 0;
            InvokeRepeating(nameof(UpdateText), 0, .5f);
        }

        
        private void UpdateText() {
            if (text == null) return;
            text.SetText(message[idx]);
            idx++;
            if (idx >= message.Length) idx = 0;
        }
    }
}