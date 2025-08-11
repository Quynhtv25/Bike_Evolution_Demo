using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElasticEvolution : MonoBehaviour
{
    [SerializeField] private Transform targetLeft;
    public Transform TargetLeft => targetLeft;
    [SerializeField] private Transform targetRight;
    public Transform TargetRight => targetRight;
    [SerializeField] private Transform targetFollow;
    public Transform TargetFollow => targetFollow;
}
