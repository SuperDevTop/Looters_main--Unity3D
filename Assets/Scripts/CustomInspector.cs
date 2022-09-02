using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ThemeManager))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ThemeManager tm = (ThemeManager)target;
        if(GUILayout.Button("Change Color ٩(◕‿◕｡)۶"))
        {
            tm.ChangeThemeColor();
        }
    }
}