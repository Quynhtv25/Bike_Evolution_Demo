using DG.Tweening;
using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElasticVisual : MonoBehaviour
{
    private EAtribute type = EAtribute.SlingShot;
    private int levelVisual;
    private GameObject graphic;
    [SerializeField] private Transform visualHolder;
    private void OnEnable() {
        this.AddListener<UpdateAtributeEvt>(OnUpdateAtribute);
        
    }
    private void OnUpdateAtribute(UpdateAtributeEvt param) {
        if(param.type!= type) return;
        CheckShowVisual(false);
    }
    public void Init() {
        CheckShowVisual();
    }
    private void CheckShowVisual(bool isFirst = true) {
        var level = UserData.GetLevelAtribute((byte)type);
        if (!GameData.Instance.EvolutionData.TryGetEvolution(type, level, out var evoGraphic)) return;
        if (levelVisual == evoGraphic.level) return;
        levelVisual = evoGraphic.level;
        if (graphic != null) Destroy(graphic);
        graphic = Instantiate(evoGraphic.graphicEvolution,visualHolder);
        graphic.transform.localPosition = Vector3.zero;
        graphic.transform.localEulerAngles = Vector3.zero;
        graphic.transform.localScale = Vector3.one;

        if(graphic.TryGetComponent<ElasticEvolution>(out var elastic)) {
            this.Dispatch(new SpawnElasticEvt {elasticEvo = elastic });
            if (isFirst) return;
            VFXEvolutionElement vfxType = GameData.Instance.VFXEvolutionData.GetVFXEvolutionElement(type);
            VFXAllElement vfxLevel = vfxType.GetVFXAtributes(level);
            VFXEvolution vfx = Instantiate(vfxLevel.prefab, LevelManager.Instance.transform);
            Logs.LogError("ye1");
            if (vfx == null) {
                Logs.LogError("no");
                return;
            }
            Logs.LogError("ye2");
            vfx.OnInit();
            // this vfx trigger;
        }
    }
}
