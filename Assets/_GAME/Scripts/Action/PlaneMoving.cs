using UnityEngine;
using IPS;
using DG.Tweening;
using UnityEngine.Rendering;
using MCL.Bike_Evolution;

[RequireComponent(typeof(Rigidbody))]
public class PlaneMoving : MonoBehaviour, IInteract {
    [SerializeField] private Transform targetCenter;
    [SerializeField] private Transform targetCenterBack;
    [SerializeField] private Transform targetCenterLeft;
    [SerializeField] private Transform targetCenterRight;
    private Rigidbody rb;
    private Vector3 startPos;
    private Vector3 endPos = Vector3.forward * 500f;
    private float totalDistance = 0f;

    private bool isFly;
    public Vector3 Tf => transform.position;



    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable() {
        this.AddListener<TouchInputEvent>(OnTouchInput);
        this.AddListener<DragInputEvent>(OnDragInput);
        this.AddListener<EndDragInput>(OnEndDrag);
        startPos = Tf;
        isFly = false;
    }
    private void Update() {
        if (!isFly) return;
        Logs.LogError("On MOve");
        this.Dispatch(new SpeedBikeRuntime() {
            CurrentSpeed = rb.velocity.z,
            MinSpeed = 0,
            MaxSpeed = GameData.Instance.AtributesData.GetBikeData(1).GetInfoUpgradeData(1).speed
        });

        float currentDistance = Vector3.Distance(startPos, Tf);
        this.Dispatch(new PercentDistanceTravel { CurrentDistanceTravel = currentDistance, TotalDistanceTravel = totalDistance });
        //if(Vector3.Distance(rb.velocity, Vector3.zero) < .05f){
        //    isFly = false;

        //}
    }
    private void OnTouchInput(TouchInputEvent param) {
        if (ReferenceEquals(param.target, this)) {
            Logs.LogError("Touched this plane via IInteract");
        }
    }

    private void OnDragInput(DragInputEvent param) {
        if (!ReferenceEquals(param.target, this)) return;

        Vector3 clampedPos = param.dragPos;
        this.Dispatch(new LimitDragEvent {
            startPos = clampedPos,
            target = transform
        });

        if (targetCenter != null) {
            Vector3 lookDirection = (targetCenter.position - transform.position).normalized;
            if (lookDirection.sqrMagnitude > 0.001f) {
                transform.forward = lookDirection;
            }
        }
    }

    private void OnEndDrag(EndDragInput param) {
        if (!ReferenceEquals(param.target, this)) return;
        //if (Vector3.Distance(Tf, param.endPos) <= 5f) {
        //    transform.DOMove(startPos, .5f);
        //    transform.DOLocalRotate(Vector3.zero, .5f);
        //    Logs.LogError("EndDragFail_");

        //    return;
        //}

        Vector3 dirToTarget = (targetCenter.position - transform.position).normalized;


        float distance = (targetCenterBack.position.z - transform.position.z);
        float percent = Mathf.Clamp(1 - (distance / targetCenterBack.position.z), 0, 1);

        float baseSpeed = GameData.Instance.AtributesData.GetBikeData(1).GetInfoUpgradeData(1).speed;
        float finalValue =  baseSpeed * percent;
        Vector3 finalForce = (transform.forward * finalValue) /*+ (finalValue * Vector3.up)*/;

        totalDistance = Vector3.Distance(startPos, endPos);


        isFly = true;
        rb.AddForce(finalForce, ForceMode.Impulse);
    }
}
