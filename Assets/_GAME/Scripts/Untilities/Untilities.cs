using System;
using System.Numerics;
using UnityEngine;

public static class Untilities {
    public static void ClampLitmitDrag(UnityEngine.Vector3 startPos, Transform obj, Transform targetCenterLeft, Transform targetCenterRight, Transform targetCenter, Transform targetCenterBack) {
        UnityEngine.Vector3 clampedPos = startPos;

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
    public static ulong PowFloatToUlong(ulong cost, float baseValue, int exponent) {
        double result = 1;
        double b = baseValue;

        while (exponent > 0) {
            if ((exponent & 1) == 1)
                result *= b;

            exponent >>= 1;
            if (exponent > 0)
                b *= b;
        }

        return cost*(ulong)Math.Round(result);
    }
    public static ulong PowFloat(ulong cost,float baseValue, int exponent) {
        float result = 1;
        float b = baseValue;

        while (exponent > 0) {
            if ((exponent & 1) == 1)
                result *= b;

            exponent >>= 1;
            if (exponent > 0)
                b *= b;
        }

        return (ulong)Mathf.RoundToInt(cost*result);
    }

}
