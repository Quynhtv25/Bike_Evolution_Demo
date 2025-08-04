using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS {

    public class MySortingOrder : MonoBehaviour {
        [SerializeField] int sortingOrder = 0;

        private void OnValidate() {
            UpdateSortingOrder();
        }

        void Start() {
            UpdateSortingOrder();
        }

        [ContextMenu("UpdateSortingOrder")]
        private void UpdateSortingOrder() {
            var mesh = GetComponent<Renderer>();
            if (mesh != null) {
                mesh.sortingOrder = sortingOrder;

                var child = GetComponentsInChildren<Renderer>();
                foreach(var c in child) {
                    if (c != null) {
                        c.sortingOrder = sortingOrder;
                    }
                }
            }
        }
    }
}
