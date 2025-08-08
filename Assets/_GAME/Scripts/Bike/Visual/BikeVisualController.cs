using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ERigBiker {
    None,
    HandLeft = 1,
    HandRight = 2,
    FootLeft = 3,
    FootRight = 4,
}
public class BikeVisualController : MonoBehaviour {
    [System.Serializable]
    public struct BodyPart {
        public ERigBiker type;
        public GameObject part;
    }
    [SerializeField] private BodyPart[] parts;
    public GameObject GetPart(ERigBiker type) {
        if (parts == null) return null;
        for (int i = 0; i < parts.Length; i++) {
            if (parts[i].type != type) continue;
            return parts[i].part;
        }
        return null;
    }
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
