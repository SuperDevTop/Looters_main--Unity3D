using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINameBoardTweenHandler : MonoBehaviour
{
    #region Properties
    [Header("Attributes")]
    [SerializeField] private float tweenTime = 0f;
    [SerializeField] private float scaleUpMultiplier = 0f;
    #endregion

    #region MonoBehaviour Functions
    private void Start()
    {
        //Tween();
    }
    #endregion

    #region Public Core Functions
    public void Tween()
    {
        LeanTween.cancel(gameObject);
        transform.localScale = Vector3.one;

        LeanTween.scale(gameObject, Vector3.one * scaleUpMultiplier, tweenTime).setEasePunch();
    }
    #endregion
}
