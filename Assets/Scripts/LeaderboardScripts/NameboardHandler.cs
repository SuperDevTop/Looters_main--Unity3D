using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameboardHandler : MonoBehaviour
{
    #region Properties
    [Header("Components Reference")]
    [SerializeField] private Text rankTxt = null;
    [SerializeField] private Text nameTxt = null;
    [SerializeField] private Image flagImg = null;
    #endregion

    #region Public Core Functions
    public void SetupName(string name)
    {
        nameTxt.text = name;
    }

    public void SetupFlag(Sprite s)
    {
        flagImg.sprite = s;
    }

    public void SetupRank(int rank)
    {
        rankTxt.text = rank.ToString();
    }
    #endregion
}
