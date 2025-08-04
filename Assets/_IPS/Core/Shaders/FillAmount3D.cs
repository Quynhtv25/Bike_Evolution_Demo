using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS {
    [RequireComponent(typeof(Renderer))]
    public class FillAmount3D : MonoBehaviour {
        [Tooltip("Material must use shader IronPirate/FillAmount3D")] 
        [SerializeField] [Range(0, 1)] float startAmount = 0;

        private Material mat;
        float targetAmount;

        private const string MatProperty = "_FillAmount";

        public float CurrentAmount => mat.GetFloat(MatProperty);

        private void Start() {
            mat = GetComponent<Renderer>().material;
            targetAmount = startAmount;
            DoFillAmount(targetAmount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Range(0, 1)</param>
        public void DoFillAmount(float value, float duration = 0) {
            targetAmount = Mathf.Clamp(value, 0, 1);
            if (duration == 0) {
                mat.SetFloat(MatProperty, targetAmount);
            }
            else if (gameObject.activeInHierarchy) {
                StopAllCoroutines();
                StartCoroutine(IEFillAmount(duration));
            }
        }

        private IEnumerator IEFillAmount(float duration) {
            float elapse = 0;
            var startAmount = CurrentAmount;

            while(elapse < duration) {
                elapse += Time.deltaTime;
                mat.SetFloat(MatProperty, Mathf.Lerp(startAmount, targetAmount, elapse / duration));
                yield return null;
            }

            mat.SetFloat(MatProperty, targetAmount);
        }
    }
}