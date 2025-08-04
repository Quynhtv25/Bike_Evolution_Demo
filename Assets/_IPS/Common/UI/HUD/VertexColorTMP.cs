using TMPro;
using UnityEngine;

namespace IPS {
    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class VertexColorTMP : MonoBehaviour {
        [SerializeField] bool verticleColor = true;
        [SerializeField] Color color1 = Color.white;
        [SerializeField] Color color2 = Color.white;

        private TextMeshProUGUI text;

        private void OnValidate() {
            UpdateColor();            
        }

        void OnEnable() {
            Invoke(nameof(UpdateColor), .02f);
        }

        [ContextMenu("UpdateColor")]
        private void UpdateColor() {
            if (text == null) text = GetComponent<TMPro.TextMeshProUGUI>();
            // Force text to update so we get access to mesh info
            text.ForceMeshUpdate();

            // Get TMP mesh info
            TMP_TextInfo textInfo = text.textInfo;

            // Loop through each character in the text
            for (int i = 0; i < textInfo.characterCount; i++) {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible) continue;

                // Get the vertex colors for the character
                int vertexIndex = charInfo.vertexIndex;
                Color32[] newVertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;

                // Set all 4 vertices of the character to the same color (no gradient)
                if (verticleColor) {
                    newVertexColors[vertexIndex + 0] = color2;
                    newVertexColors[vertexIndex + 1] = color1;
                    newVertexColors[vertexIndex + 2] = color1;
                    newVertexColors[vertexIndex + 3] = color2;
                }
                else {
                    newVertexColors[vertexIndex + 0] = color1;
                    newVertexColors[vertexIndex + 1] = color1;
                    newVertexColors[vertexIndex + 2] = color2;
                    newVertexColors[vertexIndex + 3] = color2;
                }
            }

            // Update the TMP mesh
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }
}