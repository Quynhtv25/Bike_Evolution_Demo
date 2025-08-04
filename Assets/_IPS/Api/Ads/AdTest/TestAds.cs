using UnityEngine;
using IPS;
using IPS.Api.Ads;

public class TestAds : MonoBehaviour {
    [SerializeField] GameObject native3D;
    [SerializeField] GameObject nativeUI;
    [SerializeField] GameObject nativeRawImage;
    [SerializeField] RectTransform mrecContainer;

    private void Awake() {
    }

    public void StartLevel() {
        if (Tracking.CurrentLevel < 2) {
            Tracking.Instance.LogTutStart(Tracking.CurrentLevel.ToString());
            Tracking.Instance.LogProgressStart("Level", Tracking.CurrentLevel.ToString());
        }
        Tracking.Instance.LogLevelStart(Tracking.CurrentLevel);
        NoticeText.Instance.ShowNotice($"Start Level {Tracking.CurrentLevel}");
    }

    public void LevelWin() {
        if (Tracking.CurrentLevel < 2) {
            Tracking.Instance.LogTutEnd(Tracking.CurrentLevel.ToString());
            Tracking.Instance.LogProgressComplete("Level", Tracking.CurrentLevel.ToString());
        }
        Tracking.Instance.LogLevelCompleted(true);
        AdsManager.Instance.ShowInterstitial("test_end_level", () => {
            NoticeText.Instance.ShowNotice($"Win Level {Tracking.CurrentLevel}");
            UserData.IncreaseCurrentLevel();
        });
    }

    public void LevelLose() {
        if (Tracking.CurrentLevel < 2) {
            Tracking.Instance.LogTutEnd(Tracking.CurrentLevel.ToString());
            Tracking.Instance.LogProgressFail("Level", Tracking.CurrentLevel.ToString());
        }
        Tracking.Instance.LogLevelCompleted(false);
        AdsManager.Instance.ShowInterstitial("test_end_level", () => {
            NoticeText.Instance.ShowNotice($"Win Level {Tracking.CurrentLevel}");
        });
    }

    public void OnClickBottomButton() {
        NoticeText.Instance.ShowNotice("Just use for checking whether banner ad overlap UI");
    }

    public void TestCore() {
        Transition.Instance.LoadScene("TestCore", true);
    }

    public void TestSuite() {
        AdsManager.Instance.ShowDebugger();
    }

    #region BANNER
    public void ShowBanner() {
        AdsManager.Instance.ShowBanner("test");
    }

    public void HideBanner() {
        AdsManager.Instance.HideBanner();
    }

    public void DestroyBanner() {
        AdsManager.Instance.DestroyBanner();
    }

    public void ShowBannerAdmob() {
        AdmobMediation.Instance.ShowBanner("test");
    }

    public void DestroyBannerAdmob() {
        AdmobMediation.Instance.DestroyBanner();
    }

    public void ShowBannerIS() {
#if IS
        ISMediation.Instance.ShowBanner("test");
#else
        NoticeText.Instance.ShowNotice("Turn on IS first!", true);
#endif
    }

    public void DestroyBannerIS() {
#if IS
        ISMediation.Instance.DestroyBanner();
#else
        NoticeText.Instance.ShowNotice("Turn on IS first!", true);
#endif
    }

    public void ShowBannerMax() {
#if MAX
        MaxMediation.Instance.ShowBanner("test");
#else
        NoticeText.Instance.ShowNotice("Turn on MAX first!", true);
#endif
    }

    public void DestroyBannerMax() {
#if MAX
        MaxMediation.Instance.DestroyBanner();
#else
        NoticeText.Instance.ShowNotice("Turn on MAX first!", true);
#endif
    }
#endregion BANNER

    #region MREC
    public void ShowMRec() {
        mrecContainer.transform.parent.gameObject.SetActive(true);
        AdsManager.Instance.ShowMRec("test");
    }

    public void HideMRec() {
        AdsManager.Instance.HideMRec();
    }

    public void DestroyMRec() {
        mrecContainer.transform.parent.gameObject.SetActive(false);
        AdsManager.Instance.DestroyMRec();
    }

    public void ShowMRecAdmob() {
        mrecContainer.transform.parent.gameObject.SetActive(true);
        AdmobMediation.Instance.ShowMRec("test");
    }

    public void DestroyMRecAdmob() {
        mrecContainer.transform.parent.gameObject.SetActive(false);
        AdmobMediation.Instance.DestroyMRec();
    }

