using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NameData", menuName = "ScriptableObjects/NameGenerator", order = 2)]
public class RandomNameGenerator: ScriptableObject
{
    #region Properties
    [Header("Attributes")]
    [SerializeField] private List<string> names = new List<string>();
    #endregion

    #region Getter And Setter
    public List<string> GeneratedNames { get; set; }
    #endregion

    #region Public Core Functions
    public List<string> GenerateNames(int amount)
    {
        GeneratedNames = new List<string>();
        List<string> tempNames = new List<string>();
        foreach (string s in names)
        {
            tempNames.Add(s);
        }
        int index = 0;
        for (int i = 0; i < amount; i++)
        {
            index = Random.Range(0, tempNames.Count);
            GeneratedNames.Add(tempNames[index]);
            tempNames.RemoveAt(index);
        }

        return GeneratedNames;
    }
    #endregion
}
