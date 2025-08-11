using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElasticVisual : MonoBehaviour
{
    public struct SpawnElasticEvt : IEventParam{ public ElasticEvolution elasticEvo; }
    private EAtribute type = EAtribute.SlingShot;
    private int levelVisual;
    private GameObject graphic;
    [SerializeField] private Transform visualHolder;

    private void OnEnable() {
        this.AddListener<UpdateAtributeEvt>(OnUpdateAtribute);
        
    }
    private void OnUpdateAtribute(UpdateAtributeEvt param) {
        if(param.type!= type) return;
        CheckShowVisual();
    }
    public void Init() {
        CheckShowVisual();
    }
    private void CheckShowVisual() {
        var level = UserData.GetLevelAtribute((byte)type);
        Debug.LogError(000);
        if (!GameData.Instance.EvolutionData.TryGetEvolution(type, level, out var evoGraphic)) return;
        Debug.LogError(111);
        if (levelVisual == evoGraphic.level) return;
        Debug.LogError(222);
        levelVisual = evoGraphic.level;
        if (graphic != null) Destroy(graphic);
        graphic = Instantiate(evoGraphic.graphicEvolution,visualHolder);
        graphic.transform.localPosition = Vector3.zero;
        graphic.transform.localEulerAngles = Vector3.zero;
        graphic.transform.localScale = Vector3.one;

        if(graphic.TryGetComponent<ElasticEvolution>(out var elastic)) {
            this.Dispatch(new SpawnElasticEvt {elasticEvo = elastic });
        }
    }
}
