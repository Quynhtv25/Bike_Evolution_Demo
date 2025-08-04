using IPS;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestCoreTrigger : MonoBehaviour {
    [SerializeField] GameObject prefabNoRegisterPool;
    [SerializeField] GameObject prefabRegisterPool;
    [SerializeField] GameObject prefabIPoolable;
    [SerializeField] TMPro.TextMeshProUGUI textSpawnNoPool;
    [SerializeField] TMPro.TextMeshProUGUI textSpawnHasPool;
    [SerializeField] TMPro.TextMeshProUGUI textSpawnIPoolable;


    private GameObject spawnedNoPool;
    private GameObject spawnedHasPool;
    private GameObject spawnedIPoolable;

    void Start() {
    }

    public void TestAds() {
        SceneManager.LoadScene("TestAds");
        //Transition.Instance.LoadScene("TestAds", false, false);
    }

    public void TestCommon() {
        SceneManager.LoadScene("TestCommon");
        //Transition.Instance.LoadScene("TestCommon", false, true);
    }

    public void SendEventNonParam() {
        this.Dispatch<TestEventNonParam>();
    }
    public void SendEventHasParamNoParam() {
        this.Dispatch<TestEventHasParam>();
    }
    public void SendEventHasParamWithParam() {
        this.Dispatch(new TestEventHasParam() { random1 = Random.Range(1, 5), random2 = Random.Range(6f, 10f)});
    }

    public void SpawnObjNoRegisterPool() {
        if (!spawnedNoPool) {
            textSpawnNoPool.SetText("Recycle obj NO Register Pool");
            spawnedNoPool = prefabNoRegisterPool.Spawn();
        }
        else {
            spawnedNoPool.Recycle();
            spawnedNoPool = null;
            textSpawnNoPool.SetText("Spawn obj NO Register Pool");
        }
    }

    public void SpawnObjWithRegisterPool() {
        if (!spawnedHasPool) {
            textSpawnHasPool.SetText("Recycle obj HAS Register Pool");
            prefabRegisterPool.RegisterPool();
            spawnedHasPool = prefabRegisterPool.Spawn();
        }
        else {
            spawnedHasPool.Recycle();
            spawnedHasPool = null;
            textSpawnHasPool.SetText("Spawn obj HAS Register Pool");
        }
    }
    
    public void SpawnObjIPoolable() {
        if (!spawnedIPoolable) {
            textSpawnIPoolable.SetText("Recycle obj IPoolable");
            spawnedIPoolable = prefabIPoolable.Spawn();
        }
        else {
            spawnedIPoolable.Recycle();
            spawnedIPoolable = null;
            textSpawnIPoolable.SetText("Spawn obj IPoolable");
        }
    }

    public void RecycleAll() {
        PoolManager.Instance.RecycleAll();
    }

    public void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
