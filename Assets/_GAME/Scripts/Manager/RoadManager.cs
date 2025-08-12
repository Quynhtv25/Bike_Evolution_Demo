using System.Collections;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;
using System.Collections.Generic;

public class RoadManager : MonoBehaviour {
    [SerializeField] private BuildRoad buildRoad;
    public float Width => buildRoad.Width;
    [SerializeField] private SplineContainer _splineContainer;
    private int countPoint = 100;
    public Spline GetSpline(int indx = 0) {
        if (_splineContainer == null) return null;
        if (indx < 0 || indx >= _splineContainer.Splines.Count) return null;
        return _splineContainer.Splines[indx];
    }
    public void Init(int countPoint) {
        this.countPoint = countPoint;
        buildRoad.Init(this);
    }

    public void SampleSplineWidth(float t, float width, out Vector3 p1, out Vector3 p2) {
        _splineContainer.Evaluate(0, t, out var position, out var forward, out var upVector);
        upVector = Vector3.up;
        float3 right = Vector3.Cross(forward, upVector).normalized;
        p1 = position + (right * width);
        p2 = position + (-right * width);
    }
    public void Sample(float t, out Vector3 p, out Vector3 f) {
        _splineContainer.Evaluate(0, t, out var position, out var forward, out var upVector);
        f = forward;
        p = position;
    }
    public PointPath[] GetPaths() {
        List<PointPath> paths = new List<PointPath>();

        if (countPoint <= 0) return paths.ToArray();

        for (int i = 0; i < countPoint; i++) {
            float t = (float)i / (countPoint - 1);
            Vector3 p, f;
            Sample(t, out p, out f);

            PointPath point = new PointPath {
                PointForward = p,
                DirForward = f
            };
            paths.Add(point);
        }

        return paths.ToArray();
    }

    public float GetLengthPath() {
        if (countPoint < 2) return 0f;

        float length = 0f;
        Vector3 prevPos, forward;
        Sample(0f, out prevPos, out forward);

        for (int i = 1; i < countPoint; i++) {
            float t = (float)i / (countPoint - 1);
            Vector3 pos;
            Sample(t, out pos, out forward);
            length += Vector3.Distance(prevPos, pos);
            prevPos = pos;
        }

        return length;
    }
    public float GetCurrentLength(float t) {
        if (countPoint < 2) return 0f;

        float length = 0f;
        Vector3 prevPos, forward;
        Sample(0f, out prevPos, out forward);

        for (int i = 1; i < countPoint; i++) {
            float nt = t * (i / (float)(countPoint - 1));
            Vector3 pos;
            Sample(nt, out pos, out forward);
            length += Vector3.Distance(prevPos, pos);
            prevPos = pos;
        }

        return length;
    }
    public Vector3 GetPointWithSameWidth(Vector3 sourcePoint, float width) {
        SplineUtility.GetNearestPoint(_splineContainer.Splines[0], sourcePoint, out float3 near, out float t);
        float nextTarget = Mathf.Clamp01(t + .01f);
        //Sample(nextTarget, out var p1, out var f);
        //followBike.UpdatePoint(p1,f);
        float x = -width;
        SampleSplineWidth(nextTarget, x, out var p, out var p2);
        return p;
        //var spline = _splineContainer.Spline;

        //float3 localSource = _splineContainer.transform.InverseTransformPoint(sourcePoint);

        //Ray localRay = new Ray(localSource, Vector3.right);

        //float3 pos, tangent, up, scale;
        //spline.Evaluate(sourceT, out pos, out tangent, out up);
        //Debug.LogError(sourceT + ":  " + pos);
        //float width = Vector3.Distance(sourcePoint, pos);
        //Debug.LogError(width);
        //spline.Evaluate(targetT, out float3 targetPos, out float3 targetTangent, out float3 targetUp);
        //quaternion targetRot = quaternion.LookRotationSafe(targetTangent, targetUp);

        //targetUp = Vector3.up;
        //float3 right = Vector3.Cross(targetTangent, targetUp).normalized;
        //Vector3 newPos = (Vector3)(targetPos + right * (width * 0.5f));
        ////Vector3 newPos = (Vector3)targetPos;
        //return _splineContainer.transform.TransformPoint(newPos);
    }
}

