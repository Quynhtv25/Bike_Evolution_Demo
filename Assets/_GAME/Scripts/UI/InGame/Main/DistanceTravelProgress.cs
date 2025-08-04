using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MCL.Bike_Evolution {

    public class DistanceTravelProgress: MonoBehaviour {

        [SerializeField] float maxDistanceTravel = 0;
        [SerializeField] float currentDistanceTravel = 0;
        [SerializeField] Image fillAmount;

        [SerializeField] float smoothTime = 0.2f; 
        private float targetFill = 0f;
        private float currentVelocity = 0f;
        private void OnEnable() {
            this.AddListener<PercentDistanceTravel>(OnProgress);
        }

        private void OnProgress(PercentDistanceTravel param) {
            maxDistanceTravel = param.TotalDistanceTravel;
            currentDistanceTravel = param.CurrentDistanceTravel;

            float percent = (maxDistanceTravel > 0f) ? currentDistanceTravel / maxDistanceTravel : 0f;
            targetFill = Mathf.Clamp01(percent);
            OnUpdateVisual();
        }

        private void OnUpdateVisual() {
            float current = fillAmount.fillAmount;
            float smooth = Mathf.SmoothDamp(current, targetFill, ref currentVelocity, smoothTime);
            fillAmount.fillAmount = smooth;
        }

    }
}