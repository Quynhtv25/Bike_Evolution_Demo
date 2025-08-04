using Cinemachine;
using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cameraMove;
    private void OnEnable() {
        this.AddListener<EndDragInput>(EndDrag);
        this.AddListener<DragInputEvent>(DragInput);
    }
    private void DragInput() {
        cameraMove.Priority = 9;
    }
    private void EndDrag() {
        cameraMove.Priority = 11;
    }
}
