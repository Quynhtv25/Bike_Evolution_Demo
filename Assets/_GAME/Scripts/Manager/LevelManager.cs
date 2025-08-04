using IPS;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private void OnEnable() {
        this.AddListener<EndGameEvent>(OnEndGame);
    }
    private void OnEndGame() {
        Logs.LogError("EndGame");
    }
}
