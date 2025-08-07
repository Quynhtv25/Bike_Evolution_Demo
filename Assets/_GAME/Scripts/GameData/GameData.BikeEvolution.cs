using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IPS {
    public partial class GameData
    {
        [SerializeField] private AtributesData atributesData;

        public AtributesData AtributesData => atributesData;
        [SerializeField] private GameConfig gameConfig;
        public GameConfig GameConfig => gameConfig;
    }
}
