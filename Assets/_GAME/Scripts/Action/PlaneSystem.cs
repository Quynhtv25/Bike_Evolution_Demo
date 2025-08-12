using UnityEngine;
using IPS;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.Splines;

[RequireComponent(typeof(Rigidbody))]
public class PlaneSystem : MonoBehaviour, IInteract {
    [SerializeField] private BoxCollider colliderInteract;
    private Transform targetCenter;
    private Transform targetCenterBack;
    private RoadManager roadManager;
    private BikeController bikeController;

    private Rigidbody rb;
    private Vector3 startPos;
    private float totalDistance = 1000f;
    private float width;
    public float Width => width*offset;
     private float offset;
    private float horizontalInput;
    private bool isFly;
    public Vector3 Tf => transform.position;
    private bool isMove;
    private int currentIndexPos;
    private List<Vector3> allPos = new List<Vector3>();
    [SerializeField] float pathFollowStrength = 5f;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    public void Init(Transform center, Transform centerBack) {
        roadManager = LevelManager.Instance.RoadManager;
        bikeController = GetComponent<BikeController>();
        targetCenter = center;
        targetCenterBack = centerBack;
        totalDistance = LevelManager.Instance.RoadManager.GetLengthPath();
    }
    private void OnEnable() {
        this.AddListener<TouchInputEvent>(OnTouchInput);
        this.AddListener<DragInputEvent>(OnDragInput);
        this.AddListener<EndDragInput>(OnEndDrag);
        startPos = Tf;
        isFly = false;
        rb.isKinematic = true;
        SetupCollider(new Vector3(1.6f, 5.5f, 3.75f), new Vector3(0f, 3.5f, 0));
        roadManager = LevelManager.Instance.RoadManager;
        width = roadManager.Width-.5f;
        PointPath[] paths = roadManager.GetPaths();
        for (int i = 0; i < paths.Length; i++) {
            allPos.Add(paths[i].PointForward);
        }
    }
    private void SetupCollider(Vector3 size, Vector3 center) {
        colliderInteract.size = size;
        colliderInteract.center = center;
    }

    private void Update() {
        if (GameManager.Instance.GameState != EGameState.Playing) return;
        if (isDrag) return;
        if (rb.velocity.magnitude >= 2)
            isFly = true;
        if (!isFly) return;
        float km = Mathf.Abs(rb.velocity.z);
        this.Dispatch(new SpeedBikeRuntime() {
            CurrentSpeed = km,
            MinSpeed = 0,
            MaxSpeed = GameData.Instance.AtributesData.GetValue(EAtribute.SlingShot, UserData.GetLevelAtribute((byte)EAtribute.SlingShot))
        });
        float currentDistance = roadManager.GetCurrentLength(currentIndexPos * 1f / allPos.Count);

        this.Dispatch(new PercentDistanceTravel { CurrentDistanceTravel = currentDistance, TotalDistanceTravel = totalDistance });

        if (Vector3.Distance(rb.velocity, Vector3.zero) < 2) {
            isFly = false;
            rb.velocity = Vector3.zero;
            this.Dispatch<EndGameEvent>();
        }
    }
    [SerializeField] private float rotateConfig=1;
    private void OnTouchInput(TouchInputEvent param) {
        if (ReferenceEquals(param.target, this)) {
            Logs.LogError("Touched this plane via IInteract");
        }
    }
    private void FixedUpdate() {
        if (!isDrag && isMove) {
            if (rb.velocity.sqrMagnitude <= 0.01f)
                return;
            int nearestIndex = currentIndexPos;
            float nearestDist = float.MaxValue;
            for (int i = currentIndexPos; i < allPos.Count; i++) {
                float dist = (allPos[i] - transform.position).sqrMagnitude;
                if (dist < nearestDist) {
                    nearestDist = dist;
                    nearestIndex = i;
                }
                if (i > currentIndexPos && Vector3.Dot(allPos[i] - transform.position, rb.velocity) < 0) {
                    break;
                }
            }
            currentIndexPos = nearestIndex;
            float speed = rb.velocity.magnitude;

            if (bikeController.IsGround) {
                horizontalInput = Input.GetAxis("Horizontal");
                offset += horizontalInput * Time.deltaTime;
                offset = Mathf.Clamp(offset, -1, 1);
            }





            Vector3 targetPoint = LevelManager.Instance.RoadManager.GetPointWithSameWidth(transform.position,Width);
            targetPoint.y = rb.position.y;
            Vector3 pathDir = (targetPoint - transform.position).normalized;
            //pathDir.y = rb.velocity.normalized.y;
            float fixTime = pathFollowStrength * Time.fixedDeltaTime;
            rb.velocity = Vector3.Lerp(rb.velocity, pathDir * speed, fixTime);
            this.Dispatch(new NextRotateEvt { dir = pathDir, time = fixTime });
            transform.forward = Vector3.Lerp(transform.forward, pathDir, fixTime * speed / rotateConfig);

            bikeController.UpdateSteer(transform.forward, pathDir);
            //if (!bikeController.IsGround) {
            //    rb.velocity += Vector3.down * 100;
            //}
            return;
        }

        rb.velocity = Vector3.zero;

        this.Dispatch(new LimitDragEvent {
            startPos = clampedPos,
            target = transform
        });

        if (targetCenter != null) {
            Vector3 lookDirection = targetCenter.position - transform.position;
            if (lookDirection.z < 1f) {
                lookDirection.x = 0;
                transform.DOKill();
                transform.DOLocalRotate(Vector3.zero, .5f);
            }
            if (lookDirection.sqrMagnitude > 0.1f) {
                transform.DOKill();
                transform.forward = lookDirection.normalized;
            }
        }
    }
    private Vector3 clampedPos;
    private bool isDrag;
    private void OnDragInput(DragInputEvent param) {
        if (!ReferenceEquals(param.target, this)) return;
        clampedPos = param.dragPos;
        isDrag = true;
        rb.isKinematic = false;

    }

    private void OnEndDrag(EndDragInput param) {
        if (!ReferenceEquals(param.target, this)) return;
        isDrag = false;
        var pStart = this.transform.position;
        offset = -Mathf.Clamp(pStart.x / 2 / width, -1, 1);
        //rb.constraints =RigidbodyConstraints.FreezeRotationZ;
        float distance = (targetCenterBack.position.z - transform.position.z);
        float percent = Mathf.Clamp(1 - (distance / targetCenterBack.position.z), 0, 1);
        if (percent <= .2f) {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            transform.DOKill();
            transform.DOMove(startPos, .5f);
            transform.DOLocalRotate(Vector3.zero, .5f);
            return;
        }
        SetupCollider(new Vector3(.3f, 1.5f, 3.75f), new Vector3(0f, 2f, 0));

        Vector3 dirToTarget = (targetCenter.position - transform.position).normalized;

        float baseSpeed = GameData.Instance.GameConfig.BaseSlingShot * GameData.Instance.AtributesData.GetValue(EAtribute.SlingShot, UserData.GetLevelAtribute((byte)EAtribute.SlingShot));

        float finalValue = baseSpeed * percent;

        Vector3 finalForce = (transform.forward * finalValue) /*+ (finalValue * Vector3.up)*/;
        rb.AddForce(finalForce, ForceMode.Impulse);
        this.Dispatch(new BikeStartFlyEvent());
        isMove = true;
    }
}
