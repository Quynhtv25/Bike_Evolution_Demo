using UnityEngine;
using IPS;

[RequireComponent(typeof(Rigidbody))]
public class PlaneMoving : MonoBehaviour, IInteract {
    [SerializeField] private Transform targetCenter; 
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

        if (targetCenter != null) {
            Vector3 lookDirection = (targetCenter.position - transform.position).normalized;
            if (lookDirection.sqrMagnitude > 0.001f) {
                transform.forward = lookDirection;
            }
        }

        transform.position = param.dragPos;
    }

    private void OnEndDrag(EndDragInput param) {
        if (!ReferenceEquals(param.target, this)) return;

        Vector3 dirToTarget = (targetCenter.position - transform.position).normalized;

        Vector3 finalForce = transform.forward*100f;

        rb.AddForce(finalForce, ForceMode.Impulse);
    }
}
