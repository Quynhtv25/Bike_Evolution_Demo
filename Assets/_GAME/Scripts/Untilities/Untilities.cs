using UnityEngine;

public static class Untilities 
{
    public static void ClampLitmitDrag(Vector3 startPos,Transform obj, Transform targetCenterLeft, Transform targetCenterRight, Transform targetCenter, Transform targetCenterBack) {
        Vector3 clampedPos = startPos;

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

        obj.position = clampedPos;
    }
}
