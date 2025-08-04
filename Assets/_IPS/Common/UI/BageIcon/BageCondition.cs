using UnityEngine;

namespace IPS {
    public abstract class BageCondition : ScriptableObject {
        public abstract bool CanShowBage { get; }
    }
}