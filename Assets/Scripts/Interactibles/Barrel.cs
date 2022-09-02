using UnityEngine;

public class Barrel : MonoBehaviour
{
    public Collider collider;
    public GameObject fullMesh, brokenMesh, spriteObj, blastParticle;

    public void BlastBarrel()
    {
        CasualSauce.sauce.PlayerGains();
        fullMesh.SetActive(false);
        spriteObj.SetActive(false);

        blastParticle.SetActive(true);
        brokenMesh.SetActive(true);
    }
}