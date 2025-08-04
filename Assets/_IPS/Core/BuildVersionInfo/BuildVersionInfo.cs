using UnityEngine;

namespace IPS {
    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class BuildVersionInfo : MonoBehaviour {
        [Header("Format sample (namePrefix 'v', codePrefix='_'): v0.0.1_1")]
        [SerializeField] string versionNamePrefix = "v";
        [SerializeField] string versionCodePrefix = "_";
        private void Start() {
            var text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text) text.text = $"{versionNamePrefix}{BootstrapConfig.Instance.VersionName}{versionCodePrefix}{BootstrapConfig.Instance.VersionCode}";
        }
    }
}