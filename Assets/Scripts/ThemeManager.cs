using System.Collections.Generic;
using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public bool useTestTheme = false;
    public int testThemeIndex = 0;
    public List<ThemeMaterial> themeMaterials = new List<ThemeMaterial>();

    void Start ()
    {
        int themeIndex = Random.Range(0, themeMaterials[0].floor1Col.Length);
        ApplyTheme(themeIndex);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Start();
        }
    }

    public void ApplyTheme (int themeIndex)
    {
        if (useTestTheme)
            themeIndex = testThemeIndex;
        foreach (ThemeMaterial themeMaterial in themeMaterials)
        {
            themeMaterial.floor1Mat.color = themeMaterial.floor1Col[themeIndex];
            themeMaterial.floor2Mat.color = themeMaterial.floor2Col[themeIndex];
        }
    }

    public void ChangeThemeColor()
    {
        Start();
    }
}

[System.Serializable]
public class ThemeMaterial
{
    public Material floor1Mat, floor2Mat;
    public Color[] floor1Col, floor2Col;
}