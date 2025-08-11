using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LevelManager
{
    [SerializeField] protected RoadManager roadManager;
    public RoadManager RoadManager => roadManager;
    protected partial void InitElements() {
        roadManager.Init();
    }
}
