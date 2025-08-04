using IPS;
using System;
using UnityEngine;

namespace IPS.UI {
    /// <summary>
    /// You should create other script extend from this for SettingIngame & SettingHome (ingame maybe has a quit button).
    /// </summary>
    public class SettingPopup : Frame {
        [SerializeField] private ToggleEffect btnMusic;
        [SerializeField] private ToggleEffect btnSound;
        [SerializeField] private ToggleEffect btnVibrate;
        [SerializeField] private ButtonEffect bntClose;

        private void Start() {
            btnMusic.AddListener(OnClick_MusicButton);
            btnSound.AddListener(OnClick_SoundButton);
            btnVibrate.AddListener(OnClick_VibrateButton);
            bntClose.AddListener(OnClick_CloseButton);
            UpdateUI();
        }

        private void OnEnable() {
            TrackingUI();
            UpdateUI();
        }
        protected virtual void TrackingUI() { }
        protected virtual void UpdateUI() {
            btnMusic.SetState(SFX.Instance.MusicEnable);
            btnSound.SetState(SFX.Instance.SoundEnable);
            btnVibrate.SetState(SFX.Instance.VibrateEnable);
        }
        private void OnClick_MusicButton() {
            SFX.Instance.MusicEnable = !SFX.Instance.MusicEnable;
            btnMusic.SetState(SFX.Instance.MusicEnable);
        }

        private void OnClick_SoundButton() {
            SFX.Instance.SoundEnable = !SFX.Instance.SoundEnable;
            btnSound.SetState(SFX.Instance.SoundEnable);
        }

        private void OnClick_VibrateButton() {
            SFX.Instance.VibrateEnable = !SFX.Instance.VibrateEnable;
            btnVibrate.SetState(SFX.Instance.VibrateEnable);
        }

        protected virtual void OnClick_CloseButton() {
            Hide();
        }
    }
}
