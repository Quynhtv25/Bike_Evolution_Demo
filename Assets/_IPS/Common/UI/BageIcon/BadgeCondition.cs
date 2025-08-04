using System;
using UnityEngine;

namespace IPS {
    public abstract class BadgeCondition : ScriptableObject {
        Action onUpdateStatus;
        public virtual void Init(MonoBehaviour obj, Action updateStatus) { 
            this.onUpdateStatus = updateStatus;
        }
        public virtual void UpdateStatus() {
            if (onUpdateStatus != null) onUpdateStatus.Invoke();
        }
        public abstract bool CanShowBadge { get; }
    }
}