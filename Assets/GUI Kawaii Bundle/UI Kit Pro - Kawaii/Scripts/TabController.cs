using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabController : MonoBehaviour
{
    public int tabCount;

    public void SetTabFront(GameObject tabObject)
    {
        tabObject.transform.SetSiblingIndex(tabCount);
    }
}
