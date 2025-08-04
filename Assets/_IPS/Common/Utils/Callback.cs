using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS {
    public class Callback : MonoBehaviour {
        [SerializeField] float delayFirst = 0;
        [SerializeField] bool loop = true;
        [SerializeField] float loopDuration = 5; // seconds
        [SerializeField] UnityEngine.Events.UnityEvent callback;

        void Start() {
            if (loop) {
                InvokeRepeating(nameof(DoSomething), delayFirst, loopDuration);
            }
            else Invoke(nameof(DoSomething), delayFirst);
        }

        void DoSomething() {
            callback?.Invoke();
        }
    }
}