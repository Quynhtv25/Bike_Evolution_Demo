using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MCL.Bike_Evolution {
    public class SpeedometerManager : MonoBehaviour
    {
        [SerializeField] float minSpeed = 0;
        [SerializeField] float maxSpeed = 0;
        [SerializeField] float currentSpeed = 0;

        [SerializeField] float minRotateZ = 30f;
        [SerializeField] float maxRotateZ = -180f;

        [SerializeField] Image speedometer_needle;

        [SerializeField] float needleSmoothTime = 0.1f; 

        private float needleVelocity = 0f;
        private float targetAngle;


        private void OnEnable() {
            this.AddListener<SpeedBikeRuntime>(OnSetSpeedometer);
        }

        private void OnSetSpeedometer(SpeedBikeRuntime param) { 
            minSpeed = param.MinSpeed;
            maxSpeed = param.MaxSpeed;
            currentSpeed = param.CurrentSpeed;
            OnUpdateVisual();
        }

        private void OnUpdateVisual() {
            float t = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);

            targetAngle = Mathf.Lerp(minRotateZ, maxRotateZ, t);

            float currentZ = speedometer_needle.rectTransform.localEulerAngles.z;
            if(currentZ > 180f)
                currentZ -= 360f;

            float smoothAngle = Mathf.SmoothDampAngle(currentZ, targetAngle, ref needleVelocity, needleSmoothTime);

            speedometer_needle.rectTransform.localRotation = Quaternion.Euler(0, 0, smoothAngle);
        }
    }
}