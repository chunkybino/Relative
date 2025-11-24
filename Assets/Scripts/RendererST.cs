using UnityEngine;
using UnityEngine.Rendering;

public class RendererST : MonoBehaviour
{
    public TransformST transformST;

    public MeshRenderer renderer;
    public MeshFilter meshFilter;
    public Mesh mesh;

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

        matBlock.SetVector("_Velocity", transformST.realVel);

        matBlock.SetFloat("_C", transformST.C);

        renderer.SetPropertyBlock(matBlock);
    }
}
