using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS.Test {
    public class DemoResultPanel : Frame {

        public void ShowRankingPopup() {
            DemoMenuHUD.Instance.Show<DemoRankingPopup>();
        }

        public void ShowOfferPopup() {

        }

        
    }
}