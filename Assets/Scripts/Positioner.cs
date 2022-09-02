using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Positioner : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    public bool isHealthBar = false;

    [Range(0, 1)]
    public float health;

    public Image healthFillImg;
    public Color hpMaxCol, hpMinCol;

    void Update()
    {
        if (target != null)
        {
            gameObject.transform.position = target.transform.position + offset;
        }

        if (isHealthBar & healthFillImg != null)
        {
            healthFillImg.fillAmount = health;
            healthFillImg.color = Color.Lerp(hpMaxCol, hpMinCol, health);
        }
    }
}