using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class SplineSampler : MonoBehaviour {
    [SerializeField] private SplineContainer splineContainer;
    public SplineContainer SplineContainer => splineContainer;
    [SerializeField] private int _splineIndx;
    [SerializeField][Range(0f, 1f)] private float _time;
    [SerializeField] private float _width;
    float3 position;
    float3 forward;
    float3 upVector;

    private void Update() {

    }
    //private void OnDrawGizmos() {
    //    Handles.matrix = transform.localToWorldMatrix;
    //    Handles.SphereHandleCap(0, p1, Quaternion.identity, 1f, EventType.Repaint);
    //    Handles.SphereHandleCap(0, p2, Quaternion.identity, 1f, EventType.Repaint);
    //}

    public void SampleSplineWidth(float t, float width, out Vector3 p1, out Vector3 p2) {
        splineContainer.Evaluate(_splineIndx, t, out position, out forward, out upVector);
        upVector = Vector3.up;
        float3 right = Vector3.Cross(forward, upVector).normalized;
        p1 = position + (right * width);
        p2 = position + (-right * width);
    }
    public void Sample(float t, out Vector3 p, out Vector3 f) {
        splineContainer.Evaluate(_splineIndx, t, out position, out forward, out upVector);
        f = forward;
        p = position;
    }
    public PointPath[] GetPaths(int count) {
        List<PointPath> paths = new List<PointPath>();

        if (count <= 0) return paths.ToArray();

        for (int i = 0; i < count; i++) {
            float t = (float)i / (count - 1);
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
}
[System.Serializable]
public struct PointPath {
    public Vector3 PointForward;
    public Vector3 DirForward;
}
