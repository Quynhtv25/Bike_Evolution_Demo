using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS {
    public class Debuger : MonoBehaviour {

        protected void Start() {
#if UNITY_EDITOR
            OnStart();
            return;
#endif
            if (!IPSConfig.CheatEnable) DestroyImmediate(gameObject);
            else OnStart();
        }

        protected virtual void OnStart() {}
    }
}