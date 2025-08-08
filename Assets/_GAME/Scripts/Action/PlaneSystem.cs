using UnityEngine;
using IPS;
using MCL.Bike_Evolution;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class PlaneSystem : MonoBehaviour, IInteract {
    [SerializeField] private BoxCollider colliderInteract;
    [SerializeField] private Transform targetCenter;
    [SerializeField] private Transform targetCenterBack;
    [SerializeField] private Transform targetCenterLeft;
    [SerializeField] private Transform targetCenterRight;
    [SerializeField] private float baseScale = 10;
    private Rigidbody rb;
    private Vector3 startPos;
    private Vector3 endPos = Vector3.forward * 500f;
    private float totalDistance = 0f;

    private bool isFly;
    public Vector3 Tf => transform.position;



    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable() {
        this.AddListener<TouchInputEvent>(OnTouchInput);
        this.AddListener<DragInputEvent>(OnDragInput);
        this.AddListener<EndDragInput>(OnEndDrag);
        startPos = Tf;
        isFly = false;
        rb.isKinematic = true;
        SetupCollider(new Vector3(1.6f, 5.5f, 3.75f), new Vector3(0f, 3.5f, 0));
    }
    private void SetupCollider(Vector3 size,Vector3 center) {
        colliderInteract.size = size;
        colliderInteract.center = center;
    }

    private void Update() {
        if (GameManager.Instance.GameState != EGameState.Playing) return;
        if (isDrag) return;
        if (rb.velocity.magnitude >= 2)
            isFly = true;
        if (!isFly) return;
        this.Dispatch(new SpeedBikeRuntime() {
            CurrentSpeed = rb.velocity.z,
            MinSpeed = 0,
            MaxSpeed = GameData.Instance.AtributesData.GetValue(EAtribute.SlingShot, UserData.GetLevelAtribute((byte)EAtribute.SlingShot))
        });
        float currentDistance = Vector3.Distance(startPos, Tf);

        this.Dispatch(new PercentDistanceTravel { CurrentDistanceTravel = currentDistance, TotalDistanceTravel = totalDistance });

        if (Vector3.Distance(rb.velocity, Vector3.zero) < 2) {
            isFly = false;
            this.Dispatch<EndGameEvent>();
        }
    }
    private void OnTouchInput(TouchInputEvent param) {
        if (ReferenceEquals(param.target, this)) {
            Logs.LogError("Touched this plane via IInteract");
        }
    }
    private void FixedUpdate() {
        if (!isDrag) return;
        rb.velocity = Vector3.zero;
        //rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
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
        //rb.constraints =RigidbodyConstraints.FreezeRotationZ;
        float distance = (targetCenterBack.position.z - transform.position.z);
        float percent = Mathf.Clamp(1 - (distance / targetCenterBack.position.z), 0, 1);
        if (percent <= .2f) {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            transform.DOKill();
            transform.DOMove(startPos, .5f);
            transform.DOLocalRotate(Vector3.zero, .5f);
            Logs.LogError("EndDragFail_"+ startPos);
            return;
        }
        Logs.LogError("fasf_"+percent);
        SetupCollider(new Vector3(.3f, 1.5f, 3.75f), new Vector3(0f, 2f, 0));
        Vector3 dirToTarget = (targetCenter.position - transform.position).normalized;




        float baseSpeed = GameData.Instance.AtributesData.GetValue(EAtribute.SlingShot, UserData.GetLevelAtribute((byte)EAtribute.SlingShot));
        float finalValue = baseSpeed * percent * baseScale;
        Vector3 finalForce = (transform.forward * finalValue) /*+ (finalValue * Vector3.up)*/;

        totalDistance = Vector3.Distance(startPos, endPos);


        rb.AddForce(finalForce, ForceMode.Impulse);
        this.Dispatch(new BikeStartFlyEvent());
    }
}
