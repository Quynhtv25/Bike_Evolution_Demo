using UnityEngine;
using UnityEngine.UI;
using IPS;
using System;
using IPS.Api.RemoteConfig;
using System.Security.Cryptography;

namespace IPS.Api.RemoteConfig {
    public partial class RemoteKey {
        [Header("In App Review Config")]
        public string rating_popup_enable = "rating_popup_enable";
        public string rating_popup_level_capping = "rating_popup_level_capping";
    }
}

public class RatePopup : SingletonBehaviourResourcesDontDestroy<RatePopup> {
    [Tooltip("id as string number of the game in the apple store, remove prefix 'id' ")]
    [SerializeField] string appleStoreId = "123456789";
    [SerializeField] GameObject mainPopup;
    [SerializeField] [Tooltip("Optional")] Button rateButton;
    [SerializeField] Button closeButton;
    [SerializeField] Material grayScale;
    [Tooltip("Optional, you can use only star on sprite, then set off sprite is null, it will use grayscale instead")]
    [SerializeField] Sprite starOnSprite, starOffSprite;
    [SerializeField] Image[] rateStars;

    public static bool CanShow { get; private set; } = true;
    private static int showAtSesionTimes = 5;
    private static int currentSessionCount = 0;
    private int starSelectedIdx = 0;

    private Action onCloseCallback;

    public bool IsShowing => mainPopup.activeInHierarchy;

    private bool Rated {
        get => UserData.HasKey("UserRated");
        set => UserData.SetBool("UserRated", true);
    }

    protected override void OnAwake() {
        RequestRemoteConfig();
        mainPopup.gameObject.SetActive(false);
    }

    /// <summary> Call this only one times if you want to fetch value from remote config </summary>
    private void RequestRemoteConfig() {
#if FIREBASE && REMOTE
        RemoteConfig.Instance.AddDefault(RemoteKey.Instance.rating_popup_enable, CanShow);
        RemoteConfig.Instance.AddDefault(RemoteKey.Instance.rating_popup_level_capping, showAtSesionTimes);
        RemoteConfig.Instance.GetBoolAsync(RemoteKey.Instance.rating_popup_enable, (result) => CanShow = result);
        RemoteConfig.Instance.GetLongAsync(RemoteKey.Instance.rating_popup_level_capping, (result) => showAtSesionTimes = (int)result);
#endif
    }

    private void Start() {
        closeButton.onClick.AddListener(OnClickCloseButton);
        if (rateButton) rateButton.onClick.AddListener(OnClickRateButton);
    }

    private void ShowCloseButton() {
        closeButton.gameObject.SetActive(true);
    }

    public bool ShowIfAvailable(Action callback = null, bool forceShow = false) {
        currentSessionCount++;
        Logs.Log($"Rate Popup CanShow={CanShow}: sesstionTimes={currentSessionCount}, target={showAtSesionTimes}");

        onCloseCallback = callback;
        if (!CanShow) {
            callback?.Invoke();
            return false;
        }

        if (forceShow) {
            mainPopup.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(false);
            if (rateButton) rateButton.interactable = false;
            Invoke(nameof(ShowCloseButton), 1);
            return true;
        }
        else if (!Rated && currentSessionCount == showAtSesionTimes - 1) {
            Logs.Log($"Rate Popup Show sesstionTimes={currentSessionCount}, target={showAtSesionTimes}");
            mainPopup.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(false);
            if (rateButton) rateButton.interactable = false;
            Invoke(nameof(ShowCloseButton), 1);
            return true;
        }

        if (onCloseCallback != null) {
            onCloseCallback.Invoke();
            onCloseCallback = null;
        }
        return false;
    }

    private void OnEnable() {
        foreach (var rate in rateStars) {
            //rate.color = Color.white;
            rate.raycastTarget = true;

            if (starOffSprite != null) {
                rate.sprite = starOffSprite;
            }
            else {
                rate.material = grayScale;
            }
        }

        //rateStars[rateStars.Length - 1].DOColor(rateColor, 1).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnClickCloseButton() {
        onCloseCallback?.Invoke();
        onCloseCallback = null;
        mainPopup.gameObject.SetActive(false);
    }

    private void OnClickRateButton() {
        if (starSelectedIdx < rateStars.Length - 1) {
            OnClickCloseButton();
            return;
        }

#if ADS
        AdsManager.PauseInsideApp = true;
#endif
#if IAR
        InAppReview.Instance.RequestNativeReview(() => Rated = true);
#else
        Rated = true;
#if UNITY_ANDROID
            Application.OpenURL(string.Format("market://details?id={0}", Application.identifier));
#else
            Application.OpenURL(string.Format("itms-apps://itunes.apple.com/app/id{0}", appleStoreId));
#endif

#endif
        OnClickCloseButton();
    }

    public void OnClickStartButton(int index) {
        starSelectedIdx = index;

        for (int i = 0; i < rateStars.Length; i++) {
            if (!rateButton || !rateButton.gameObject.activeInHierarchy) {
                rateStars[i].raycastTarget = false;
            }
            //rateStars[i].DOKill();
            if (i <= index) {
                rateStars[i].material = default;
                if (starOnSprite != null) {
                    rateStars[i].sprite = starOnSprite;
                }
            }
            else {
                if (starOffSprite != null) {
                    rateStars[i].sprite = starOffSprite;
                }
                else rateStars[i].material = grayScale;
            }
        }

        if (rateButton && rateButton.gameObject.activeInHierarchy) {
            rateButton.interactable = true;
        }
        else {
            Invoke(nameof(OnClickRateButton), 1f);
        }
    }

#if UNITY_ANDROID
    private void Update() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            OnClickCloseButton();
        }
    }
#endif
}
