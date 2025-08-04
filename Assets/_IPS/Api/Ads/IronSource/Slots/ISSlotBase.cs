#if IS
using System;
using UnityEngine;

namespace IPS.Api.Ads {

    internal abstract class ISSlotBase : IAd {
        public Action onAdLoaded, onAdDisplayed, onAdClose, onAdClicked;
        public Action<AdSlotFormat, string> onAdFailedToLoad, onAdFailedToShow;
        public Action<IronSourceAdInfo, AdSlotFormat, string> onAdPaid;
        public Action<bool, string> onNewRequest; // closeAd or fail

        private bool enableLog;

        protected string adSlotId;
        protected bool isRequesting;
        protected int retryCount = 0;
        
        private string placement;

        protected float RetryDelayTime => (float)Math.Pow(2, Math.Min(6, retryCount));

        private const string sponsor = "IronSource";
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

        public ISSlotBase(bool enableLog) {
            //if (string.IsNullOrEmpty(adSlotID)) {
            //    LogError("slot id was empty!");
            //}

            Log("Initialized");
            //this.adSlotId = adSlotID;
            this.enableLog = enableLog;
        }
        
        public void SetLogEnable(bool enable) {
            this.enableLog = enable;
        }

        protected void SetPlacement(string placement) {
            if (!string.IsNullOrEmpty(placement)) this.placement = placement;
        }

        public virtual void Request() {
            if (isRequesting/* || string.IsNullOrEmpty(adSlotId)*/) return;
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

        protected virtual void OnAdLoaded(IronSourceAdInfo ad) {
            if (ad == null) return;
            Log($"OnAdLoaded adInfo={ad}.");
            retryCount = 0;
            isRequesting = false;
            SafeCallback(onAdLoaded);
        }

        protected virtual void OnAdFailedToLoad(IronSourceError err) {
            if (err ==null) return;
            string errormsg = err != null ? string.Format("{0}_{1}", err.getErrorCode(), err.getDescription()) : "UNKNOW";
            LogError($"OnAdFailedToLoad errormsg={errormsg}");

            isRequesting = false;
            retryCount++;

            SafeCallback(() => {
                if (onAdFailedToLoad != null) onAdFailedToLoad.Invoke(AdType, errormsg);
                NewRequest(false);        
            });
        }

        protected void OnAdFullScreenContentFailed(IronSourceError error, IronSourceAdInfo ad) {
            string errormsg = error != null ? string.Format("{0}_{1}", error.getErrorCode(), error.getDescription()) : "UNKNOW";
            LogError($"OnAdFullScreenContentFailed error={errormsg} adInfo={ad}");
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.

            SafeCallback(() => {
                if (onAdFailedToShow != null) onAdFailedToShow.Invoke(AdType, errormsg);
                if (onAdClose != null) onAdClose.Invoke();
                NewRequest(false);
            });

        }

        protected virtual void OnAdFullScreenContentOpened(IronSourceAdInfo ad) {
            if (ad == null) return;
            Log("OnAdFullScreenContentOpened");
            SafeCallback(onAdDisplayed);
        }

        protected virtual void OnAdFullScreenContentClosed(IronSourceAdInfo ad) {
            if (ad == null) return;
            Log("OnAdFullScreenContentClosed");
            SafeCallback(onAdClose);
            NewRequest(true);
        }

        protected virtual void OnAdPaid(IronSourceAdInfo ad) {
            if (ad ==null) return;
            Log(string.Format("OnAdPaid. (value: {0}", ad.revenue));
            Excutor.Schedule(() => {
                if (onAdPaid != null) onAdPaid.Invoke(ad, AdType, placement);
            });
        }

        protected virtual void OnAdClick(IronSourceAdInfo ad) {
            Log(string.Format("OnAdClicked"));
            onAdClicked?.Invoke();
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