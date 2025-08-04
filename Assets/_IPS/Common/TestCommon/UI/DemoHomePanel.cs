using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS.Test {
    public class DemoHomePanel : Frame {
        
        public void GoIngame() {
            DemoMenuHUD.Instance.Show<DemoIngamePanel>(deactiveCurrent: true);
        }
    }
}