using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]

public class CasualSauceEditor : MonoBehaviour
{
#if UNITY_EDITOR
    void OnEnable()
    {
        EditorApplication.hierarchyWindowItemOnGUI += CasualSauceEditor.HierarchyIconCallback;
    }

    void OnDisable()
    {
        EditorApplication.hierarchyWindowItemOnGUI -= CasualSauceEditor.HierarchyIconCallback;
    }

    public static void HierarchyIconCallback(int instanceID, Rect rectangle)
    {
        GameObject go = (GameObject)EditorUtility.InstanceIDToObject(instanceID);
        if (go != null && go.GetComponent<CasualSauce>() != null)
        {
            Texture2D ycIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Textures/CKIO_Logo.png", typeof(Texture2D));
            Graphics.DrawTexture(new Rect(GUILayoutUtility.GetLastRect().width - rectangle.height - 5, rectangle.y, rectangle.height, rectangle.height), ycIcon);
        }
    }
#endif

}