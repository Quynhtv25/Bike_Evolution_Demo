using DG.Tweening;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class ProgesstionEndGame : MonoBehaviour {
    [System.Serializable]
    private class CustomAngle {
        public float startAngle;
        public float endAngle;
        public float valueMutiple;
    }
    public float MutilpleValue {
        get {
            StopLoopArrow();
            return mutilple;
        }
    }

    #region SerializeField

    [SerializeField] private Transform arrowMutiple;
    [SerializeField] private CustomAngle[] angleMutilple;
    [SerializeField] private TextMeshProUGUI[] textMultiple;
    [SerializeField] private float angle;


    [Header("Custom")]
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private Ease easeLoop = Ease.Flash;

    #endregion

    #region Field

    private float mutilple;
    private Tween arrowTween;
    private TextMeshProUGUI coinTextWithAds;

    #endregion

    #region Public Method
    /// <summary>
    /// Method to Set text that show mutiple coin
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="numberCoin"></param>
    public void Init(TextMeshProUGUI txt, float numberCoin) {
        coinTextWithAds = txt;
        LoopArrow();
        StartCoroutine(SetText(numberCoin));
    }
    /// <summary>
    /// Method to Set text that show mutiple coin
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="numberCoin"></param>
    public void Init(float numberCoin = 100) {
        LoopArrow();
        StartCoroutine(SetText(numberCoin));
    }

    
    #endregion

    #region PRIVATE

    /// <summary>
    /// Method to stop loop arrow
    /// </summary>
    private void StopLoopArrow() {
        if (arrowTween != null) {
            arrowTween.Kill();
            arrowTween = null;
        }

        mutilple = GetNumberMutiple(GetCurrentAngle());
    }


    /// <summary>
    /// Get MutipleNumber
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    private float GetNumberMutiple(float angle) {
        CustomAngle value = angleMutilple.FirstOrDefault(CustomAngle => (angle >= CustomAngle.startAngle && angle <= CustomAngle.endAngle));
        return value != null ? value.valueMutiple : 1;
    }

    /// <summary>
    /// Get Angle of arrow
    /// </summary>
    /// <returns></returns>
    private float GetCurrentAngle() => Mathf.Abs(arrowMutiple.localRotation.eulerAngles.z > 180 ? arrowMutiple.localRotation.eulerAngles.z - 360 : arrowMutiple.localRotation.eulerAngles.z);


    /// <summary>
    /// Method to start loop arrow
    /// </summary>
    private void LoopArrow() {
        if (arrowTween != null) return;
        arrowMutiple.DOLocalRotate(new Vector3(0, 0, angle), Time.deltaTime);
        arrowTween = arrowMutiple.DOLocalRotate(new Vector3(0, 0, -angle), duration).SetLoops(-1, LoopType.Yoyo).SetEase(easeLoop);
    }


    /// <summary>
    /// Coroutine to set text coin 
    /// </summary>
    /// <param name="numberCoin"></param>
    /// <returns></returns>
    IEnumerator SetText(float numberCoin) {
        while (arrowTween != null) {
            if (coinTextWithAds) {
                coinTextWithAds.SetText(((int)(numberCoin * mutilple)).ToString());
            }
            yield return null;
        }
    }

    #endregion
}
