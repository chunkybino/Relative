using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class RendererST : MonoBehaviour
{
    Frame frame;

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
        frame = Frame.singleton;

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

        matBlock.SetVector("_BasePos", transformST.basePosition);
        matBlock.SetVector("_BaseVel", transformST.baseVelocity);
        matBlock.SetVector("_RealVel", transformST.realVel);

        matBlock.SetVector("_BaseLengthContractionVector", transformST.baseLengthContractionVector);
        matBlock.SetVector("_RealLengthContractionVector", transformST.realLengthContractionVector);

        matBlock.SetVector("_FramePos", frame.framePos);
        matBlock.SetVector("_FrameVel", frame.frameVel);

        matBlock.SetFloat("_C", transformST.C);

        //previous positions stuff
        if (prevPosBufferDirty)
        {
            prevPosBufferDirty = false;
            matBlock.SetBuffer(prevPosBufferID, prevPosBuffer);
        }
        matBlock.SetFloat("_PrevPosCount", transformST.prevPositions.Length);
        matBlock.SetFloat("_PrevPosCurrentIndex", transformST.prevPosWriteIndex);
        matBlock.SetFloat("_PrevPosCurrentTime", frame.currentRealTime);

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
