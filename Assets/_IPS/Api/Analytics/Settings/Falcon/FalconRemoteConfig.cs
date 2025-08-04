//#if FALCON_ANALYTIC
//using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;
//using Falcon.FalconCore.Scripts.Utils.Entities;
//using IPS;
//using IPS.Api.Ads;
//using System.Collections;
//using UnityEngine;

//public class FalconRemoteConfig : FalconConfig {

//    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
//    static void FetchConfig() {
//        FalconConfig.OnUpdateFromNet += ((sender, args) => {
//            var config = FalconConfig.Instance<FalconRemoteConfig>();
//        });
//    }
//}
//#endif