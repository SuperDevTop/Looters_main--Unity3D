using UnityEngine;

public class RippleGenerator : MonoBehaviour
{
    public Material RippleMaterial;
    public float MaxAmount = 50f;
 
    [Range(0,1)]
    public float Friction = .9f;
 
    private float Amount = 0f;
    public Vector2 ripplePosition;
 
    void Update()
    {
        // if (Input.GetMouseButton(0))
        // {
        //     this.Amount = this.MaxAmount;
        //     // Vector3 pos = Input.mousePosition;
        //     this.RippleMaterial.SetFloat("_CenterX", ripplePosition.x);
        //     this.RippleMaterial.SetFloat("_CenterY", ripplePosition.y);
        // }
 
        this.RippleMaterial.SetFloat("_Amount", this.Amount);
        this.Amount *= this.Friction;
    }

    public void GenerateRippleEffect()
    {
        this.Amount = this.MaxAmount;
        this.RippleMaterial.SetFloat("_CenterX", ripplePosition.x);
        this.RippleMaterial.SetFloat("_CenterY", ripplePosition.y);

        this.RippleMaterial.SetFloat("_Amount", this.Amount);
        this.Amount *= this.Friction;
    }
 
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, this.RippleMaterial);
    }
}