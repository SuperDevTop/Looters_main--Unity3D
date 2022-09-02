using UnityEngine;
using Lofelt.NiceVibrations;

public class Coin : MonoBehaviour
{
    public GameObject coinParticle;
    public Collider coll;
    public GameObject meshh;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
            coinParticle.SetActive(true);
            coll.enabled = false;
            meshh.SetActive(false);
        }
    }
}