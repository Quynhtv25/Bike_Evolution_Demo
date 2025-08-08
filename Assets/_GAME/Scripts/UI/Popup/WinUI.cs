using IPS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinUI : Frame {
    [SerializeField] TextMeshProUGUI textDistance;
    [SerializeField] private ButtonEffect bntClaim;
    [SerializeField] private TextMeshProUGUI textClaim;
    [SerializeField] Transform tartGetCollect;
    private int claimCoin;
    private void OnEnable() {      
        bntClaim.AddListener(OnClickClaim);
    }

    private void OnClickClaim() {
        ItemCollectEffect.Instance.Collect(textClaim.transform.position, tartGetCollect, () => {
            UserData.SetCoin(UserData.CurrentCoin + (ulong)claimCoin);
        }, () => {
            GameManager.Instance.RequestLoadScene();
        });
    }
    public void SetTextDistance(float distance) {
        textDistance.SetText($"Km : {Mathf.RoundToInt(distance)}");
        claimCoin = Mathf.FloorToInt(distance * GameData.Instance.GameConfig.BaseClaim * GameData.Instance.AtributesData.GetValue(EAtribute.Income, UserData.CurrentLevel));
        textClaim.text = claimCoin.ToString();
    }
}
