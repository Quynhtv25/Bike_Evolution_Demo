using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBike : MonoBehaviour {
    private Vector3 dir;
    private float horizontalInput;
    private float width;
    private float offset;
    public float Width => width*offset;


    private void FixedUpdate() {
        return;
        horizontalInput = Input.GetAxis("Horizontal");
        Vector3 pathDir = (dir - transform.position).normalized;
        offset += horizontalInput * Time.deltaTime;
        offset = Mathf.Clamp(offset, -1, 1);
    }
    public void SetWidth(float value) {
        width = value;
    }
    public void UpdatePoint(Vector3 nextPoint,Vector3 f) {
        dir = nextPoint;
        transform.position = dir;
        transform.forward = f;
    }
}
