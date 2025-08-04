using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IPS.Logcat {
    public class LogItem : MonoBehaviour {
        [SerializeField] TextMeshProUGUI messageText;
        [SerializeField] Image icon;
        [SerializeField] Sprite errorIcon;
        [SerializeField] Sprite warningIcon;
        [SerializeField] TextMeshProUGUI detailText;

        bool canShowByType = true;
        bool canShowByFillter = true;
        LogType logType;

        private string myMessage;
        public void ShowDetail() {
            detailText.SetText(messageText.text);
        }

        public void SetFillter(string message) {
            canShowByFillter = string.IsNullOrEmpty(message) || myMessage.Contains(message);
            gameObject.SetActive(canShowByFillter && canShowByType);
        }

        public void SetCanShowByLogType(bool errorEnable, bool warningEnable, bool normalEnable) {
            switch(logType) {
                case LogType.Warning: canShowByType = warningEnable; break;
                case LogType.Log: canShowByType = normalEnable; break;
                default: canShowByType = errorEnable; break;
            }

            gameObject.SetActive(canShowByFillter && canShowByType);
        }

        public void Show(string message, LogType type, string stackTrace, Color bg) {
            GetComponent<Image>().color = bg;
            this.logType = type;
            if (type == LogType.Warning) {
                icon.overrideSprite = warningIcon;
                messageText.color = Color.yellow;
            }
            else if (type == LogType.Log) {
                icon.overrideSprite = null;
                messageText.color = Color.white;
            }
            else {
                icon.overrideSprite = errorIcon;
                messageText.color = Color.red;
            }

            messageText.SetText(string.Format("[{0}]: {1}\n\n<color=grey>{2}</color>\n", System.DateTime.Now.ToLongTimeString(), message, stackTrace));
            myMessage = messageText.text.ToLower();
        }

        public void RemoveSeft() {
            this.Recycle();
        }
    }
}