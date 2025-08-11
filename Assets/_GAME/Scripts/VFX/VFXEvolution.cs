using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXEvolution : MonoBehaviour
{
    [SerializeField] private ParticleSystem vfx;

    public void OnInit() {
        vfx.Play();
    }
}
