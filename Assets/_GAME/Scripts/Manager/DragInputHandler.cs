using UnityEngine;
using IPS;

public class DragInputHandler : MonoBehaviour {
    [SerializeField] private float forceMultiplier = 10f;

    private Vector2 dragPos;
    private bool isDragging;
    private IInteract currentTarget;

    void Update() {
        if (InputCtrl.IsPointerOverUI()) return;
        dragPos = InputCtrl.TouchPosition(0);
        if (InputCtrl.IsTouchBegin(0)) {
            isDragging = true;
            currentTarget = null;
            Ray ray = Camera.main.ScreenPointToRay(dragPos);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                currentTarget = hit.collider.GetComponent<IInteract>();
            }
            if (currentTarget == null) {
                Logs.LogError("NotFindTargetTouch");
                return;
            };
            this.Dispatch(new TouchInputEvent {
                target = currentTarget
            });
        }

        if (isDragging && InputCtrl.IsDragging(0)) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(dragPos);

            this.Dispatch(new DragInputEvent {
                startPos = pos
            });
        }

        if (isDragging && InputCtrl.IsTouchEnd(0)) {
            Vector2 dragEnd = InputCtrl.TouchPosition(0);
            Vector2 dir = dragPos - dragEnd;
            Vector2 force = dir * forceMultiplier;

            this.Dispatch(new OnEndDragInput {
                startPos = dragPos,
                endPos = dragEnd,
                direction = dir.normalized,
                magnitude = dir.magnitude,
                force = force
            });
            isDragging = false;
            currentTarget = null;
        }
    }
}
