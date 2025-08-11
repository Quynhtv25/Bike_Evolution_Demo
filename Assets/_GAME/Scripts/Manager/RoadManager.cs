using System.Collections;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;
using System.Collections.Generic;

public class RoadManager : MonoBehaviour {
    [SerializeField] private BuildRoad buildRoad;
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
}

