#if APPMETRICA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Io.AppMetrica;
using AppLovinMax.Scripts.IntegrationManager.Editor;
using UnityEditor.Media;

namespace IPS.Api.Analytics {
    public class IPSAppMetricaAnalytic : SingletonBehaviourDontDestroy<IPSAppMetricaAnalytic> {
        private bool sdkInitialized;

        protected override void OnAwake() {
            Init();
        }

        private void Init() {
            var settings = Resources.Load<AppMetricaSettings>($"{typeof(AppMetricaSettings).Name}/{typeof(AppMetricaSettings).Name}");
            if (settings == null) {
                Logs.LogError($"[AppMetrica] Cannot load setting from resources path: {typeof(AppMetricaSettings).Name}/{typeof(AppMetricaSettings).Name}");
                return;
            }

            AppMetrica.Activate(new AppMetricaConfig(settings.ApiKey) {
                FirstActivationAsUpdate = !UserData.FirstInstall,
            });

            sdkInitialized = true;

            Logs.Log($"<color=green>[AppMetrica] Initialized: SDK Version={AppMetrica.GetLibraryVersion()}</color>");
        }

        /// <summary>Need to turn on field 'UseCustomUserId' in the AppMetricaSettings first</summary>
        public void SetUserId(string id) {
            AppMetrica.SetUserProfileID(id);
            Logs.Log($"<color=green>[AppMetrica] SetUserId={id}</color>");
        }

        public void LogAppOpenDeepLink(string deeplink) {
            AppMetrica.ReportAppOpen(deeplink);
            Push();
        }

        public void LogEvent(string eventName) {
            this.Log(eventName, null);
        }

        public void Log(string eventName, ParameterBuilder para) {
            string json = string.Empty;
            if (para != null) json = JsonUtility.ToJson(para);

            Logs.Log($"[AppMetrica] eventName={eventName} with para={json}");

            if (UnityEngine.Application.isEditor) {
                return;
            }
            if (para == null) AppMetrica.ReportEvent(eventName);

            else AppMetrica.ReportEvent(eventName, json);
            Push();
        }

        public void LogAdRevenue(int mediation, string network, double revenue, string currency, ParameterBuilder parameter) {
            Logs.Log($"[AppMetrica] logAdRevenue, mediation={mediation}, network={network}, revenue={revenue}{currency}");
            if (UnityEngine.Application.isEditor) {
                return;
            }

            try {
                AppMetrica.ReportAdRevenue(new AdRevenue((long)(revenue * 1000000), currency));
            }
            catch {
                Logs.LogError($"[AppMetrica] logAdRevenue INVALID MEDIATION = {mediation}!");
                throw;
            }
        }

        public void LogPurchase(string productId, string currency, double revenue, int quantity = 1) {
            Logs.Log($"[AppMetrica] logPurchaseRevenue, product={productId}, revenue={revenue}{currency}");
            AppMetrica.ReportRevenue(new Revenue((long)revenue, currency));
            Push();
        }

        private void Push() {
            AppMetrica.SendEventsBuffer();
        }
    }
}
#endif