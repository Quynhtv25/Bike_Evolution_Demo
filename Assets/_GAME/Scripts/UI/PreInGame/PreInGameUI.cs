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
        this.Dispatch(new StartGameEvent());
    }
}
