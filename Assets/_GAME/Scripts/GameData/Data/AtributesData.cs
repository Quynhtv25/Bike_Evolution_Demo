using System;
using UnityEngine;
[CreateAssetMenu(fileName = "AtributesData", menuName = "GAME" + "/AtributesData")]
public class AtributesData : ScriptableObject {
    [SerializeField] private AtributesState[] slingshotState;
    [SerializeField] private AtributesState[] bikeState;
    [SerializeField] private AtributesState[] incomeState;

    public AtributesState[] SlingshotState => slingshotState;
    public AtributesState[] BikeState => bikeState;
    public AtributesState[] IncomeState => incomeState;

    public AtributesState GetBikeData(int level) {
        for (int i = 0; i < bikeState.Length; i++) {
            if (bikeState[i].level != level) continue;
            return bikeState[i];
        }
        return new AtributesState();
    }
}
[Serializable]
public struct AtributesState {
    public int level;
    public InfoUpgrade[] InfoUpgrade;
    public InfoUpgrade GetInfoUpgradeData(int step) {
        for (int i = 0; i < InfoUpgrade.Length; i++) {
            if (InfoUpgrade[i].stepUpgrade != step) continue;
            return InfoUpgrade[i];
        }
        return new InfoUpgrade();
    }
}
[Serializable]
public struct InfoUpgrade {
    public int stepUpgrade;
    public float speed;
}
