#if MAX
using System;
using UnityEngine;

namespace IPS.Api.Ads {

    internal abstract class MaxSlotBase : IAd {
        public Action onAdLoaded, onAdDisplayed, onAdClose, onAdClicked;
        public Action<AdSlotFormat, string> onAdFailedToLoad, onAdFailedToShow;
        public Action<MaxSdkBase.AdInfo, AdSlotFormat, string> onAdPaid;
        public Action<bool, string> onNewRequest; // closeAd or failure

        private bool enableLog;

        protected string adSlotId;
        protected bool isRequesting;
        protected int retryCount = 0;

        protected string placement;

        protected float RetryDelayTime => (float)Math.Pow(2, Math.Min(6, retryCount));

        private const string sponsor = "Max";
        protected virtual string AdSponsor => sponsor;
        protected abstract AdSlotFormat AdType { get; }

        protected abstract bool IsLoaded { get; }
        
        private bool _autoRequest = false;
        public bool AutoRequest {
            get => _autoRequest;
            set {
                _autoRequest = value;
                Log($"AutoRequest={value}");
            }
        }


        public virtual bool IsAvailable { 
            get {
                if (IsLoaded) return true;
                if (AutoRequest) Request();
                return false; 
            }
        }

        public MaxSlotBase(string adSlotID, bool enableLog) {
            if (string.IsNullOrEmpty(adSlotID)) {
                LogError("slot id was empty!");
            }

            Log("Initialized");
            this.adSlotId = adSlotID;
            this.enableLog = enableLog;
        }
        
        public void SetLogEnable(bool enable) {
            this.enableLog = enable;
        }

        protected void SetPlacement(string placement) {
            if (!string.IsNullOrEmpty(placement)) this.placement = placement;
        }

        public virtual void Request() {
            if (isRequesting || string.IsNullOrEmpty(adSlotId)) return;
            float time = RetryDelayTime;
            isRequesting = true;
            Log($"Request after {time}s, retry={retryCount}");
            Excutor.Schedule(DoRequest, time, true);
        }

        protected abstract void DoRequest();
        //public abstract void Show();
        public abstract void Hide();
        public abstract void Destroy();

        protected void SafeCallback(Action callback) {
            if (callback == null) return;
            Excutor.Schedule(callback);
        }

        #region Ad Handler
        protected virtual void NewRequest(bool closeAd) {
            if (AutoRequest) Request();
            else Excutor.Schedule(() => onNewRequest.Invoke(closeAd, placement));
        }

        protected virtual void OnAdLoaded(string adUnitId, MaxSdkBase.AdInfo ad) {
            if (!adUnitId.Equals(adSlotId)) return;
            Log($"OnAdLoaded unitId={adUnitId} adInfo={ad}.");
            retryCount = 0;
            isRequesting = false;
            SafeCallback(onAdLoaded);
        }

        protected virtual void OnAdFailedToLoad(string adUnitId, MaxSdkBase.ErrorInfo err) {
            if (!adUnitId.Equals(adSlotId)) return;
            string errormsg = err != null ? string.Format("{0}_{1}", err.Code, err.Message) : "UNKNOW";
            LogError($"OnAdFailedToLoad unitId={adUnitId} errormsg={errormsg}");

            isRequesting = false;
            retryCount++;

            SafeCallback(() => {
                if (onAdFailedToLoad != null) onAdFailedToLoad.Invoke(AdType, errormsg);
                NewRequest(false);          
            });
        }

        protected void OnAdFullScreenContentFailed(string adUnitId, MaxSdkBase.ErrorInfo error, MaxSdkBase.AdInfo ad) {
            if (!adUnitId.Equals(adSlotId)) return;
            string errormsg = error != null ? string.Format("{0}_{1}", error.Code, error.Message) : "UNKNOW";
            LogError(string.Format("OnAdFullScreenContentFailed unitId={0} error={1} adInfo={2}", adUnitId, errormsg, ad));
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.

            SafeCallback(() => {
                if (onAdFailedToShow != null) onAdFailedToShow.Invoke(AdType, errormsg);
                if (onAdClose != null) onAdClose.Invoke();
                NewRequest(false);
            });

        }

        protected virtual void OnAdFullScreenContentOpened(string adUnitId, MaxSdkBase.AdInfo ad) {
            if (!adUnitId.Equals(adSlotId)) return;
            Log(string.Format("OnAdFullScreenContentOpened placement={0}", placement));
            SafeCallback(onAdDisplayed);
        }

        protected virtual void OnAdFullScreenContentClosed(string adUnitId, MaxSdkBase.AdInfo ad) {
            if (!adUnitId.Equals(adSlotId)) return;
            Log(string.Format("OnAdFullScreenContentClosed placement={0}", placement));
            SafeCallback(onAdClose);
            NewRequest(true);
        }

        protected virtual void OnAdPaid(string adUnitId, MaxSdkBase.AdInfo ad) {
            if (!adUnitId.Equals(adSlotId)) return;
            Log(string.Format("OnAdPaid. (value: {0}", ad.Revenue));
            Excutor.Schedule(() => {
                if (onAdPaid != null) onAdPaid.Invoke(ad, AdType, placement);
            });
        }

        protected virtual void OnAdClicked(string adUnityId, MaxSdkBase.AdInfo ad) {
            Log(string.Format("OnAdClicked placement={0}", placement));
            SafeCallback(onAdClicked);
        }
        #endregion Ad Handler

        #region Log
        protected void Log(string msg) {
            if (enableLog) {
                Debug.Log($"[Ads.{AdSponsor}.{AdType}] {msg}");
            }
        }

        protected void LogError(string msg) {
            Debug.LogError($"[Ads.{AdSponsor}.{AdType}] {msg}");
        }
        #endregion
    }
}
#endif