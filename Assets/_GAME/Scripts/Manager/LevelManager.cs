using IPS;
using System.Collections;
using UnityEngine;
public partial class LevelManager : SingletonBehaviour<LevelManager>
{
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    protected partial void InitElements();
    private void OnEnable() {
        this.AddListener<EndGameEvent>(OnEndGame);

        this.AddListener<BikeStartFlyEvent>(OnBikeStartFly);
        Init();
    }
    private void Init() {
        
        Invoke(nameof(PreStart), .05f);
    }
    private void PreStart() {
        InitElements();
        this.Dispatch<PreStartGameEvent>();
    }
    private void OnEndGame() {
        Logs.LogError("EndGame");
    }
    private void OnBikeStartFly() {

    }

    protected override void OnAwake() {
    }
}
