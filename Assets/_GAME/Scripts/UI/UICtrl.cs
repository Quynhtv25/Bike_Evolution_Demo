using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IPS;
public class UICtrl : HUD<UICtrl>
{
    protected override void OnAwake() {
    }

    protected override void OnStart() {
    }
    private void OnEnable() {
        this.AddListener<StartGameEvent>(OnStartGame);
        Show<PreInGameUI>();
    }
    private void OnStartGame() {
        Hide<PreInGameUI>();

        Show<IngameUI>();
    }
}
