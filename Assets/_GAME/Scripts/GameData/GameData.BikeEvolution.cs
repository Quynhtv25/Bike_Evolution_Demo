using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IPS {
    public partial class GameData
    {
        [SerializeField] private AtributesData atributesData;
        [SerializeField] private VFXEvolutionData vfxEvolutionData;

        public AtributesData AtributesData => atributesData;
        [SerializeField] private GameConfig gameConfig;
        public GameConfig GameConfig => gameConfig;
        [SerializeField] private bool isCoinEnable;
        public bool IsCoinEnable => isCoinEnable;

        [SerializeField] private EvolutionData evolutionData;
        public EvolutionData EvolutionData => evolutionData;
        public VFXEvolutionData VFXEvolutionData => vfxEvolutionData;
    }
}
