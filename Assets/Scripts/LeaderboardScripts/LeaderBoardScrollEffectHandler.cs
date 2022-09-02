using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardScrollEffectHandler : MonoBehaviour
{
    #region Properties
    [Header("Properties")]
    [SerializeField] private float scrollSpeed = 0;

    [Header("Components Reference")]
    [SerializeField] private RectTransform rectTransform = null;
    [SerializeField] private PlayerNameBoardTweenHandler playerNameBoardTweenHandler = null;

    private float scrollTarget = 0f;
    private Vector3 targetPosition = Vector3.zero;
    #endregion

    #region MonoBehaviour Functions
    private void Start()
    {
        scrollTarget = rectTransform.localPosition.y;
        scrollTarget -= 60;
        SetupScrollTarget(LeaderBoardScrollOffset);
    }

    private void Update()
    {
        if (rectTransform.localPosition.y > scrollTarget + .2f)
        {
            rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, targetPosition, Time.deltaTime * scrollSpeed);
            //rectTransform.Translate(-Vector3.up * Time.deltaTime * scrollSpeed);
        }
        else
        {
            playerNameBoardTweenHandler.enabled = true;
            this.enabled = false;
        }
    }
    #endregion

    #region Getter And Setter
    public float LeaderBoardScrollOffset { get; set; }
    #endregion

    #region Public Core Functions
    public void SetupScrollTarget(float target)
    {
        scrollTarget += target;
        targetPosition = new Vector3(0, scrollTarget, 0);
    }
    #endregion
}
