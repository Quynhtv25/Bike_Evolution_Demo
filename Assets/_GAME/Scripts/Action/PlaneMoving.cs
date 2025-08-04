using UnityEngine;
using IPS;
using DG.Tweening;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
public class PlaneMoving : MonoBehaviour, IInteract {
    [SerializeField] private Transform targetCenter;
    [SerializeField] private Transform targetCenterBack;
    [SerializeField] private Transform targetCenterLeft;
    [SerializeField] private Transform targetCenterRight;
    private Rigidbody rb;
    private Vector3 startPos;
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
        this.Dispatch(new SpeedBikeRuntime() {
            CurrentSpeed = rb.velocity.z,
            MinSpeed = 0,
            MaxSpeed = GameData.Instance.AtributesData.GetBikeData(1).GetInfoUpgradeData(1).speed
        });

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
        isFly = true;
        rb.AddForce(finalForce, ForceMode.Impulse);
    }
}
