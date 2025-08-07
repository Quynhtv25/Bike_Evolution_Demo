using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBehaviourDontDestroy<GameManager>
{
    private EGameState gameState;
    public EGameState GameState => gameState;
    protected override void OnAwake() {
        
    }

    private void OnEnable() {
        this.AddListener<StartGameEvent>(OnStartGame);
        this.AddListener<PauseGameEvent>(OnPauseGame);
        this.AddListener<ContinueGameEvent>(OnContinueGame);
        this.AddListener<EndGameEvent>(OnEndGame);
    }
    private void OnEndGame() {
        gameState = EGameState.End;
    }
    private void OnStartGame() {
        gameState = EGameState.Playing;
    }
    private void OnPauseGame() {
        gameState = EGameState.Pause;
    }
    private void OnContinueGame() {
        gameState = EGameState.Playing;
    }

}
