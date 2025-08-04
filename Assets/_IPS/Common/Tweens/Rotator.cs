using UnityEngine;
using System.Collections;

namespace IPS {
    public class Rotator : MonoBehaviour {

        [Header("Rotate axises by degrees per second")]
        public Vector3 speed = Vector3.back;

        public enum SpaceEnum { Local, World };
        public SpaceEnum rotateSpace;

        void Update() {
            if (rotateSpace == SpaceEnum.Local)
                transform.Rotate(speed);
            else if (rotateSpace == SpaceEnum.World)
                transform.Rotate(speed, Space.World);
        }
    }
}