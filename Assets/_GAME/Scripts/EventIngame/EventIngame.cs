using IPS;
using UnityEngine;

public struct TouchInputEvent : IEventParam {
    public IInteract target;
}
public struct DragInputEvent : IEventParam { public Vector3 dragPos; public IInteract target; }
public struct EndDragInput : IEventParam {
    public IInteract target;
    public Vector3 startPos;
    public Vector3 endPos;
    public Vector3 direction;
    public float magnitude;
    public Vector3 force;
}
