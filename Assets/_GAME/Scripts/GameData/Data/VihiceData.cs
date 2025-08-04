using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GameData", menuName = "GAME" + "/VihiceData")]
public class VihiceData : ScriptableObject
{
    [SerializeField] private List<AtributesStat> slingshotState;
    [SerializeField] private List<AtributesStat> bikeState;
    [SerializeField] private List<AtributesStat> incomeState;
}
[Serializable]
public struct AtributesStat {
    public int level;
    public int stepUpgrade;
    public float speed;
}
