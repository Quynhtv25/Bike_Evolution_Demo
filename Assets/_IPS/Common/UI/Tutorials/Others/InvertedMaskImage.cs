using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InvertedMaskImage : Image {
    Material mat;

    public override Material materialForRendering {
        get {
            //if (mat == null) {
                mat = new Material(base.materialForRendering);
            //}
            mat.SetFloat("_StencilComp", (float)CompareFunction.NotEqual);
            return mat;

        }
    }
}