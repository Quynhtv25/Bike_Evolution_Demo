using DG.Tweening;
using IPS;
using UnityEngine;
using static ElasticVisual;

public enum EBikeRig {
    None = 0,
    HandLeft = 1,
    HandRight = 2,
    FootLeft = 3,
    FootRight = 4,
    Aim = 5,
}
[System.Serializable]
public class BikeRig{
    public EBikeRig eBike;
    public GameObject part;
}
public class BikeController : MonoBehaviour {
    private Rigidbody rb;
    private BikeGraphicController graphic;
    [SerializeField] private RigPointController rigPointController;
    [SerializeField] private WheelCollider frontWheel;
    [SerializeField] private WheelCollider rearWheel;
    [SerializeField] private AnimationCurve curveAngle;
    public float acceleration = 400f;
    public float breakForce = 500f;
    public float maxTurnAngle = 40f;

    [Range(-90, 90)] public float angleLeftRight;

    private float currentAcceleration = 0f;
    private float currentBreakForce = 0f;
    private float currentTurnAngle = 0f;

    //private float verticalInput;
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
       // graphic.CtrlTurnSteer(currentSteerAngle);
        Balance();
        graphic.UpdateFontWheel(frontWheel);
        graphic.UpdateBackWheel(rearWheel);
        graphic.RotatePedals(IsGround, speed, rearWheel);

    }
    private float lastInput = 0f;

    private void Movement() {
        //frontWheel.steerAngle = 0;
        frontWheel.rotationSpeed = 0;
        frontWheel.motorTorque = 0f;
        rearWheel.motorTorque = 0f;
        frontWheel.brakeTorque = 0f;
        rearWheel.brakeTorque = 0f;
        //verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        //bool isChangingDirection = Mathf.Sign(verticalInput) != Mathf.Sign(lastInput) && Mathf.Abs(verticalInput) > 0.1f && Mathf.Abs(lastInput) > 0.1f;

        //if (isChangingDirection) {
        //    frontWheel.brakeTorque = breakForce * 2f;
        //    rearWheel.brakeTorque = breakForce * 2f;
        //    rearWheel.motorTorque = 0f;
        //}
        //{
        //    //currentAcceleration = acceleration * verticalInput;
        //    rearWheel.motorTorque = currentAcceleration;

        //    if (Input.GetKey(KeyCode.Space)) {
        //        currentBreakForce = breakForce;
        //    }
        //    else {
        //        currentBreakForce = 0;
        //    }

        //    frontWheel.brakeTorque = currentBreakForce;
        //    rearWheel.brakeTorque = currentBreakForce;
        //}
        currentTurnAngle = maxTurnAngle * horizontalInput;
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, currentTurnAngle, Time.deltaTime * steerSmoothSpeed);
        //frontWheel.steerAngle = currentSteerAngle;

        //lastInput = verticalInput;
    }
    private bool isGroundedFront;
    private bool isGroundedRear;
    public bool IsGround => isGroundedFront || isGroundedRear;

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
        WheelHit hit;
        isGroundedFront = frontWheel.GetGroundHit(out hit);
        isGroundedRear = rearWheel.GetGroundHit(out hit);
        return;
        angleLeftRight = curveAngle.Evaluate(speed) * horizontalInput;
        float angleX = transform.eulerAngles.x;
        Vector3 directLeftRight = Quaternion.AngleAxis(-angleLeftRight, transform.forward) * Vector3.up;
        Vector3 dirForwBack = Quaternion.AngleAxis(angleX, transform.right) * directLeftRight;
        Vector3 axisFromRotate = Vector3.Cross(transform.up, dirForwBack);
        Vector3 torqueForce = axisFromRotate.normalized * axisFromRotate.magnitude * 50;
        torqueForce.x = torqueForce.x * forceBalanceX;
        torqueForce.y = torqueForce.y * forceBalanceY;
        torqueForce.z = torqueForce.z * forceBalanceZ;
        torqueForce -= rb.angularVelocity;
        rb.AddTorque(torqueForce * rb.mass * forceBalance, ForceMode.Impulse);
        Debug.DrawLine(transform.position, transform.position + directLeftRight * 100, Color.green);
    }

    public void UpdateSteer(Vector3 forward, Vector3 dir) {
        Vector3 localDir = transform.InverseTransformDirection(dir);
        float steerAngle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
        graphic.CtrlTurnSteer(steerAngle);
        //bikeHolder.transform.localEulerAngles = Vector3.Lerp(bikeHolder.transform.localEulerAngles, new Vector3(0, 0, steerAngle), .1f);
    }


    [SerializeField] private Transform leftPedal;
    [SerializeField] private Transform rightPedal;
    private float pedalAngle = 0f;
    private float currentPedalSpeed = 0f;

    [SerializeField] private float pedalAccelerate = 300f;
    [SerializeField] private float pedalDecelerate = 200f;
    [SerializeField] private float maxPedalSpeed = 720f;

    [SerializeField] private float groundedDelay = 0.5f;

    private void OnEnable() {
        this.AddListener<UpdateAtributeEvt>(OnUpdateAtribute);
        CheckShowVisual();
    }


    [SerializeField] private Transform visualHolder;
    private EAtribute type = EAtribute.Bike;
     private int levelVisual;
    private void OnUpdateAtribute(UpdateAtributeEvt param) {
        if (param.type != type) return;
        CheckShowVisual(false);

    }

    [SerializeField] private Transform bikeHolder;
    private void CheckShowVisual(bool isFirst = true) {
        var level = UserData.GetLevelAtribute((byte)type);
        if (!GameData.Instance.EvolutionData.TryGetEvolution(type, level, out var evoGraphic)) return;
        if (levelVisual == evoGraphic.level) return;
        levelVisual = evoGraphic.level;
        if (graphic != null) Destroy(graphic.gameObject);
        graphic = Instantiate(evoGraphic.graphicEvolution, visualHolder).GetComponent<BikeGraphicController>();
        if (graphic == null) return;
        graphic.transform.localPosition = Vector3.zero;
        graphic.transform.localEulerAngles = Vector3.zero;
        graphic.transform.localScale = Vector3.one;
        rigPointController.OnChangeRigPoint(graphic.BikeRigs);

        if (isFirst) return;
        bikeHolder.DOPunchScale(new Vector3(.1f, .1f, .1f), .3f, 0, 1);
        //show vfx evoluction;
    }
}
