using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IPS.Api.Ads {
    public class MRecAdsPlacement : MonoBehaviour {
        [SerializeField] string placement;
        [SerializeField] float paddingY = 0;
        [SerializeField] bool hideOnEnable = false;
        [SerializeField] bool showOnFirstTimes = true;
        [SerializeField] GameObject[] ObjsHideWhenMrecDisplay;

        private bool[] objStates;

        private bool registered;
        private bool mrecDisplayed;
        Frame myFrame;

#if ADS
        //#if UNITY_EDITOR
        //        private void OnValidate() {
        //            (transform as RectTransform).anchoredPosition = new Vector2(0, paddingY == 0 ? AdmobSettings.Instance.MrecPaddingY : paddingY);
        //            (transform as RectTransform).sizeDelta = MRecMax.CalculateMRecSizeInPixel();
        //        }
        //#endif

        private void OnEnable() {
            if (!showOnFirstTimes && UserData.PlayTimes == 0) return;
            if (hideOnEnable) (transform as RectTransform).sizeDelta = Vector2.zero;

            if ((ObjsHideWhenMrecDisplay != null && ObjsHideWhenMrecDisplay.Length > 0)
                && (objStates == null || objStates.Length == 0)) {
                objStates = new bool[ObjsHideWhenMrecDisplay.Length];
                for(int i = 0; i < ObjsHideWhenMrecDisplay.Length; ++i) {
                    if (ObjsHideWhenMrecDisplay[i]) objStates[i] = ObjsHideWhenMrecDisplay[i].activeSelf;
                }
            }

            if (objStates != null && objStates.Length > 0) {
                for (int i = 0; i < ObjsHideWhenMrecDisplay.Length; ++i) {
                    if (ObjsHideWhenMrecDisplay[i]) ObjsHideWhenMrecDisplay[i].SetActive(objStates[i]);
                }
            }

            if (!AdsManager.Instance.IsShowingMRec) {
                registered = true;
                AdsManager.Instance.onMRecDisplayed = null;
                AdsManager.Instance.onMRecDisplayed += OnMRecDisplayed;

                AdsManager.Instance.ShowMRec(placement);

                myFrame = GetComponentInParent<Frame>();
                if (myFrame != null) {
                    myFrame.onHideTrigger += HideMRec;
                    Logs.Log($"MRec on frame={myFrame}");
                }
            }
            else {
                AdsManager.Instance.ShowMRec(placement); // call show method but set placement only
                registered = false;
                Resize();
            }
        }

        private void OnDisable() {
            if (!AdsManager.Initialized || !registered) return;
            AdsManager.Instance.onMRecDisplayed -= Resize;
            if (myFrame != null) {
                myFrame.onHideTrigger -= HideMRec;
            }
            
            HideMRec();
        }

        private void HideMRec() {
            if (mrecDisplayed) AdsManager.Instance.HideMRec();
            mrecDisplayed = false;
        }

        private void OnMRecDisplayed() {
            if (registered && !mrecDisplayed) {
                mrecDisplayed = true;
                Resize();
                Logs.Log($"MRecAdsPlacement displayed placement={placement}, size={(transform as RectTransform).sizeDelta}px");
                if (ObjsHideWhenMrecDisplay != null && ObjsHideWhenMrecDisplay.Length > 0) {
                    foreach(var obj in ObjsHideWhenMrecDisplay) {
                        obj.SetActive(false);
                    }
                }
            }
        }

        private void Resize() {
            (transform as RectTransform).sizeDelta = AdsManager.Instance.GetMRecSize();// + new Vector2 (10, 10);
            Canvas.ForceUpdateCanvases();
        }
#endif
    }
}