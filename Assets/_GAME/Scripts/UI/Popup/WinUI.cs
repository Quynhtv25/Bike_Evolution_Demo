using IPS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinUI : Frame
{
    [SerializeField] TextMeshProUGUI textDistance;

    private void OnEnable() {
    }

    public void SetTextDistance(float distance) {
        textDistance.SetText($"Distance : {distance}");
    }
}
