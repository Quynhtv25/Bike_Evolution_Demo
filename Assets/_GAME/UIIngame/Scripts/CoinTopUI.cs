using IPS;
using Mind.CreamOut;
using UnityEngine;
using UnityEngine.UI;

public class CoinTopUI : MonoBehaviour {
    [SerializeField] Button addCoinBtn;
    // Start is called before the first frame update
    void Start() {
        if (!GameData.Instance.IsCoinEnable) gameObject.SetActive(false);

        if (addCoinBtn) addCoinBtn.onClick.AddListener(() => {
            //if (GameManager.Instance.GameState != EGameState.None) UICtrl.Instance.ShowShopIngame();
        });
    }
}
