using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IPS {
    public class InputCtrl {
        /// <summary> Is Mouse over a UI Element? Used for ignoring World clicks through UI </summary>
        public static bool IsPointerOverUI() {
            if (EventSystem.current == null) return false;
            if (EventSystem.current.IsPointerOverGameObject()) {
                return true;
            }
            else {
                if (Input.touchCount > 0) {
                    for (int i = 0; i < Input.touchCount; ++i) {
                        PointerEventData pe = new PointerEventData(EventSystem.current);
                        pe.position = Input.GetTouch(i).position;
                        List<RaycastResult> hits = new List<RaycastResult>();
                        EventSystem.current.RaycastAll(pe, hits);
                        if (hits.Count > 0) return true;
                    }
                    return false;
                }
                else {
                    PointerEventData pe = new PointerEventData(EventSystem.current);
                    pe.position = Input.mousePosition;
                    List<RaycastResult> hits = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pe, hits);
                    return hits.Count > 0;
                }
            }
        }

        public static bool IsTouchBegin(int touchIdx) {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            return Input.touchCount > touchIdx && Input.GetTouch(touchIdx).phase == TouchPhase.Began;
#else
            return Input.GetMouseButtonDown(touchIdx);
#endif
        }

        public static bool IsTouchEnd(int touchIdx) {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            return Input.touchCount > touchIdx && (Input.GetTouch(touchIdx).phase == TouchPhase.Ended || Input.GetTouch(touchIdx).phase == TouchPhase.Canceled);
#else
            return Input.GetMouseButtonUp(touchIdx);
#endif
        }

        public static bool IsDragging(int touchIdx) {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            return Input.touchCount > touchIdx && Input.GetTouch(touchIdx).phase == TouchPhase.Moved;
#else
            return Input.GetMouseButton(touchIdx);
#endif
        }

        public static bool IsHoldingNotDrag(int touchIdx) {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            return Input.touchCount > touchIdx && Input.GetTouch(touchIdx).phase == TouchPhase.Stationary;
#else
            return Input.GetMouseButton(touchIdx) && DeltaTouchPosition(touchIdx) == default;
#endif
        }

        public static Vector2 TouchPosition(int touchIdx) {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            return Input.touchCount > touchIdx ? Input.GetTouch(touchIdx).position : Vector2.zero;
#else
            return Input.mousePosition;
#endif
        }

        public static Vector3 TouchPosition(int touchIdx, float z) {
            Vector3 pos = Vector3.zero;
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            pos = Input.touchCount > touchIdx ? Input.GetTouch(touchIdx).position : Vector3.zero;
            pos.z = z;
#else
            pos = Input.mousePosition;
            pos.z = z;
#endif
            return pos;
        }

        public static Vector2 DeltaTouchPosition(int touchIdx) {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            return Input.touchCount > touchIdx ? Input.GetTouch(touchIdx).deltaPosition : Vector2.zero;
#else
            return Input.GetMouseButton(touchIdx) ? new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) : Vector2.zero;
#endif
        }

        public static bool IsTouchOnLeftScreen(int touchIdx) {
            return Camera.main != null && Camera.main.ScreenToViewportPoint(TouchPosition(touchIdx)).x < .5f;
        }

        public static bool IsTouchOnTopScreen(int touchIdx) {
            return Camera.main != null && Camera.main.ScreenToViewportPoint(TouchPosition(touchIdx)).y > .5f;
        }

    }

}