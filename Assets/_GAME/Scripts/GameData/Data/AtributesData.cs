using System;
using UnityEngine;
[CreateAssetMenu(fileName = "AtributesData", menuName = "GAME" + "/AtributesData")]
public class AtributesData : ScriptableObject {
    [SerializeField] private AtributesState[] atributesStates;
    [SerializeField] private int defaultValue = 1;


    public AtributesState[] AtributesStates => atributesStates;
    public float GetValue(EAtribute type, int level) {
        if (atributesStates == null) return defaultValue;
        for (int i = 0; i < atributesStates.Length; ++i) {
            var atribu = atributesStates[i];
            if (atribu.Type != type) continue;
            return atribu.GetValue(level);
        }
        return defaultValue;
    }
}
[Serializable]
public struct AtributesState {
    public EAtribute Type;
    public float defaultValue;
    public float stepValue;
    public OverrideValue[] OverrideValues;
    public float GetValue(int level) {
        float value = defaultValue;
        float current = 0;
        float step = stepValue;
        for (int i = OverrideValues.Length - 1; i > 0; i--) {
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
}
[Serializable]
public struct OverrideValue {
    public int level;
    public float value;
    public float overrideStepValue;
}
