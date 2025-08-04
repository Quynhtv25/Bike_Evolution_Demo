using IPS;
using UnityEngine;

public struct TouchInputEvent : IEventParam {
    public IInteract target;
}
public struct DragInputEvent : IEventParam { public Vector3 dragPos; public IInteract target; }
public struct EndDragInput : IEventParam {
    public IInteract target;
    public Vector3 endPos;
}
public struct LimitDragEvent :IEventParam {
    public Vector3 startPos;
    public Transform target;
}


// --------------- UI Event ----------------------
public struct SpeedBikeRuntime : IEventParam {
    public float CurrentSpeed;
    public float MinSpeed;
    public float MaxSpeed;
}

public struct PercentDistanceTravel : IEventParam {
    public float TotalDistanceTravel;
    public float CurrentDistanceTravel;
}