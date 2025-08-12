using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LevelManager
{
    [SerializeField] protected RoadManager roadManager;
    public RoadManager RoadManager => roadManager;
    [SerializeField] private PlaneSystem planeSystem;
    [SerializeField] private DragInputHandler dragInput;
    protected partial void InitElements() {
        roadManager.Init(100);
        SpawnBike();
    }

    private void SpawnBike() {
        var bike = Instantiate(planeSystem);
        bike.Init(dragInput.TargetCenter,dragInput.TargetCenterBack);
        this.Dispatch(new SpawnBikeEvt { bike = bike });
    }
}
