using IPS;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private void OnEnable() {
        this.AddListener<EndGameEvent>(OnEndGame);

        this.AddListener<BikeStartFlyEvent>(OnBikeStartFly);
    }
    private void OnEndGame() {
        Logs.LogError("EndGame");
    }
    private void OnBikeStartFly() {

    }
}
