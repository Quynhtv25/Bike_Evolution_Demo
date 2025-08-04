using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TestEventHasParam : IEventParam {
    public int random1;
    public float random2;
}

public struct TestEventNonParam: IEventParam {}

public class TestCoreDontDestroy : SingletonBehaviourDontDestroy<TestCoreDontDestroy> {
    List<KeyValuePair<int, System.Action>> repeatCallback = new();

    protected override void OnAwake() {
        IPSConfig.LogEnable = true;
        IPSConfig.CheatEnable = true;
        EventDispatcher.Instance.SetLogEnable(true);
        EventDispatcher.Instance.AddListener<TestEventNonParam>(OnEventNonParamTriggered);
        EventDispatcher.Instance.AddListener<TestEventNonParam>(OnEventNonParamTriggered_2);

        this.AddListener<TestEventHasParam>(OnEventHasParamNoParamTriggerd, false);
        this.AddListener<TestEventHasParam>(OnEventHasParamTriggerd, false);
        this.AddListener<TestEventHasParam>(OnEventHasParamNoParamTriggerd_2, false);

        foreach(var i in repeatCallback) {
            InvokeRepeating(nameof(i.Value), 10, 10);
        }
    }

    private void OnEventNonParamTriggered() {
        Debug.Log($"<color=yellow>OnEventNonParamTriggered</color>");
        EventDispatcher.Instance.RemoveListener<TestEventNonParam>(OnEventNonParamTriggered);
    }

    private void OnEventNonParamTriggered_2() {
        Debug.Log($"<color=yellow>OnEventNonParamTriggered 2</color>");
    }

    private void OnEventHasParamNoParamTriggerd() {
        Debug.Log($"<color=yellow>OnEventHasParamNoParamTriggerd</color>");
    }
    private void OnEventHasParamNoParamTriggerd_2() {
        Debug.Log($"<color=yellow>OnEventHasParamNoParamTriggerd 2</color>");
    }

    private void OnEventHasParamTriggerd(TestEventHasParam param) {
        Debug.Log($"<color=yellow>OnEventHasParamTriggerd : param1={param.random1}, param2={param.random2}</color>");
    }
}
