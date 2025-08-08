using IPS;
using UnityEngine;
public class LevelManager : SingletonBehaviour<LevelManager>
{
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    private void OnEnable() {
        this.AddListener<EndGameEvent>(OnEndGame);

        this.AddListener<BikeStartFlyEvent>(OnBikeStartFly);
    }
    private void OnEndGame() {
        Logs.LogError("EndGame");
    }
    private void OnBikeStartFly() {

    }

    protected override void OnAwake() {
    }
}
