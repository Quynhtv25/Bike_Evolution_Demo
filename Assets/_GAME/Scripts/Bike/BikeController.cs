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
    private float currentSteerAngle = 0f;
    [SerializeField] private float steerSmoothSpeed = 5f;

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
        RotatePedals();

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
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, currentTurnAngle, Time.deltaTime * steerSmoothSpeed);
        frontWheel.steerAngle = currentSteerAngle;

    }
    private bool isGround;
    public bool IsGround => isGround;

    [SerializeField] private float balanceStrengthZ = 1.5f;
    [SerializeField] private float balanceStrengthX = 0.4f;
    [SerializeField] private float balanceTorqueMultiplier = 50f;
    [SerializeField] private float balanceDamping = 0.02f;
    //private void Balance() {
    //    bool isGroundedFront, isGroundedRear;
    //    WheelHit hit;
    //    isGroundedFront = frontWheel.GetGroundHit(out hit);
    //    isGroundedRear = rearWheel.GetGroundHit(out hit);
    //    isGround = isGroundedFront && !isGroundedRear;

    //    angleLeftRight = curveAngle.Evaluate(speed) * horizontalInput;

    //    float angleX = transform.eulerAngles.x;
    //    Vector3 desiredUp = Quaternion.AngleAxis(angleX, transform.forward) * Vector3.up;

    //    Vector3 axisFromRotate = Vector3.Cross(transform.up, desiredUp);

    //    axisFromRotate.x = 0;
    //    axisFromRotate.z *= balanceStrengthZ;
    //    axisFromRotate.y = 0f;

    //    Vector3 torqueForce = axisFromRotate.normalized * axisFromRotate.magnitude * balanceTorqueMultiplier;

    //    torqueForce -= rb.angularVelocity;

    //    rb.AddTorque(torqueForce * rb.mass * balanceDamping, ForceMode.Impulse);

    //    Debug.DrawLine(transform.position, transform.position + desiredUp * 10f, Color.green);
    //}

    [SerializeField] private float forceBalance = .02f;
    [SerializeField] private float forceBalanceX = .4f;
    [SerializeField] private float forceBalanceY = .4f;
    [SerializeField] private float forceBalanceZ = .4f;
    private void Balance() {
        angleLeftRight = curveAngle.Evaluate(speed) * horizontalInput;
        float angleX = transform.eulerAngles.x;
        Vector3 directLeftRight = Quaternion.AngleAxis(-angleLeftRight, transform.forward) * Vector3.up;
        Vector3 dirForwBack = Quaternion.AngleAxis(angleX, transform.right) * directLeftRight;
        Vector3 axisFromRotate = Vector3.Cross(transform.up,dirForwBack);
        Vector3 torqueForce = axisFromRotate.normalized * axisFromRotate.magnitude * 50;
        torqueForce.x = torqueForce.x * forceBalanceX;
        torqueForce.y = torqueForce.y * forceBalanceY;
        torqueForce.z = torqueForce.z * forceBalanceZ;
        torqueForce -= rb.angularVelocity;
        rb.AddTorque(torqueForce* rb.mass*forceBalance,ForceMode.Impulse);
        Debug.DrawLine(transform.position, transform.position + directLeftRight * 100, Color.green);
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
        SteerObj.rotation = Quaternion.AngleAxis(currentSteerAngle - angleTurn, SteerObj.transform.up) * SteerObj.rotation;
        frontTransform.rotation = helperTransform.rotation;
        frontTransform.rotation = Quaternion.FromToRotation(frontTransform.right, SteerObj.transform.right) * frontTransform.transform.rotation;
        frontTransform.position = Damper.position;

        //float currentUp = helperTransform.localPosition.y - Damper.localPosition.y;
        //Damper.position = Damper.position + SteerObj.transform.up * (currentUp - .5f);
    }
    [SerializeField] private Transform leftPedal;
    [SerializeField] private Transform rightPedal;
    private float pedalAngle = 0f;
    private float currentPedalSpeed = 0f;

    [SerializeField] private float pedalAccelerate = 300f;
    [SerializeField] private float pedalDecelerate = 200f;
    [SerializeField] private float maxPedalSpeed = 720f;

    private void RotatePedals() {
        float input = verticalInput;

        if (Mathf.Abs(input) > 0.01f) {
            float targetSpeed = maxPedalSpeed * Mathf.Sign(input);
            currentPedalSpeed = Mathf.MoveTowards(currentPedalSpeed, targetSpeed, pedalAccelerate * Time.deltaTime);
        }
        else {
            currentPedalSpeed = Mathf.MoveTowards(currentPedalSpeed, 0f, pedalDecelerate * Time.deltaTime);
        }

        pedalAngle += currentPedalSpeed * Time.deltaTime;
        pedalAngle %= 360f;

        leftPedal.localRotation = Quaternion.Euler(0f, 0f, pedalAngle);
        rightPedal.localRotation = Quaternion.Euler(0f, 0f, pedalAngle);
        //rightPedal.localRotation = Quaternion.Euler(0f, 0f, (pedalAngle + 180f) % 360f);
    }



}
