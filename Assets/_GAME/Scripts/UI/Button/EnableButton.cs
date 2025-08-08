using IPS;
using UnityEngine;
using UnityEngine.UI;
namespace Mind.CreamOut {
    public class EnableButton : MonoBehaviour {
        [SerializeField] private Button bnt;
        private void OnEnable() {
            Invoke(nameof(LoadActive), .01f);
        }
        private void LoadActive() {
            if(bnt != null) {
                bnt.interactable = GameData.Instance.IsCoinEnable;
            }
            gameObject.SetActive(GameData.Instance.IsCoinEnable);
        }
    }
}
