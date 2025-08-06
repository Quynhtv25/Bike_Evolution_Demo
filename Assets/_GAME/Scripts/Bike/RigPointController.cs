using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RigPointController : MonoBehaviour
{
    [System.Serializable]
    public class RigPoint {
        public bool isLockRotation;
        public Vector3 offset;
        public Transform point;
        public Transform target;
        public void Start() {
            point.position = target.position+ offset;
            point.rotation = target.rotation;
        }
        public void Update() {
            point.position = target.position+ offset;
            if (isLockRotation) return;
            point.rotation = target.rotation;
        }
    }
    [SerializeField] private RigPoint[] rigPoints;
    private void Start() {
        if (rigPoints == null) return;
        for(int i=0;i< rigPoints.Length;i++) {
            rigPoints[i].Start();
        }
    }
    void Update()
    {
        if (rigPoints == null) return;
        for(int i=0;i< rigPoints.Length;i++) {
            rigPoints[i].Update();
        }
    }
}
