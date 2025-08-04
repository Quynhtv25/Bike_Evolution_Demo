using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "VihiceData", menuName = "GAME" + "/VihiceData")]
public class VihiceData : ScriptableObject
{
    [SerializeField] private List<AtributesState> slingshotState;
    [SerializeField] private List<AtributesState> bikeState;
    [SerializeField] private List<AtributesState> incomeState;

    public List<AtributesState> SlingshotState => slingshotState;
    public List<AtributesState> BikeState => bikeState;
    public List<AtributesState> IncomeState => incomeState;
}
[Serializable]
public struct AtributesState {
    public int level;
    public InfoUpgrade[] InfoUpgrade;
}
[Serializable] 
public struct InfoUpgrade {
    public int stepUpgrade;
    public float speed;
}
