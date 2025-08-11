using IPS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonVisual : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI upgradeCost;
    [SerializeField] private TextMeshProUGUI upgradeValue;
    [SerializeField] private Image[] imgStates;
    [SerializeField] private Sprite enableState, disableState;
    private EAtribute atributeType;
    public void Init(EAtribute type) {
        atributeType = type;
    }

    public void ShowCurrentLevel(int currentLevel) {
        levelText.text = currentLevel.ToString();
        if (!GameData.Instance.AtributesData.TryGetAtribute(atributeType, out var atribute)) return;
        upgradeValue.text = atribute.GetValue(currentLevel).ToString();
        var value = atribute.GetCost(currentLevel).ToString();
        if (value.Length < 5) {
            upgradeCost.SetText(value.ToDotCurrency());
        }
        else upgradeCost.SetText(value.ToBigNumber());
        if (imgStates == null || imgStates.Length <= 0) return;
        var current = currentLevel % imgStates.Length;
        if (current == 0)
            current = imgStates.Length;
        for (int i = 0; i < imgStates.Length; i++) {
            if (i < current)
                imgStates[i].sprite = enableState;
            else
                imgStates[i].sprite = disableState;
        }

    }
}
