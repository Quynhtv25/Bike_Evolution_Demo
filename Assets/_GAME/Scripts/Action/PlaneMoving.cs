using UnityEngine;
using IPS;

[RequireComponent(typeof(Rigidbody))]
public class PlaneMoving : MonoBehaviour, IInteract {
    [SerializeField] private Transform targetCenter;
    [SerializeField] private Transform targetCenterBack;
    [SerializeField] private Transform targetCenterLeft;
    [SerializeField] private Transform targetCenterRight;
    [SerializeField] private float maxFlyDistance = 10f;
    [SerializeField] private float forceMultiplier = 10f;

    private Rigidbody rb;
    private Vector3 startPos;

    public Vector3 Tf => transform.position;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable() {
        this.AddListener<TouchInputEvent>(OnTouchInput);
        this.AddListener<DragInputEvent>(OnDragInput);
        this.AddListener<EndDragInput>(OnEndDrag);
        startPos = Tf;
    }

    private void OnTouchInput(TouchInputEvent param) {
        if (ReferenceEquals(param.target, this)) {
            Logs.LogError("Touched this plane via IInteract");
        }
    }

    private void OnDragInput(DragInputEvent param) {
        if (!ReferenceEquals(param.target, this)) return;

        Vector3 clampedPos = param.dragPos;

        if (targetCenterLeft != null && clampedPos.x < targetCenterLeft.position.x) {
            clampedPos.x = targetCenterLeft.position.x;
        }
        if (targetCenterRight != null && clampedPos.x > targetCenterRight.position.x) {
            clampedPos.x = targetCenterRight.position.x;
        }

        if (targetCenter != null && clampedPos.z > targetCenter.position.z) {
            clampedPos.z = targetCenter.position.z;
        }
        if (targetCenterBack != null && clampedPos.z < targetCenterBack.position.z) {
            clampedPos.z = targetCenterBack.position.z;
        }

        transform.position = clampedPos;

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

        Vector3 finalForce = (transform.forward * 20f) + (20f * Vector3.up);
        Logs.LogError("value_" + finalForce);

        float distance = (targetCenterBack.position.z - transform.position.z);
        float percent = Mathf.Clamp(1 - (distance / targetCenterBack.position.z), 0, 1);
        Logs.LogError("Distance_" + distance.ToString());
        Logs.LogError("Percent_" + percent.ToString());
        rb.AddForce(finalForce, ForceMode.Impulse);
    }
}
