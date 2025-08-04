using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class RestorePurchaseButton : MonoBehaviour {

    private void Awake() {
#if UNITY_IOS
        gameObject.SetActive(true);
#else

        gameObject.SetActive(true);
        GetComponent<UnityEngine.UI.Button>().interactable = false;
#endif

    }

    protected void Start() {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => IAP.Instance.RestorePurchases());
    }
}