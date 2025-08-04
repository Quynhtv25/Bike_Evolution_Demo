using System;
using UnityEngine;

namespace IPS {
    [CreateAssetMenu(fileName="DefaultFirstSceneCondition", menuName="IPS/DefaultFirstSceneCondition")]
    public class DefaultFirstSceneCondition : FirstSceneCondition {

        public override Action FirstSceneEnterAction => null;
    }

    public abstract class FirstSceneCondition : ScriptableObject {
        [SerializeField] int nextSceneIdx = 1;
        public virtual int NextSceneIdx => nextSceneIdx;

        /// <summary>
        /// This action will be called when the first scene is loaded.
        /// </summary>
        public abstract Action FirstSceneEnterAction { get; }
    }
}