using System;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
[CreateAssetMenu(fileName = "EvolutionData", menuName = "GAME" + "/EvolutionData")]
public class EvolutionData : ScriptableObject {
    [SerializeField] private Evolution[] evolutions;
    public bool TryGetEvolution(EAtribute type, int level, out EvolutionStep evo) {
        if (evolutions != null) {
            for (int i = 0; i < evolutions.Length; i++) {
                var e = evolutions[i];
                if (e.Type != type) continue;
                if (e.TryGetEvolution(level, out evo)) return true;
                break;

            }
        }
        evo = new EvolutionStep();
        return false;
    }
    [Serializable]
    public struct Evolution {
        public EAtribute Type;
        public EvolutionStep[] evolutionSteps;
        public bool TryGetEvolution(int level, out EvolutionStep evo) {
            if (evolutionSteps != null) {
                for (int i = evolutionSteps.Length-1; i >= 0; i--) {
                    var e = evolutionSteps[i];
                    if (e.level>level) continue;
                    evo = e;
                    return true;
                }
            }
            evo = new EvolutionStep();
            return false;
        }
    }
    [Serializable]
    public struct EvolutionStep {
        public int level;
        public string name;
        public string description;
        public GameObject graphicEvolution;
    }
}