    public void ShowMRecIS() {
#if IS
        mrecContainer.transform.parent.gameObject.SetActive(true);
        ISMediation.Instance.ShowMRec("test");
#else
        NoticeText.Instance.ShowNotice("Turn on IS first!", true);
#endif
    }

    public void DestroyMRecIS() {
#if IS
        mrecContainer.transform.parent.gameObject.SetActive(false);
        ISMediation.Instance.DestroyMRec();
#else
        NoticeText.Instance.ShowNotice("Turn on IS first!", true);
#endif
    }

    public void ShowMRecMax() {
#if MAX
        mrecContainer.transform.parent.gameObject.SetActive(true);
        MaxMediation.Instance.ShowMRec("test");
#else
        NoticeText.Instance.ShowNotice("Turn on MAX first!", true);
#endif
    }

    public void DestroyMRecMax() {
#if MAX
        mrecContainer.transform.parent.gameObject.SetActive(false);
        MaxMediation.Instance.DestroyMRec();
#else
        NoticeText.Instance.ShowNotice("Turn on MAX first!", true);
#endif
    }
    #endregion

    public void SetRemoveAds() {
        AdsManager.Instance.SetRemovedAds();
    }

    public void ShowInterstitial() {
        AdsManager.Instance.ShowInterstitial("test", () => {
            Debug.Log("[Test Ad] Interstitial completed.");
        });
    }

    public void ShowInterstitialAdmob() {
#if ADS
        AdmobMediation.Instance.ShowInterstitial("test", () => {
            Debug.Log("[Test Ad] Interstitial completed.");
        });
#endif
    }

    public void ShowInterstitialIS() {
#if IS
        ISMediation.Instance.ShowInterstitial("test", () => {
            Debug.Log("[Test Ad] Interstitial completed.");
        });
#else
        NoticeText.Instance.ShowNotice("Turn on IS first!", true);
#endif
    }

    public void ShowInterstitialMax() {
#if MAX
        MaxMediation.Instance.ShowInterstitial("test", () => {
            Debug.Log("[Test Ad] Interstitial completed.");
        });
#else
        NoticeText.Instance.ShowNotice("Turn on MAX first!", true);
#endif
    }

    public void ShowRewardInterstitial() {
        AdsManager.Instance.ShowRewardInterstitial("test", () => {
            Debug.Log("[Test Ad] RewardInterstitialAd earned reward.");
        }, () => {
            Debug.Log("[Test Ad] RewardInterstitialAd close.");
        });
    }

    public void ShowAOA() {
        AdsManager.Instance.ShowAOA("Test", () => {
            Debug.Log("[Test Ad] AOA close.");
        });
    }

    public void ShowRewardVideo() {
        AdsManager.Instance.ShowRewardVideo("test", () => {
            Debug.Log("[Test Ad] RewardVideoAd earned reward.");
        }, () => {
            Debug.Log("[Test Ad] RewardVideoAd close.");
        });
    }

    public void ShowRewardVideoAdmob() {
#if ADS
        AdmobMediation.Instance.ShowRewardVideo("test", () => {
            Debug.Log("[Test Ad] RewardVideoAd earned reward.");
        }, () => {
            Debug.Log("[Test Ad] RewardVideoAd close.");
        });
#else
        NoticeText.Instance.ShowNotice("Turn on ADS first!", true);
#endif
    }

    public void ShowRewardVideoIS() {
#if IS
        ISMediation.Instance.ShowRewardVideo("test", () => {
            Debug.Log("[Test Ad] RewardVideoAd earned reward.");
        }, () => {
            Debug.Log("[Test Ad] RewardVideoAd close.");
        });
#else
        NoticeText.Instance.ShowNotice("Turn on IS first!", true);
#endif
    }

    public void ShowRewardVideoMax() {
#if MAX
        MaxMediation.Instance.ShowRewardVideo("test", () => {
            Debug.Log("[Test Ad] RewardVideoAd earned reward.");
        }, () => {
            Debug.Log("[Test Ad] RewardVideoAd close.");
        });
#else
        NoticeText.Instance.ShowNotice("Turn on MAX first!", true);
#endif
    }

    public void ShowNative_UI() {
        nativeRawImage.SetActive(false);
        native3D.SetActive(false);
        nativeUI.SetActive(!nativeUI.activeSelf);
    }

    public void ShowNative_RawImage() {
        nativeUI.SetActive(false);
        native3D.SetActive(false);
        nativeRawImage.SetActive(!nativeRawImage.activeSelf);
    }

    public void ShowNative_3D() {
        nativeUI.SetActive(false);
        nativeRawImage.SetActive(false);
        native3D.SetActive(!native3D.activeSelf);
    }
}