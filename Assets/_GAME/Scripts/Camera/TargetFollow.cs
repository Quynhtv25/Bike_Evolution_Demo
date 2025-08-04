using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollow : MonoBehaviour
{
    [SerializeField] private Transform tartget;
    [SerializeField] private bool isFollow;
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

    private void FixedUpdate() {
        if (!isFollow) return;
        if (tartget == null) return;
        Vector3 target = tartget.position; 
        this.transform.position = target;
    }
}
