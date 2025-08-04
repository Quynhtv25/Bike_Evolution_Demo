
using System;
using IPS.Api.IAP;
using UnityEngine;


#if IAP
using UnityEngine.Purchasing;
#endif

public partial class IAP : BaseIap<IAP> {
    public static string RemoveAdsKey = "remove.ads";
    public static string StarterPack = "starter.pack";

    public bool IsOwnedStarterPack => IsOwned(StarterPack);
    public bool IsRemovedAds => IsOwned(RemoveAdsKey);

    public event Action onInitialized;
#if IAP
    /// <summary> If all product was config at IAP Catalog, then no need to do anythings here. </summary>
    protected override void AddProduct(ConfigurationBuilder builder) {
#if UNITY_IOS
        RemoveAdsKey = $"{Application.identifier}.{RemoveAdsKey}";
        StarterPack = $"{Application.identifier}.{StarterPack}";
#endif
    }

    protected override void OnInitSuccessCallback() {
        if (onInitialized != null) onInitialized.Invoke();
    }
#endif
    }
