using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IPS {
    public partial class GameData
    {
        [SerializeField] private VihiceData vihiceData;

        public VihiceData VihiceData => vihiceData;
    }
}
