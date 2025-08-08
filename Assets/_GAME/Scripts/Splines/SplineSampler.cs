using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;
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
    Vector3 p1;
    Vector3 p2;

    private void Update() {
        SampleSplineWidth(_time,_width, out p1, out p2);
        
    }
    //private void OnDrawGizmos() {
    //    Handles.matrix = transform.localToWorldMatrix;
    //    Handles.SphereHandleCap(0, p1, Quaternion.identity, 1f, EventType.Repaint);
    //    Handles.SphereHandleCap(0, p2, Quaternion.identity, 1f, EventType.Repaint);
    //}

    public void SampleSplineWidth(float t,float width, out Vector3 p1, out Vector3 p2) {
        splineContainer.Evaluate(_splineIndx, t, out position, out forward, out upVector);
        upVector = Vector3.up;
        float3 right = Vector3.Cross(forward, upVector).normalized;
        p1 = position + (right * width);
        p2 = position + (-right * width);
    }
    public void Sample( float t, out Vector3 p, out Vector3 f) {
        splineContainer.Evaluate(_splineIndx, t, out position, out forward, out upVector);
        f = forward;
        p = position;
    }
}
