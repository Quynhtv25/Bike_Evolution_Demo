using UnityEngine;

public class BikeController : MonoBehaviour {
    private Rigidbody rb;

    [SerializeField] private WheelCollider frontWheel;
    [SerializeField] private WheelCollider rearWheel;

    [SerializeField] private Transform frontTransform;
    [SerializeField] private Transform backTransform;
    [SerializeField] private Transform helperTransform;
    [SerializeField] private Transform SteerObj;
    [SerializeField] private Transform Damper;
    [SerializeField] private Transform backDamper;
    [SerializeField] private AnimationCurve curveAngle;
    public float acceleration = 400f;
    public float breakForce = 500f;
    public float maxTurnAngle = 40f;

    [Range(-90, 90)] public float angleLeftRight;

    private float currentAcceleration = 0f;
    private float currentBreakForce = 0f;
    private float currentTurnAngle = 0f;

    private float verticalInput;
    private float horizontalInput;
    private float speed;
    private void Start() {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate() {
        Movement();
        speed = Mathf.Round((new Vector2(rb.velocity.x, rb.velocity.z).magnitude * 100f) * 0.01f);
        CtrlTurnSteer();
        Balance();
        UpdateWheel(frontWheel, frontTransform);
        UpdateWheel(rearWheel, backTransform);

    }
    private void Movement() {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        currentAcceleration = acceleration * verticalInput;
        rearWheel.motorTorque = currentAcceleration;

        if (Input.GetKey(KeyCode.Space))
            currentBreakForce = breakForce;
        else
            currentBreakForce = 0f;

        frontWheel.brakeTorque = currentBreakForce;
        rearWheel.brakeTorque = currentBreakForce;

        currentTurnAngle = maxTurnAngle * horizontalInput;
        frontWheel.steerAngle = currentTurnAngle;
    }
    private bool isGround;
    public bool IsGround => isGround;
    private void Balance() {
        bool isGroundedFront, isGroundedRear;
        WheelHit hit;
        isGroundedFront = frontWheel.GetGroundHit(out hit);
        isGroundedRear = rearWheel.GetGroundHit(out hit);
        isGround = isGroundedFront && !isGroundedRear;
        if (!IsGround)
            return;

        if (speed < 5f)
            return;

        float rollAngle = Vector3.SignedAngle(Vector3.up, transform.up, transform.forward);

        if (Mathf.Abs(rollAngle) > 35f)
            return;

        float torqueMultiplier = Mathf.Clamp01(speed / 30f);

        angleLeftRight = curveAngle.Evaluate(speed);

        Vector3 desiredUp = Quaternion.AngleAxis(-angleLeftRight, transform.forward) * Vector3.up;

        Vector3 axisFromRotate = Vector3.Cross(transform.up, desiredUp);

        axisFromRotate.x = 0f;
        axisFromRotate.y = 0f;

        Vector3 torqueForce = axisFromRotate.normalized * axisFromRotate.magnitude * 50f * torqueMultiplier;

        if (Mathf.Abs(torqueForce.z) <= 0.1f) torqueForce.z = 0f;

        torqueForce -= rb.angularVelocity;

        rb.AddTorque(torqueForce * rb.mass * 0.02f, ForceMode.Impulse);

        Debug.DrawLine(transform.position, transform.position + desiredUp * 1.2f, Color.green);
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
    private void CtrlTurnSteer() {
        float angleTurn = SteerObj.localEulerAngles.y;
        SteerObj.rotation = Quaternion.AngleAxis(currentTurnAngle - angleTurn, SteerObj.transform.up) * SteerObj.rotation;
        frontTransform.rotation = helperTransform.rotation;
        frontTransform.rotation = Quaternion.FromToRotation(frontTransform.right, SteerObj.transform.right) * frontTransform.transform.rotation;
        frontTransform.position = Damper.position;

        //float currentUp = helperTransform.localPosition.y - Damper.localPosition.y;
        //Damper.position = Damper.position + SteerObj.transform.up * (currentUp - .5f);
    }
}
