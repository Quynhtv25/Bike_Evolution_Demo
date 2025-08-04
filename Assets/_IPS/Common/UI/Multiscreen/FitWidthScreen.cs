using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IPS {
    [RequireComponent(typeof(RectTransform))]
    public class FitWidthScreen : UIComponent {

        void Start() {
            RectTransform.SetWidth(Screen.width / GetScaleFactor());
        }

        private float GetScaleFactor() {
            float scaleFactor = 1.0f;
            var canvasScaler = GetComponentInParent<CanvasScaler>(true);

            if (canvasScaler == null) return scaleFactor;
            return canvasScaler.CaculateScaleFactor();
        }
    }
}