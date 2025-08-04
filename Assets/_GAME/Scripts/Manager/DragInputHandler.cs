using UnityEngine;
using IPS;

public class DragInputHandler : MonoBehaviour {
    [SerializeField] private Transform targetCenter;
    [SerializeField] private Transform targetCenterBack;
    [SerializeField] private Transform targetCenterLeft;
    [SerializeField] private Transform targetCenterRight;



    private Vector3 dragStart;
    private Vector3 dragCurrent;
    private bool isDragging;
    private IInteract currentTarget;

    void Update() {
        if (InputCtrl.IsPointerOverUI()) return;

        Vector3 touchPos = InputCtrl.TouchPosition(0);

        if (InputCtrl.IsTouchBegin(0)) {
            isDragging = true;
            currentTarget = null;
            Ray ray = Camera.main.ScreenPointToRay(touchPos);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                currentTarget = hit.collider.GetComponent<IInteract>();
            }

            if (currentTarget == null) {
                Logs.LogError("NotFindTargetTouch");
                return;
            }

            dragStart = GetWorldPointFromScreen(touchPos, currentTarget.Tf, false);
            this.Dispatch(new TouchInputEvent {
                target = currentTarget
            });
        }

        if (isDragging && InputCtrl.IsDragging(0) && currentTarget != null) {
            dragCurrent = GetWorldPointFromScreen(touchPos, currentTarget.Tf,true);
            this.Dispatch(new DragInputEvent {
                dragPos = dragCurrent,
                target = currentTarget 
            });

        }

        if (isDragging && InputCtrl.IsTouchEnd(0) && currentTarget != null) {
            Vector3 dragEnd = GetWorldPointFromScreen(touchPos, currentTarget.Tf,false);

            this.Dispatch(new EndDragInput {
                target = currentTarget,
                endPos = dragEnd
            });
            isDragging = false;
            currentTarget = null;
        }
    }
    private void OnEnable() {
        this.AddListener<LimitDragEvent>(LimitDrag);
    }
    public void LimitDrag(LimitDragEvent param) {
        Untilities.ClampLitmitDrag(param.startPos, param.target, targetCenterLeft, targetCenterRight, targetCenter, targetCenterBack);
    }
    private Vector3 GetWorldPointFromScreen(Vector3 screenPos, Vector3 referenceWorldPos, bool convertY) {
        screenPos.z = Camera.main.WorldToScreenPoint(referenceWorldPos).z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        if (convertY) {
            worldPos.y = 0f;
        }
        return worldPos;
    }
}
