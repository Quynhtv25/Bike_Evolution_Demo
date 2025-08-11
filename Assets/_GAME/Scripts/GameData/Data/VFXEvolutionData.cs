using UnityEngine;
[CreateAssetMenu(fileName = "VFXEvolutionData", menuName = "GAME" + "/VFXEvolutionData")]
public class VFXEvolutionData : ScriptableObject
{
    [SerializeField] private VFXEvolutionElement[] allVfx;
    public VFXEvolutionElement GetVFXEvolutionElement(EAtribute type) {
        for (int i = 0; i < allVfx.Length; i++) {
            if (allVfx[i].type != type) continue;
            return allVfx[i];
        }
        return new VFXEvolutionElement();
    }
}
[System.Serializable]
public struct VFXEvolutionElement {

    public EAtribute type;
    public VFXAllElement[] allElements; 
    public VFXAllElement GetVFXAtributes(int level) {
        for (int i = 0; i < allElements.Length; i++) {
            if (level != allElements[i].level) continue;
            return allElements[i];
        }
        return new VFXAllElement();
    }
}
[System.Serializable]
public struct VFXAllElement {
    public int level;
    public GameObject prefab;

}