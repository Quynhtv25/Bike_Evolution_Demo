using UnityEngine;
using IPS;

[RequireComponent(typeof(Rigidbody))]
public class PlaneMoving : MonoBehaviour , IInteract {
    private Rigidbody rb;
    private Vector3 startPos;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable() {
        this.AddListener<TouchInputEvent>(OnTouchInput);
        this.AddListener<DragInputEvent>(OnDragInput);
        this.AddListener<OnEndDragInput>(OnEndDrag);
    }

    private void OnTouchInput(TouchInputEvent param) {
        if (ReferenceEquals(param.target, this)) {
            Logs.LogError("Touched this plane via IInteract");
        }
    }
    private void OnDragInput(DragInputEvent param) {
        transform.position = param.startPos;
        Logs.LogError("value_" + param.startPos);
    }

    private void OnEndDrag(OnEndDragInput data) {
    }
}
