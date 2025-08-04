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

        [SerializeField] float minRotateZ = 390f;
        [SerializeField] float maxRotateZ = 180f;

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

        }
    }
}