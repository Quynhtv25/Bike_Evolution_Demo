using System;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
[CreateAssetMenu(fileName = "AtributesData", menuName = "GAME" + "/AtributesData")]
public class AtributesData : ScriptableObject {
    [SerializeField] private AtributesState[] atributesStates;
    [SerializeField] private int defaultValue = 1;


    public AtributesState[] AtributesStates => atributesStates;
    public float GetValue(EAtribute type, int level) {
        if (TryGetAtribute(type, out var atribute)) return atribute.GetValue(level);
        return defaultValue;
    }
    public float GetCost(EAtribute type, int level) {
        if (TryGetAtribute(type, out var atribute)) return atribute.GetCost(level);
        return defaultValue;
    }
    public bool TryGetAtribute(EAtribute type, out AtributesState atribute) {
        atribute = new AtributesState();
        if (atributesStates == null) return false;
        for (int i = 0; i < atributesStates.Length; ++i) {
            var atribu = atributesStates[i];
            if (atribu.Type != type) continue;
            atribute = atribu;
            return true;
        }
        return false;
    }
}
[Serializable]
public struct AtributesState {
    public EAtribute Type;
    public float defaultValue;
    public float stepValue;

    public float defaultCost;
    public float stepUpdateCost;
    public OverrideValue[] OverrideValues;
    public OvrrideCost[] OvrrideCosts;
    public float GetValue(int level) {
        float value = defaultValue;
        float current = 1;
        float step = stepValue;
        for (int i = OverrideValues.Length - 1; i >= 0; i--) {
            var o = OverrideValues[i];
            if (o.level > level) continue;
            current = o.level;
            value = o.value;
            if (o.overrideStepValue > 0)
                step = o.overrideStepValue;
            break;

        }
        value += (level - current) * step;
        return value;
    }
    public float GetCost(int level) {
        float cost = defaultCost;
        float scale = stepUpdateCost;
        int current = 1;
        for (int i = OvrrideCosts.Length - 1; i >= 0; i--) {
            var o = OvrrideCosts[i];
            if (o.level > level) continue;
            current = o.level;
            cost = o.cost;
            if (o.scaleCost > 1)
                scale = o.scaleCost;
            break;

        }
        return Mathf.CeilToInt(cost * Mathf.Pow(scale, level- current));
    }
}
[Serializable]
public struct OverrideValue {
    public int level;
    public float value;
    public float overrideStepValue;
}
[Serializable]
public struct OvrrideCost {
    public int level;
    public float cost;
    public float scaleCost;
}
