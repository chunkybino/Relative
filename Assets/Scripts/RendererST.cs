using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class RendererST : MonoBehaviour
{
    public TransformST transformST;

    public Color color = Color.white;

    public MeshRenderer renderer;
    public MeshFilter meshFilter;

    MaterialPropertyBlock matBlock;

    void Update()
    {
        if (matBlock == null) 
        {
            matBlock = new MaterialPropertyBlock();

            Bounds newBounds = new Bounds();
            newBounds.max = new Vector3(99999,99999,99999);
            newBounds.min = new Vector3(-99999,-99999,-99999);
            renderer.bounds = newBounds;
        }

        matBlock.SetVector("_Color", color);

        matBlock.SetVector("_Velocity", transformST.realVel);

        matBlock.SetFloat("_C", transformST.C);

        renderer.SetPropertyBlock(matBlock);
    }
}
