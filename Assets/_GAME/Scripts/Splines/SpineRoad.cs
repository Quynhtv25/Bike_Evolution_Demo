using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class SpineRoad : MonoBehaviour {
    [SerializeField] private int _resolution = 14;
    [SerializeField] private SplineSampler _splineSampler;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private float _width = 5;
    [SerializeField] private float _curveStep = .1f;
    [SerializeField][Range(0f, 1f)] private float _time;
    List<Vector3> _vertsP1;
    List<Vector3> _vertsP2;
    private void Awake() {

    }
    private void OnEnable() {
        Spline.Changed += OnSplineChaned;
        GetVerts();
        BuildMesh();
    }
    private void OnDisable() {
        Spline.Changed -= OnSplineChaned;
    }
    private void Update() {
        GetVerts();
        abc();
    }

    [SerializeField] private GameObject obj;
    private void abc() {
        _splineSampler.Sample(_time, out var point, out var f);
        obj.transform.position = point;
        obj.transform.forward = f;
    }
    private void OnSplineChaned(Spline arg1, int arg2, SplineModification arg3) {
        Rebuild();
    }
    [ContextMenu("ReBuild")]
    public void Rebuild() {
        BuildMesh();
    }
    private void GetVerts() {
        _vertsP1 = new List<Vector3>();
        _vertsP2 = new List<Vector3>();

        float step = 1f / (float)_resolution;

        for (int i = 0; i < _resolution; i++) {
            float t = step * i;
            _splineSampler.SampleSplineWidth(t, _width, out Vector3 p1, out Vector3 p2);
            _vertsP1.Add(p1);
            _vertsP2.Add(p2);

        }
    }

    private void BuildMesh() {
        bool loop = _splineSampler.SplineContainer.Splines[0].Closed;
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        int offset = 0;
        int length = _vertsP2.Count;
        float uvOffset = 0;
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 1; i <= length; i++) {
            Vector3 p1 = _vertsP1[i - 1];
            Vector3 p2 = _vertsP2[i - 1];
            Vector3 p3;
            Vector3 p4;

            if (i == length) {
                if (!loop) continue;
                p3 = _vertsP1[0];
                p4 = _vertsP2[0];
            }
            else {
                p3 = _vertsP1[i];
                p4 = _vertsP2[i];
            }

            offset = 4 * (i - 1);
            int t1 = offset + 0;
            int t2 = offset + 2;
            int t3 = offset + 3;

            int t4 = offset + 3;
            int t5 = offset + 1;
            int t6 = offset + 0;

            verts.AddRange(new List<Vector3>() { p1, p2, p3, p4 });
            tris.AddRange(new List<int>() { t1, t2, t3, t4, t5, t6 });
            float distance = Vector3.Distance(p1, p3) / 4f;
            float uvDistance = uvOffset + distance;
            uvs.AddRange(new List<Vector2> { new Vector2(uvOffset, 0), new Vector2(uvOffset, 1), new Vector2(uvDistance, 0), new Vector2(uvDistance, 1) });
            uvOffset += distance;
        }

        //Intersection intersection = new Intersection();
        //int indx = 0;
        //var spi = _splineSampler.SplineContainer.Splines[0];
        //foreach( BezierKnot k in spi.Knots) {
        //    intersection.junctions.Add(new JunctionInfo(0,indx,spi,k));
        //}

        //int count = 0;
        //List<JunctionEdge> junctionEdges = new List<JunctionEdge>();
        //Vector3 center = new Vector3();

        //foreach (JunctionInfo junction in intersection.GetJunctions()) {
        //    int splineIndx = junction.splineIndx;
        //    float t = junction.knotIndx == 0 ? 0 : 1;
        //    _splineSampler.SampleSplineWidth(t, _width, out Vector3 p1, out Vector3 p2);
        //    if (junction.knotIndx == 0) {
        //        junctionEdges.Add(new JunctionEdge(p1, p2));
        //    }
        //    else {
        //        junctionEdges.Add(new JunctionEdge(p2, p1));
        //    }
        //    center += p1;
        //    center += p2;
        //    count++;
        //}
        //center = center / (junctionEdges.Count * 2);
        ////junctionEdges.Sort((x, y) => SortPoints(center, x.Center, y.Center));

        //List<Vector3> curvePoints = new List<Vector3>();

        //Vector3 mid;
        //Vector3 c;
        //Vector3 b;
        //Vector3 a;
        //BezierCurve curve;

        //for (int i = 1; i <= junctionEdges.Count; i++) {
        //    a = junctionEdges[i - 1].left;
        //    curvePoints.Add(a);
        //    b = (i < junctionEdges.Count) ? junctionEdges[i].right : junctionEdges[0].right;
        //    mid = Vector3.Lerp(a, b, .5f);
        //    Vector3 dir = center - mid;
        //    mid = mid - dir;
        //    c = Vector3.Lerp(mid, center, .3f);
        //    curve = new BezierCurve(a, c, b);

        //    for (float t = 0f; t < 1f; t += _curveStep) {
        //        Vector3 pos = CurveUtility.EvaluatePosition(curve, t);
        //        curvePoints.Add(pos);
        //    }

        //    curvePoints.Add(b);
        //}
        //curvePoints.Reverse();
        //Debug.LogError(curvePoints.Count);
        //int pointsOffset = verts.Count;

        //for (int i = 1; i <= curvePoints.Count; i++) {
        //    verts.Add(center);
        //    verts.Add(curvePoints[i - 1]);
        //    if (i == curvePoints.Count) {
        //        verts.Add(curvePoints[0]);
        //    }
        //    else {
        //        verts.Add(curvePoints[i]);
        //    }
        //    tris.Add(pointsOffset + ((i - 1) * 3) + 0);
        //    tris.Add(pointsOffset + ((i - 1) * 3) + 1);
        //    tris.Add(pointsOffset + ((i - 1) * 3) + 2);
        //}
        List<Vector3> thickVerts = new List<Vector3>();
        List<int> thickTris = new List<int>();
        List<Vector2> thickUVs = new List<Vector2>();

        //GetIntersectionVerts(verts, trisB, uvs);
        m.subMeshCount = 2;
        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        //m.SetTriangles(trisB, 1);
        m.SetUVs(0, uvs);
        _meshFilter.mesh = m;
    }
    int SortPoints(Vector2 origin, Vector2 a, Vector2 b) {
        float angleA = Mathf.Atan2(a.y - origin.y, a.x - origin.x);
        float angleB = Mathf.Atan2(b.y - origin.y, b.x - origin.x);
        Debug.LogError(angleA.CompareTo(angleB));
        return angleA.CompareTo(angleB);
    }

    //private void Curve() {
    //    Intersection intersection = intersections[0];
    //    int count = 0;
    //    List<JunctionEdge> junctionEdges = new List<JunctionEdge>();
    //    Vector3 center = new Vector3();

    //    foreach (JunctionInfo junction in intersection.GetJunctions()) {
    //        int splineIndx = junction.splineIndx;
    //        float t = junction.knotIndx == 0 ? 0 : 1;
    //        _splineSampler.SampleSplineWidth(t, _width, out Vector3 p1, out Vector3 p2);
    //        if (junction.knotIndx == 0) {
    //            junctionEdges.Add(new JunctionEdge(p1, p2));
    //        }
    //        else {
    //            junctionEdges.Add(new JunctionEdge(p2, p1));
    //        }
    //        center += p1;
    //        center += p2;
    //        count++;
    //    }
    //    center = center / (junctionEdges.Count * 2);
    //    junctionEdges.Sort((x, y) => SortPoints(center, x.Center, y.Center));

    //    List<Vector3> curvePoints = new List<Vector3>();

    //    Vector3 mid;
    //    Vector3 c;
    //    Vector3 b;
    //    Vector3 a;
    //    BezierCurve curve;

    //    for (int i = 0; i < junctionEdges.Count; i++) {
    //        a = junctionEdges[i - 1].left;
    //        curvePoints.Add(a);
    //        b = (i < junctionEdges.Count) ? junctionEdges[i].right : junctionEdges[0].right;
    //        mid = Vector3.Lerp(a, b, .5f);
    //        c = Vector3.Lerp(mid, center, .3f);
    //        curve = new BezierCurve(a, c, b);

    //        for (float t = 0f; t < 1f; t += _curveStep) {
    //            Vector3 pos = CurveUtility.EvaluatePosition(curve, t);
    //            curvePoints.Add(pos);
    //        }

    //        curvePoints.Add(b);
    //    }
    //    curvePoints.Reverse();

    //    int pointsOffset = verts
    //}

    private void OnDrawGizmos() {
        Handles.matrix = transform.localToWorldMatrix;
        for (int i = 0; i < _resolution; i++) {
            Handles.SphereHandleCap(0, _vertsP1[i], Quaternion.identity, 1f, EventType.Repaint);
            Handles.SphereHandleCap(0, _vertsP2[i], Quaternion.identity, 1f, EventType.Repaint);
        }

    }
}
[Serializable]
public class JunctionInfo {
    public int splineIndx;
    public int knotIndx;
    public Spline spline;
    public BezierKnot knot;
    public JunctionInfo(int splineIndx, int knotIndx, Spline spline, BezierKnot knot) {
        this.splineIndx = splineIndx;
        this.knot = knot;
        this.spline = spline;
        this.knot = knot;
    }
}
public struct JunctionEdge {
    public Vector3 left;
    public Vector3 right;
    public Vector3 Center => (left + right) / 2;

    public JunctionEdge(Vector3 p1, Vector3 p2) {
        this.left = p1;
        this.right = p2;
    }
}
[Serializable]
public class Intersection {
    public List<JunctionInfo> junctions;
    public List<float> curves;
    public void AddJunction(int splineIndx, int knotIndx, Spline spine, BezierKnot knot) {
        if (junctions == null) {
            junctions = new List<JunctionInfo>();
        }
        junctions.Add(new JunctionInfo(splineIndx, knotIndx, spine, knot));
    }
    internal IEnumerable<JunctionInfo> GetJunctions() {
        return junctions;
    }
    public Intersection() {
        junctions = new List<JunctionInfo>();
    }
}