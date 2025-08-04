
using System;
using System.Collections.Generic;

namespace IPS {
    public struct TutorialStart<T> : IEventParam {
        public T stepObject;
    }
    public struct TutorialStop<T> : IEventParam {
        public T stepObject;
    }

    public class TutorialSystem : Service<TutorialSystem> {
        #region Data
        Dictionary<Type, object> steps;

        protected override void Initialize() {

        }

        public void RunStep<T, E>(T obj) where E : IEventParam {
            Logs.Log($"<color=magenta>[Tutorial] Run step type={obj.GetType().Name}, obj={obj}, event={typeof(E).Name}</color>");

            EventDispatcher.Instance.Dispatch(new TutorialStart<T> { stepObject = obj });
            EventDispatcher.Instance.AddListener<E>((param) => {
                RemoveStep<T, E>();
            });
            steps ??= new();
            if (steps.ContainsKey(typeof(T))) steps[typeof(T)] = obj;
            else steps.Add(typeof(T), obj);
        }

        public void RemoveStep<T, E>() where E : IEventParam {
            if (steps.ContainsKey(typeof(T))) {
                EventDispatcher.Instance.Dispatch(new TutorialStop<T> { stepObject = (T)steps[typeof(T)] });
                steps.Remove(typeof(T));
            }
            EventDispatcher.Instance.RemoveListener<E>();
            Logs.Log($"<color=green>[Tutorial] Completed step type={typeof(T).Name}, event={typeof(E).Name}</color>");
        }

        public bool GetStep<T>(out T result) {
            if (steps != null && steps.ContainsKey(typeof(T))) {
                result = (T)steps[typeof(T)];
                return true;
            }
            else {
                result = default;
                return false;
            }
        }

        public bool WaitingStep<T>(T obj) {
            if (GetStep<T>(out var step)) {
                return !step.Equals(obj);
            }
            return false;
        }
        #endregion
    }
}