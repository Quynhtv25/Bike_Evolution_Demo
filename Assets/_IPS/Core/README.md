# IPS `Core` Package
> Last Update: `15/11/2024`
------------------------
# CHANGE LOGS
25/04: 
- Update Excutor wait for realtime instead of game timeScale.
- Update Editor BuildScripts, more menu item for fast build from local 

------------------------
`DO NOT edit any file of this package`
------------------------
# CONTENTS:
- [`Bootstrap`](##bootstrap)
- [`IPSConfig`](##ipsconfig)
- [`Logs`](##logs)
- [`Singletons`](##singletons)
- [`UserData`](##userdata)
- [`Yielder`](##yielder)
- [`Excutor`](##excutor)
- [`Networker`](##networker)
- [`Observer`](##observer)
- [`Pooling`](##pooling)
----------------------------

# CODE IMPLEMENTATION
-----------------------
## Bootstrap
`Add this script into the first loading scene`

----------------------
## IPSConfig

- `CheatEnable`: return true for the editor and the build which turn off `production` from Jenskin. Use this to check your custom cheat logic such as: add currency, unlock all content...
- `LogEnable`: return true for the editor and the build which turn on `development` from Jenskin. Use to control all log of `Logs.Log`

----------------------

## Logs
Use this class instead of `UnityEngine.Debug.Log`, usefull to remove all log for production version.
- `Log(string mesage)`: If *IsEnable* = false, no log message was throw.
- `LogError(string mesage)`: Always throw log error message.
-----------------------

## Singletons
- `Service`: A normal class which is not MonoBehavior. 
- `SingletonBehaviour`: "Instance" = Instantiate new Gameobject if it does not exist in the scene
- `SingletonBehaviourDontDestroy`: Same as `SingletonBehaviour` but make it don't destroy on load scene.
- `SingletonBehaviourResources`: "Instance" = Instantiate from Resources folder when be called at runtime.Place your prefab in Resources: "T/T", T is the name of class.
- `SingletonBehaviourResourcesDontDestroy`: Same as `SingletonBehaviourResources` but make it don't destroy on load scene.
- `SingletonScriptable`: An easily of create Instance of ScriptableObject.
- `SingletonResourcesScriptable`: Place your prefab in Resources folder: "T/T", T is the name of class.
-----------------------

## UserData
The helper to save all user data in game. Use this instead of UnityEngine.PlayerPref.
**`NOTE`**: You can create a new script with `partial` class of `UserData` to write your all new data need to save.
-----------------------

## Yielder
The helper class to get a yield waiting in IEnumerator and Corountine, use this for GC optimization.
`Example`: 
    use 

        yield return Yielder.Wait(0.5f);

instead of
    
        yield return new WaitForSeconds(0.5f);

-----------------------

## Excutor
The helper class to excute a function or ienumerator (corountine) from any where into Unity mainthread. 
Usefull for call back by Ads, Firebase, Facebook... or other third party.

- `Schedule(task, delayTime)`: Run a action from anywhere to mainthread, can be delay with delayTime, 
- `Schedule(enumerator)`: Run a coroutine from anywhere, event the caller is MonoBehaviour or not.
-----------------------

## Networker
A helper to check internet easily
- `IsInternetAvaiable`: Need to be call from mainthread (method of MonoBehaviour such as Awake, Start, Update, LateUpdate, FixedUpdate, OnDestroy, ...). If use this from other thirdparty (firebase, ads, facebook...): need to wrap into `Excutor.Schedule` 

-----------------------

## Observer
Main class `EventDispatcher`.
When you want to dispatch a message from anywhere to other listener in your system, use this patern to reduce instance and reduce reference together.

- `EventDispatcher.Instance.AddListener`: Add a listener callback
- `EventDispatcher.Instance.RemoveListener`: If listener had added before, then need to remove when unuse.
- `EventDispatcher.Instance.Dispatch`: raise an event at anywhere for all listener.

**The other way to use more easily: `using IPS` namespace then call from a MonoBehaivour:**
- `this.AddListener()` (no need to call RemoveListener any more).
- `this.Dispatch()`

`Example`:

    private struct PlayerParam : IEventParam {
        public float damage;
        public float health;
        public float fireRate;
    }
        
    public class PlayerListener : MonoBehaviour {
        private void Awake() {
            this.AddListener(OnPlayerChanged);
        }
        
        private void OnPlayerChanged(Param param) {
            Logs.Log($"OnPlayerChanged damage={param.damage}, health={param.health}, fireRate={param.fireRate}");
        }
    }
    
    public class GameController : MonoBehaviour {
        private void StartGame() {
            this.Dispatch(new PlayerParam() {damage = 1, health = 10, fireRate = 1});
        }
    }
    
-----------------------

## Pooling
Main class **`PoolManager`**

When your game has a lots of same object will be spawn and destroy in runtime, use this patern to recycle thems.

- `IPoolable`: Mark the game object is poolable, it mean the gameobject will be add to pooled when recycle, else it will be destroy.
- `gameObject.RegisterPool()`: Call this to register gameobject with PoolManager, so the game object will be add to pooled when recycle, else it will be destroy. If the gameobject was inhenrit from "IPoolable", you no longer need to call this method.
- `gameObject.Spawn()`: Instantiate (clone) a game object from "prefab" object, If gameObject is  "IPoolable", or was register by "CreatePool" before, output gameObject will be add into pooled list and can be reuse when recycle.
- `gameObject.Recycle()`: Remove game object from the scene (Disable or Destroy). If game object is "IPoolable" (or was register by "RegisterPool"), it will be set to **DISABLE**, else it will be **DESTROY**.

-----------------------

