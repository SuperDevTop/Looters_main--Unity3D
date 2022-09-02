using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{
    public Vector3 rotationAxis = new Vector3(0, 1, 0);
    public float rotationSpeed = 30;

    void Update ()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
