using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IPS.Logcat {
    public class IngameLogcat : SingletonBehaviourResourcesDontDestroy<IngameLogcat> {
        [SerializeField] RectTransform safeRect;
        [SerializeField] GameObject logRegion;
        [SerializeField] GameObject triggerShowButton;
        [SerializeField] Transform logContainer;
        [SerializeField] ScrollRect scrollView;
        [SerializeField] LogItem logItem;
        [SerializeField] TMPro.TMP_InputField inputFillter;
        [SerializeField] Button clearButton;
        [SerializeField] ToggleEffect normalLogButton, errorLogButton, warningLogButton;
        [SerializeField] Color bgColor1 = Color.white;
        [SerializeField] Color bgColor2 = Color.white;

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        //private static void InitIngame() {
        //    IngameLogcat.Instance.Preload();
        //}

        private bool showing;
        private bool errorEnable = true, warningEnable = false, normalEnable = true;
        private List<Action> queue = new List<Action>();
        private List<LogItem> allLogs = new List<LogItem>();

        protected override void OnAwake() {
            Application.logMessageReceivedThreaded += Log;
            Application.lowMemory += OnLowMemmory;
            Application.memoryUsageChanged += OnMemoryChanged;


            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            safeRect.anchorMin = anchorMin;
            safeRect.anchorMax = anchorMax;
            safeRect.pivot = new Vector2(0, 1);

            ForceShowing();
        }

        private void Start() {
            if (!IPSConfig.LogEnable && !IPSConfig.CheatEnable) DestroyImmediate(gameObject);
            else {
                allLogs.Capacity = 1000;
                logItem.RegisterPool(200);
                inputFillter.onSubmit.AddListener(UpdateFillter);
                clearButton.onClick.AddListener(OnClick_ClearAllLog);
                normalLogButton.AddListener(OnClick_FillterNormal);
                errorLogButton.AddListener(OnClick_FillterError);
                warningLogButton.AddListener(OnClick_FillterWarning);
            }
        }

        protected override void OnDestroy() {
            Application.logMessageReceivedThreaded -= Log;
            Application.lowMemory -= OnLowMemmory;
            Application.memoryUsageChanged -= OnMemoryChanged;
            if (PoolManager.Initialized) PoolManager.Instance.RecycleAll(logItem);
            allLogs.Clear();
            queue.Clear();
            GC.Collect();
            base.OnDestroy();
        }

        public void ForceShowing() {
            showing = true;
            triggerShowButton.SetActive(true);
        }

        private void OnClick_ClearAllLog() { 
            foreach(var i in allLogs) {
                i.Recycle();
            }
            allLogs.Clear();
        }

        private void OnClick_FillterError() {
            errorLogButton.SetState(!errorLogButton.State);
            errorEnable = errorLogButton.State;
            foreach(var i in allLogs) { 
                i.SetCanShowByLogType(errorEnable, warningEnable, normalEnable); 
            }
        }

        private void OnClick_FillterWarning() {
            warningLogButton.SetState(!warningLogButton.State);
            warningEnable = warningLogButton.State;
            foreach(var i in allLogs) { 
                i.SetCanShowByLogType(errorEnable, warningEnable, normalEnable); 
            }
        }

        private void OnClick_FillterNormal() {
            normalLogButton.SetState(!normalLogButton.State);
            normalEnable = normalLogButton.State;
            foreach (var i in allLogs) { 
                i.SetCanShowByLogType(errorEnable, warningEnable, normalEnable); 
            }
        }

        private void UpdateFillter(string value) {
            string fillter = inputFillter.text.ToLower();
            foreach(var i in allLogs) {
                i.SetFillter(fillter);
            }
        }

        private void Update() {
            if (queue.Count > 0) {
                var temp = queue.ToArray();
                queue.Clear();
                foreach(var t in temp) {
                    t.Invoke();
                }
            }

            if (showing) return;
            if (Input.touchCount >= 4) {
                showing = true;
                triggerShowButton.SetActive(true);
            }
        }

        private void Log(string message, string stackTrace, LogType type) {
            queue.Add(() => {
                var item = logItem.Spawn(logContainer);
                item.Show(message, type, stackTrace, allLogs.Count % 2 == 0 ? bgColor1 : bgColor2);
                item.SetFillter(inputFillter.text);
                item.SetCanShowByLogType(errorEnable, warningEnable, normalEnable);
                allLogs.Add(item);
                if (scrollView.verticalScrollbar.value < .1f) scrollView.verticalScrollbar.value = 0;
            });
        }

        private void OnLowMemmory() {
            Logs.LogError($"LowMemory Warning!");
        }

        ApplicationMemoryUsage currentMemory;
        private void OnMemoryChanged(in ApplicationMemoryUsageChange usage) {
            if (usage.memoryUsage != currentMemory) {
                currentMemory = usage.memoryUsage;
                Logs.Log($"OnMemoryChanged: {usage.memoryUsage}");
            }
        }
    }
}