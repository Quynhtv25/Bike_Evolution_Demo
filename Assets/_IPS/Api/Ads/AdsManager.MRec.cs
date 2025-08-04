#if ADMOB
using GoogleMobileAds.Api;
#endif
using IPS.Api.Ads;
using System;
using UnityEngine;

public partial class AdsManager {
    public Action onMRecDisplayed;
    private string currentMRecPlacement;
    
    public bool IsShowingMRec {
        get {
#if ADS
#if IS
            if (ISMediation.Initialized && ISMediation.Instance.IsShowingMRec) return true;
#endif
#if MAX
            if (MaxMediation.Initialized && MaxMediation.Instance.IsShowingMRec) return true;
#endif
            return AdmobMediation.Initialized && AdmobMediation.Instance.IsShowingMRec;
#else
        return false;
#endif
        }
    }

    private void LoadMRecMediation(Mediation mediation, string placement) {
#if ADS
#if IS
        if (mediation == Mediation.admob && ISSettings.Instance.UseMRecAd) {
            ISMediation.Instance.ShowMRec(placement);
            return;
        }
#endif
#if MAX
        if (mediation == Mediation.admob && MaxSettings.Instance.UseMRecAd) {
            MaxMediation.Instance.ShowMRec(placement);
            return;
        }
#endif
        if (AdmobSettings.Instance.UseMRecAd ) {
            AdmobMediation.Instance.ShowMRec(placement);
            return;
        }
#endif
    }

    /// <summary>
    /// Follow ScreenAnchor: TopLeft (0,0), BottomRight (Screen.width, Screen.height). 
    /// <para>The top-left corner of the MRec is positioned at the x and y values passed to the constructor, where the origin is the top-left of the screen.</para>
    /// </summary>
    /// <param name="yPadding"> The padding y in pixel from top/bottom screen to the top/bottom of the MRec, follow ScreenAnchor: TopLeft (0,0), BottomRight (Screen.width, Screen.height). Ad anchor also is Top-Left</param>
    /// <param name="adSize">default use MREC = 300x250 (pt in iOS, dp in Android)</param>
    public void ShowMRec(string placement, float yPadding = 0, Vector2Int? size = null) {
#if ADS
        if (IsRemovedAds) return;
        currentMRecPlacement = placement;

        Logs.Log($"Show MREC egilible with yPadding={yPadding}px, size={(size.HasValue ? size : GetMRecSizeEstimate())}px");

#if MAX
        if (MaxSettings.Instance.UseMRecAd) {
            //if (yPadding > 0) {
                MaxMediation.Instance.ShowMRec(placement, yPadding);
            //}
            //else MaxMediation.Instance.ShowMRec(placement);
            return;
        }
#endif

#if IS
        if (ISSettings.Instance.UseMRecAd) {
            ISMediation.Instance.ShowMRec(placement);
            return;
        }
#endif

        if (AdmobSettings.Instance.UseMRecAd) {
            AdmobMediation.Instance.ShowMRec(placement, yPadding, size);
        }

#else
        Logs.Log("[Ads] Turn ON ADS config first!");
#endif
    }

    public void HideMRec() {
        Logs.Log("[Ads] Hide MRec triggered");
#if ADS
        // if (IsRemovedAds) return;

#if MAX
        if (MaxMediation.Initialized) MaxMediation.Instance.HideMRec();
#endif
#if IS
        if (ISMediation.Initialized && ISSettings.Instance.UseMRecAd) ISMediation.Instance.HideMrec();
#endif

        if (AdmobMediation.Initialized && AdmobSettings.Instance.UseMRecAd) AdmobMediation.Instance.HideMRec();

#endif
    }

    public void DestroyMRec() {
        Logs.Log("[Ads] DestroyMRec triggered.");
#if ADS

#if MAX
        if (MaxMediation.Initialized) MaxMediation.Instance.DestroyMRec();
#endif

#if IS
        if (ISMediation.Initialized && ISSettings.Instance.UseMRecAd) ISMediation.Instance.DestroyMRec();
#endif

        if (AdmobMediation.Initialized && AdmobSettings.Instance.UseMRecAd) AdmobMediation.Instance.DestroyMRec();

#endif

    }

    public void SetMRecPadding(uint y) {
        MaxSettings.Instance.MrecPaddingY_Dp = y;
        AdmobSettings.Instance.MrecPaddingY_Dp = y;
    }

    public Vector2 GetMRecSize() {
#if ADS
#if MAX
        if (MaxSettings.Instance.UseMRecAd) return MaxMediation.Instance.MRecSize;
#endif
#if IS
        if (ISSettings.Instance.UseMRecAd) return ISMediation.Instance.MRecSize;
#endif
        if (AdmobSettings.Instance.UseMRecAd) return AdmobMediation.Instance.MRecSize;

#endif
        return default;
    }

    public Vector2 GetMRecSizeEstimate() {
#if ADS
#if MAX
        if (MaxSettings.Instance.UseMRecAd) return MaxMediation.Instance.MRecSizeEstimate;
#endif
#if IS
        if (ISSettings.Instance.UseMRecAd) return ISMediation.Instance.MRecSizeEstimate;
#endif
        if (AdmobSettings.Instance.UseMRecAd) return AdmobMediation.Instance.MRecSizeEstimate;

#endif
        return default;
    }
}