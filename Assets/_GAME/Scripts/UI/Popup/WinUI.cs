using IPS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinUI : Frame
{
    [SerializeField] TextMeshProUGUI textDistance;
    [SerializeField] private ButtonEffect bntClaim;
    private void OnEnable() {
        bntClaim.AddListener(OnClickClaim);
    }
    private void OnClickClaim() {
    }
    public void SetTextDistance(float distance) {
        textDistance.SetText($"Km : {Mathf.RoundToInt(distance)}");
    }
}
