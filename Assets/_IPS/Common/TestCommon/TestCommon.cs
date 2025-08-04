using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS.Test {
    public class TestCommon : MonoBehaviour {
        public void GoSceneTestAd() {
            Transition.Instance.LoadScene("TestAds", false, true);
        }

        public void GoSceneTestCore() {
            Transition.Instance.LoadScene("TestCommon", true);
        }

        public void ReloadScene() {
            Transition.Instance.LoadCurrentScene(true);
        }

        public void ShowNotice() {
            NoticeText.Instance.ShowNotice("This is a test notice");
        }

        public void ShowNoticeError() {
            NoticeText.Instance.ShowNotice("This is a test notice ERROR", true);
        }

        public void ShowTestCollectEffect() {

        }
    }
}