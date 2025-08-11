using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ElasticVisual;

public class DayController : MonoBehaviour {
    [SerializeField] private Transform targetPoint;
    [SerializeField] private Transform targetFollow;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private ElasticVisual elasticVisual;

    private Vector3 baseStart;
    private bool isFollow;

    // Bounce
    private bool isBouncing = false;
    private float bounceTime = 0f;
    private float bounceDuration = 1.5f;
    private float bounceFrequency = 6f;
    private float bounceAmplitude = 0.7f;

    private Vector3 velocity = Vector3.zero;

    private void Start() {
        baseStart = targetPoint.position;
    }

    private void OnEnable() {
        this.AddListener<EndDragInput>(EndDrag);
        this.AddListener<DragInputEvent>(DragInput);
        this.AddListener<SpawnElasticEvt>(OnSpawnElastic);
        elasticVisual.Init();
    }

    private void OnSpawnElastic(SpawnElasticEvt param) {
        if (param.elasticEvo == null) return;
        targetPoint = param.elasticEvo.TargetFollow;

    }
    private void DragInput() {
        isFollow = true;
        isBouncing = false;
    }

    private void EndDrag() {
        isFollow = false;
        isBouncing = true;
        bounceTime = 0f;
    }

    private void FixedUpdate() {
        if (isFollow) {
            Vector3 targetPos = targetFollow.position + offset;

            targetPoint.position = Vector3.Lerp(
                targetPoint.position,
                targetPos,
                followSpeed * Time.fixedDeltaTime
            );
        }
        else if (isBouncing) {
            bounceTime += Time.fixedDeltaTime;

            float t = bounceTime / bounceDuration;
            float damping = Mathf.Exp(-3f * t);
            float oscillation = Mathf.Sin(bounceFrequency * t * Mathf.PI * 2);

            Vector3 dir = targetPoint.position - baseStart;
            if (dir == Vector3.zero) dir = Vector3.right;

            Vector3 offset = dir.normalized * oscillation * bounceAmplitude * damping;

            targetPoint.position = Vector3.SmoothDamp(
                targetPoint.position,
                baseStart + offset,
                ref velocity,
                0.1f, // smooth time
                Mathf.Infinity,
                Time.fixedDeltaTime
            );

            if (t >= 1f) {
                isBouncing = false;
                targetPoint.position = baseStart;
            }
        }
        else {
            targetPoint.position = Vector3.SmoothDamp(
                targetPoint.position,
                baseStart,
                ref velocity,
                0.1f,
                Mathf.Infinity,
                Time.fixedDeltaTime
            );
        }
    }
}
