using IPS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour {
    [SerializeField] private Button mainBtn;
    [SerializeField] private EAtribute atributeType;
    [SerializeField] private UpgradeButtonVisual buttonVisual;

    void Start() {
        mainBtn.onClick.AddListener(OnClickUpgrade);
    }
    private void OnEnable() {
        this.AddListener<UpdateAtributeEvt>(OnUpdateAtribute);
        buttonVisual.Init(atributeType);
        ShowCurrentLevel();
    }
    private void OnClickUpgrade() {
        var currentCoin = UserData.CurrentCoin;
        var currentCost = GameData.Instance.AtributesData.GetCost(atributeType, UserData.GetLevelAtribute((byte)atributeType));
        if (currentCoin < currentCost) return;
        UserData.SetCoin(currentCoin - currentCost);
        UserData.IncreaseAtribute(atributeType);
    }
    private void OnUpdateAtribute(UpdateAtributeEvt param) {
        if (param.type != atributeType) return;
        ShowCurrentLevel();

    }
    private void ShowCurrentLevel() {
        var currentLevel = UserData.GetLevelAtribute((byte)atributeType);
        buttonVisual.ShowCurrentLevel(currentLevel);
    }
}
