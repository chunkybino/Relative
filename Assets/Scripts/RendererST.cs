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

    ComputeBuffer prevPosBuffer;
    static readonly int prevPosBufferID = Shader.PropertyToID("_PrevPosBuffer");
    bool prevPosBufferDirty;

    void OnEnable()
    {
        if (prevPosBuffer == null) CreatePrevPosBuffer();
    }
    void OnDisable()
    {
        ReleaseBuffers();
    }

    void Start()
    {
        CreateMatBlock();
    }

    void Update()
    {
        if (matBlock == null) 
        {
            CreateMatBlock();
        }

        matBlock.SetVector("_Color", color);

        matBlock.SetVector("_Velocity", transformST.realVel);

        matBlock.SetFloat("_C", transformST.C);

        if (prevPosBufferDirty)
        {
            prevPosBufferDirty = false;
            matBlock.SetBuffer(prevPosBufferID, prevPosBuffer);
        }
        matBlock.SetFloat("_PrevPosCount", transformST.prevPositions.Length);
        matBlock.SetFloat("_PrevPosCurrentIndex", transformST.prevPosWriteIndex);
        matBlock.SetFloat("_PrevPosCurrentTime", transformST.currentTime);

        renderer.SetPropertyBlock(matBlock);
    }

    void FixedUpdate()
    {
        SetPrevPosBuffer();
    }

    void CreateMatBlock()
    {
        matBlock = new MaterialPropertyBlock();

        Bounds newBounds = new Bounds();
        newBounds.max = new Vector3(99999,99999,99999);
        newBounds.min = new Vector3(-99999,-99999,-99999);
        renderer.bounds = newBounds;
    }

    void CreatePrevPosBuffer()
    {
        prevPosBuffer = new ComputeBuffer(transformST.prevPositions.Length, System.Runtime.InteropServices.Marshal.SizeOf(typeof(TransformST.PrevPosData)));
        //matBlock.SetGlobalBuffer("_LightDataNoShadow", prevPosBuffer);
    }

    void SetPrevPosBuffer()
    {
        prevPosBuffer.SetData(transformST.prevPositions);
        prevPosBufferDirty = true;
    }

    void ReleaseBuffers()
    {
        prevPosBuffer?.Release();
    }
}
