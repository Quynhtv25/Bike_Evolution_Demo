using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupHUD: HUD<PopupHUD> {
    
    private float currentDistance = 0;
    protected override void OnAwake() {
    }

    protected override void OnStart() {
    }

    private void OnEnable() {
        this.AddListener<EndGameEvent>(OnEndGame);
        this.AddListener<PercentDistanceTravel>(OnDistance);
    }

    private void OnDistance(PercentDistanceTravel param) {
        currentDistance = param.CurrentDistanceTravel;
    }

    private void OnEndGame(EndGameEvent param) {
        var win = Get<WinUI>();
        win.SetTextDistance(currentDistance);
        Show<WinUI>();
    }

    public void ShowUpgradePopup() {
        Show<UpgradePopup>();
    }

    public void HideUpgradePopup() {
        Hide<UpgradePopup>();
    }
}
