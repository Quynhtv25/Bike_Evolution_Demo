#if IAR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS {
    public partial class Bootstrap {
        [SerializeField] bool preloadInAppReview = true;

        partial void PreloadInAppReview() {
            if (preloadInAppReview) {
                Excutor.Schedule(() => {
                    RatePopup.Instance.Preload();
                }, 5);
            }
        }
    }

}
#endif