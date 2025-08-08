using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "GAME" + "/GameConfig")]

public class GameConfig : ScriptableObject
{
    [SerializeField] private float baseSpeedBike =100;
    [SerializeField] private float baseSlingShot = 100;
    [SerializeField] protected float baseBikeStrength = 100;
    [SerializeField] protected float baseIncome = 10;


    #region Property
    public float BaseSpeedBike => baseSpeedBike;
    public float BaseSlingShot => baseSlingShot;
    public float BaseBikeStrength => baseBikeStrength;
    #endregion Property
}
