
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIComponent : MonoBehaviour {
    private RectTransform m_RectTransform;

    private Rect myRect;
    public RectTransform RectTransform { 
        get { 
            if (m_RectTransform != null) return m_RectTransform; 
            m_RectTransform = transform as RectTransform;
            myRect = GetWorldSpace(m_RectTransform);
            return m_RectTransform;
        }
    }

    public void UpdateRect() {
        myRect = GetWorldSpace(RectTransform);
    }

    public void SetWidth(float width) {
        RectTransform.SetWidth(width);
    }

    public void SetHeight(float height) {
        RectTransform.SetHeight(height);
    }
    
    public bool IsCollideWith(RectTransform other, bool updateMyRect = false) {
        if (updateMyRect) {
            this.myRect = GetWorldSpace(RectTransform);
        }
        //Vector2 otherCenterPoint = GetWorldSpace(other).center;
        //return myRect.Contains(otherCenterPoint);

        return myRect.Overlaps(GetWorldSpace(other));
    }

    public Rect GetWorldSpace(RectTransform rectTransform) {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            
            float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
            float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

            //With this it works even if the pivot is not at the center
            Vector3 position =rectTransform.TransformPoint(rectTransform.rect.center);
            float x = position.x - rectTransformWidth * 0.5f;
            float y = position.y - rectTransformHeight * 0.5f;

        return new Rect(x,y, rectTransformWidth, rectTransformHeight);
        //return new Rect(rectTransform.localPosition.x, rectTransform.localPosition.y, rectTransform.rect.width, rectTransform.rect.height);
        //var rect = rectTransform.rect;
        //rect.center = Camera.main.WorldToScreenPoint(rectTransform.TransformPoint(rect.center));
        //rect.size = Camera.main.WorldToScreenPoint(rectTransform.TransformVector(rect.size));
        //return rect;
    }
}

public static class UIExtension {
    public static float CaculateScaleFactor(this CanvasScaler canvasScaler) {
        if (canvasScaler == null) return 1;

        if (canvasScaler.matchWidthOrHeight == 0) {
            return Screen.width / canvasScaler.referenceResolution.x;
        } else if (canvasScaler.matchWidthOrHeight == 1) {
            return Screen.height / canvasScaler.referenceResolution.y;
        } else {
            float widthScale = Screen.width / canvasScaler.referenceResolution.x;
            float heightScale = Screen.height / canvasScaler.referenceResolution.y;
            return Mathf.Lerp(widthScale, heightScale, canvasScaler.matchWidthOrHeight);
        }
    }

    public static void SetWidth(this RectTransform rectTransform, float width) {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    public static void SetHeight(this RectTransform rectTransform, float height) {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public static float GetWidth(this RectTransform rectTransform) {
        return rectTransform.rect.size.x;
    }

    public static float GetHeight(this RectTransform rectTransform) {
        return rectTransform.rect.size.y;
    }

    public static void ForceUpdateLayout(this RectTransform rectTransform) {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public static void ForceUpdateCanvas(this RectTransform rectTransform) {
        Canvas.ForceUpdateCanvases();
    }
}