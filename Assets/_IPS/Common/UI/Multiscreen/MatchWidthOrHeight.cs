using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;
using UnityEngine.UI;

namespace IPS {
    [RequireComponent(typeof(CanvasScaler))]
    public class MatchWidthOrHeight : MonoBehaviour {

        private const float Size_9x20   = 9f / 20.5f;   //~0.439
        private const float Size_9x16   = 9f / 16;      // ~0.5625
        private const float Size_10x16  = 10f / 16;     //~0.625
        private const float Size_3x4    = 3f / 4;       // ~0.75 // 

        void Awake() {
            var scaler = GetComponent<CanvasScaler>();
            if (scaler != null) {
                if (Screen.safeArea.width / Screen.safeArea.height >= Size_10x16) {
                    scaler.matchWidthOrHeight = 1;
                }
                else scaler.matchWidthOrHeight = 0;
                Logs.Log($"{name} Apply MatchWidthOrHeight={scaler.matchWidthOrHeight} safe area ratio={(Screen.safeArea.width / Screen.safeArea.height)}");
            }
        }

    }
}