using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS.Test {
    public class DemoIngamePanel : Frame {
        public void GoResult() {
            DemoMenuHUD.Instance.Show<DemoResultPanel>(deactiveCurrent: true);
        }
    }
}