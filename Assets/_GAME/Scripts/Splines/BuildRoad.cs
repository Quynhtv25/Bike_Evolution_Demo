using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildRoad : MonoBehaviour {
    private RoadManager roadManager;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private int _resolution = 30;
    [SerializeField] private float _width = 8;
    List<Vector3> _vertsP1;
    List<Vector3> _vertsP2;

    public void Init(RoadManager roadManager) {
        this.roadManager = roadManager;
        GetVerts();
        BuildMesh();
    }
    private void GetVerts() {
        _vertsP1 = new List<Vector3>();
        _vertsP2 = new List<Vector3>();

        float step = 1f / (float)_resolution;

        for (int i = 0; i < _resolution; i++) {
            float t = step * i;
            roadManager.SampleSplineWidth(t, _width, out Vector3 p1, out Vector3 p2);
            _vertsP1.Add(p1);
            _vertsP2.Add(p2);

        }
    }

    private void BuildMesh() {
        bool loop = roadManager.GetSpline().Closed;
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


        List<Vector3> thickVerts = new List<Vector3>();
        List<int> thickTris = new List<int>();
        List<Vector2> thickUVs = new List<Vector2>();

        m.subMeshCount = 2;
        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m.SetUVs(0, uvs);
        _meshFilter.mesh = m;
    }
}
