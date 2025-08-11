using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeGraphicController : MonoBehaviour {
    [SerializeField] private BikeRig[] bikeRigs;
    public BikeRig[] BikeRigs => bikeRigs;
    [SerializeField] private Transform frontTransform;
    [SerializeField] private Transform backTransform;
    [SerializeField] private Transform helperTransform;
    [SerializeField] private Transform SteerObj;
    [SerializeField] private Transform Damper;


    public void CtrlTurnSteer(float currentSteerAngle) {
        float angleTurn = SteerObj.localEulerAngles.y;
        SteerObj.rotation = Quaternion.AngleAxis(currentSteerAngle - angleTurn, SteerObj.transform.up) * SteerObj.rotation;
        frontTransform.rotation = helperTransform.rotation;
        frontTransform.rotation = Quaternion.FromToRotation(frontTransform.right, SteerObj.transform.right) * frontTransform.transform.rotation;
        frontTransform.position = Damper.position;

        //float currentUp = helperTransform.localPosition.y - Damper.localPosition.y;
        //Damper.position = Damper.position + SteerObj.transform.up * (currentUp - .5f);
    }
    public void UpdateFontWheel(WheelCollider wheel) {
        UpdateWheel(wheel, frontTransform);
    }
    public void UpdateBackWheel(WheelCollider wheel) {
        UpdateWheel(wheel, backTransform);
    }
    private void UpdateWheel(WheelCollider col, Transform wheelTransform) {
        Vector3 position;
        Quaternion rotation;

        col.GetWorldPose(out position, out rotation);
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
        var w = wheelTransform.localEulerAngles;
        var x = w.x;
        w.x = w.y = 0;
        w.z = x;
        wheelTransform.localEulerAngles = w;
    }

    [SerializeField] private Transform leftPedal;
    [SerializeField] private Transform rightPedal;
    private float pedalAngle = 0f;
    private float currentPedalSpeed = 0f;

    [SerializeField] private float pedalAccelerate = 300f;
    [SerializeField] private float pedalDecelerate = 200f;
    [SerializeField] private float maxPedalSpeed = 720f;
    private float timeSinceGrounded;
    [SerializeField] private float groundedDelay = 0.5f;
    public void RotatePedals(bool IsGround, float speed, WheelCollider rearWheel) {
        if (IsGround) {
            timeSinceGrounded = Time.time;

            float rpm = rearWheel.rpm;
            if (rpm <= 0)
                return;
            if (speed <= 3)
                return;
            float pedalSpeed = (rpm * 360f) / 60f;
            currentPedalSpeed = Mathf.Clamp(pedalSpeed, -maxPedalSpeed, maxPedalSpeed);
        }
        else {
            currentPedalSpeed = Mathf.MoveTowards(currentPedalSpeed, 0f, pedalDecelerate * Time.deltaTime);

            if (Time.time - timeSinceGrounded > groundedDelay && Mathf.Abs(currentPedalSpeed) < 1f) {
                return;
            }
        }

        pedalAngle += currentPedalSpeed * Time.deltaTime;
        pedalAngle %= 360f;

        leftPedal.localRotation = Quaternion.Euler(0f, 0f, pedalAngle);
        rightPedal.localRotation = Quaternion.Euler(0f, 0f, pedalAngle);
        //rightPedal.localRotation = Quaternion.Euler(0f, 0f, (pedalAngle + 180f) % 360f); 
    }


}
