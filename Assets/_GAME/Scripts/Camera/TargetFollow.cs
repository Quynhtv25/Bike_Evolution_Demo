using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollow : MonoBehaviour
{
    [SerializeField] private Transform tartget;
    [SerializeField] private bool isFollow;
    private Vector3 dir;
    private float time;
    //private void OnEnable() {
    //    this.AddListener<EndDragInput>(EndDrag);
    //    this.AddListener<DragInputEvent>(DragInput);
    //}
    //private void DragInput() {
    //    followX = false;
    //    isFollow = false;
    //}
    //private void EndDrag() {

    //    followX = true;
    //    isFollow = true;
    //}
    private void OnEnable() {
        this.AddListener<NextRotateEvt>(OnNextRotate);
        this.AddListener<SpawnBikeEvt>(OnSpawnBike);
    }
    private void OnSpawnBike(SpawnBikeEvt param) {
        tartget = param.bike.transform;
    }
    private void OnNextRotate(NextRotateEvt param) {
        dir = param.dir;
        time = param.time;
    }
    private void FixedUpdate() {
        if (!isFollow) return;
        if (tartget == null) return;
        Vector3 target = tartget.position; 
        this.transform.position = target;

        transform.forward = Vector3.Lerp(transform.forward, dir, time);
    }
}
