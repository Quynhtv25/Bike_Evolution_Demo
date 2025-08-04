using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RewardAdButton : MonoBehaviour {
    [SerializeField]
    [Tooltip("[Optional] This mask will be active when interactable=false")]
    private Button disableMask;
    [SerializeField] private bool autoTurnOffAtFirst = true;
    [SerializeField] private Image arrowImg;

    Button button;

    private void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(TurnOffIfAdNotReady);

        if (disableMask) disableMask.onClick.AddListener(ShowNoticeAdNotReady);
    }

    protected void Start() {
        //Debug.Log("Button Start");
        if (AdsManager.Initialized) {
            AdsManager.Instance.onRewardVideoAvailable += TurnOnInteractable;
            if (AdsManager.Instance.HasRewardVideo) {
                TurnOnInteractable();
            }
            else if (autoTurnOffAtFirst) {
                TurnOffInteractable();
            }
        }
        else {
            Debug.Log("Ads is not initialize. You should preload ads first.");
            if (autoTurnOffAtFirst) TurnOffInteractable();
        }
    }

    protected void OnDestroy() {
        //Debug.Log("Button OnDestroy");
        if (AdsManager.Initialized) {
            AdsManager.Instance.onRewardVideoAvailable -= TurnOnInteractable;
        }
    }

    private void TurnOffIfAdNotReady() {
        if (!AdsManager.Instance.HasRewardVideo) TurnOffInteractable();
    }

    public void SetEnable(bool interactable) {
        if (interactable) TurnOnInteractable();
        else TurnOffInteractable();
    }

    public void TurnOffInteractable() {
        if (gameObject.activeInHierarchy) {
            if (button != null) button.interactable = false;
            if (disableMask != null) disableMask.gameObject.SetActive(true);
            if (arrowImg != null) arrowImg.gameObject.SetActive(false);
        }
    }

    public void TurnOnInteractable() {
        //Debug.Log("Button enable");
        if (gameObject.activeInHierarchy) {
            if (button != null) button.interactable = true;
            if (disableMask != null) disableMask.gameObject.SetActive(false);
        }
    }

    private void ShowNoticeAdNotReady() {
        NoticeText.Instance.ShowNotice("AD NOT AVAILABLE!");
    }
}