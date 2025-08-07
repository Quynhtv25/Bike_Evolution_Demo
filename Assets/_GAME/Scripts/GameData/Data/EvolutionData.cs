using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
[Serializable]
public struct Evolution {
    public EAtribute Type;

    public int level;
    public string name;
    public string description;

}
[Serializable]
public struct EvolutionStep {
    public int level;

}
