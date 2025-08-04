
using UnityEngine;
namespace IPS {
    public class TestProgesstion : MonoBehaviour {
        [SerializeField] ProgesstionEndGame progess;

        private void Start() {
            progess.Init();
            Invoke(nameof(Test), 4f);
        }
        private void Test() {
            Debug.Log(progess.MutilpleValue);
        }
    }
}

