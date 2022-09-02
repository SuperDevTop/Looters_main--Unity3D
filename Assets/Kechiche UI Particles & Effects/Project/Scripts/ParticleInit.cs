using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInit : MonoBehaviour
{
    public GameObject smoke;
    public Transform target;
    public Transform FireWorkTarget;
    public Transform WinEffectTarget;

    public Canvas canvas;
    public GameObject FireWork;
    public GameObject Winnefect1;
    public GameObject Winnefect2;
    public GameObject Winnefect3;
    public GameObject Panel;


    //2D Particles needs to be  instantiated in canvas
    public void Fire()
    {
        GameObject newSmoke = Instantiate(smoke, target.localPosition, target.localRotation) as GameObject;
        newSmoke.transform.SetParent(canvas.transform, false);
        Destroy(newSmoke, 0.5f);
        Panel.SetActive(false);


    }
    public void FireWorks()
    {
        
        GameObject Firework = Instantiate(FireWork, FireWorkTarget.localPosition, FireWorkTarget.localRotation) as GameObject;
        Firework.transform.SetParent(canvas.transform, false);
        Destroy(Firework,3f);
        Panel.SetActive(false);

    }

    public void Win1()
    {
        Panel.SetActive(true);
        GameObject WinEffect = Instantiate(Winnefect1, WinEffectTarget.localPosition, WinEffectTarget.localRotation) as GameObject;
        WinEffect.transform.SetParent(canvas.transform, false);
        Destroy(WinEffect, 2f);
        

    }
    public void Win2()
    {
        Panel.SetActive(true);
        GameObject WinEffect = Instantiate(Winnefect2, WinEffectTarget.localPosition, WinEffectTarget.localRotation) as GameObject;
        WinEffect.transform.SetParent(canvas.transform, false);
        Destroy(WinEffect, 2f);
        

    }
    public void Win3()
    {
        Panel.SetActive(true);
        GameObject WinEffect = Instantiate(Winnefect3, WinEffectTarget.localPosition, WinEffectTarget.localRotation) as GameObject;
        WinEffect.transform.SetParent(canvas.transform, false);
        Destroy(WinEffect, 2f);
        

    }

}
