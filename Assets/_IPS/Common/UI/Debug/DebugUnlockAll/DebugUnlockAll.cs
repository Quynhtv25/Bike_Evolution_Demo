using IPS;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IPS {
    [RequireComponent (typeof(Button))]
    public class DebugUnlockAll : Debuger {

        protected override void OnStart() {
            GetComponent<Button>().onClick.AddListener(OnClickUnlockAll);
        }

        private void OnClickUnlockAll() {
            GameData.Instance.UnlockAll();
            IsUnlockedAll = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private bool IsUnlockedAll {
            get => PlayerPrefs.GetInt("DebugUnlockedALl", 0) == 1;
            set => PlayerPrefs.SetInt("DebugUnlockedALl", 1);
        }
    }
}