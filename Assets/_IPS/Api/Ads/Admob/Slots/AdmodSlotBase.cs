#if ADMOB
using GoogleMobileAds.Api;
#endif
using System;
using UnityEngine;

namespace IPS.Api.Ads {
    public enum AdSlotFormat { None, AOA, Banner, Inter, Reward, RewardInter, MRec, Native }
    
    internal interface IAd {
        /// <summary> By default, all ad will auto request on load/show failed. Set this to false for manual request other mediation. </summary>
        public bool AutoRequest { get; set; }
        void Request();
        //void Show();
        void Hide();
        void Destroy();
    }

    internal abstract class AdmodSlotBase : IAd {
        public Action onAdLoaded, onAdDisplayed, onAdClose, onAdClicked;
        public Action<AdSlotFormat, string> onAdFailedToLoad, onAdFailedToShow;
        public Action<bool, string> onNewRequest; // failure or close ad
#if ADMOB
        public Action<AdValue, AdSlotFormat, string> onAdPaid;
#endif


        private bool enableLog;

        protected string adSlotId;
        protected bool isRequesting;
        protected int retryCount = 0;

        protected string placement;

        protected float RetryDelayTime => (float)Math.Pow(2, Math.Min(6, retryCount));

        private const string sponsor = "Admob";
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

        public AdmodSlotBase(string adSlotID, bool enableLog) {
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

#if ADMOB
        protected AdRequest CreateAdRequest() {
            return new AdRequest();
        }
#endif

        protected void SafeCallback(Action callback) {
            if (callback == null) return;
            Excutor.Schedule(callback);
        }

        #region Ad Handler
        protected virtual void NewRequest(bool closeAd) {
            if (AutoRequest) Request();
            else Excutor.Schedule(() => {
                if (onNewRequest != null) onNewRequest.Invoke(closeAd, placement);
            });
        }

        protected virtual void OnAdLoaded() {
            Log("OnAdLoaded.");
            retryCount = 0;
            isRequesting = false;
            SafeCallback(onAdLoaded);
        }

#if ADMOB
        protected virtual void OnAdFailedToLoad(AdError err) {
            string errormsg = err != null ? string.Format("{0}_{1}", err.GetCode(), err.GetMessage()) : "UNKNOW";
            LogError($"OnAdFailedToLoad errormsg={errormsg}");

            isRequesting = false;
            retryCount++;

            SafeCallback(() => {
                if (onAdFailedToLoad != null) onAdFailedToLoad.Invoke(AdType, errormsg);
                NewRequest(false);
            });
        }

        protected void OnAdFullScreenContentFailed(AdError error) {
            string errormsg = error != null ? string.Format("{0}_{1}", error.GetCode(), error.GetMessage()) : "UNKNOW";
            LogError(string.Format("OnAdFullScreenContentFailed error={0}", errormsg));
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.

            SafeCallback(() => {
                if (onAdFailedToShow != null) onAdFailedToShow.Invoke(AdType, errormsg);
                if (onAdClose != null) onAdClose.Invoke();
                NewRequest(false);
            });

        }

        protected virtual void OnAdFullScreenContentOpened() {
            Log("OnAdFullScreenContentOpened");
            SafeCallback(onAdDisplayed);
        }

        protected virtual void OnAdFullScreenContentClosed() {
            Log("OnAdFullScreenContentClosed");
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            SafeCallback(onAdClose);
            NewRequest(true);
        }

        protected virtual void OnAdPaid(AdValue adValue) {
            Log(string.Format("OnAdPaid. (value: {0}", adValue.Value));
            Excutor.Schedule(() => {
                if (onAdPaid != null) onAdPaid.Invoke(adValue, AdType, placement);
            });
        }

        protected virtual void OnAdClicked() {            
            Log(string.Format("OnAdClicked"));
            onAdClicked?.Invoke();
        }
#endif
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