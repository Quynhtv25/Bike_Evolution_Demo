using IPS;
using UnityEngine;
using UnityEngine.UI;
public class PreInGameUI : Frame
{
    [SerializeField] private Button bntTapPlay;
    private void OnEnable() {
        bntTapPlay.onClick.AddListener(OnPlayGame);
    }
    private void OnPlayGame() {
        Logs.LogError("fasf");
        this.Dispatch(new StartGameEvent());
    }
}
