using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class TransformST : MonoBehaviour
{
    Frame frame;

    public float C {get{return Frame.C;}}

    public Vector3 position {get{return transform.position;} set{transform.position = value;}}
    public Vector4 position4 {get{return new Vector4(position.x,position.y,position.z,0);}}

    public float gamma {get{return 1 / Mathf.Sqrt(1 - realVel.sqrMagnitude/(C*C));}}
    //public float oppositeGamma {get{return 1 / Mathf.Sqrt(1 + m_velocityProper.sqrMagnitude/(C*C));}} //gamma but it takes in the proper velocity and gives you that factor you need to mult by to get velocity

    /*
    Vector3 m_velocity;
    Vector3 m_velocityProper;

    public Vector3 velocity {
        get{return m_velocity;} 
        set{
            m_velocity = value;
            m_velocityProper = gamma*m_velocity;
        }
    }
    public Vector3 velocityProper {
        get{return m_velocityProper;} 
        set{
            m_velocityProper = value;
            m_velocity = m_velocityProper*oppositeGamma;
        }
    }

    public Vector4 velocity4 {get{return new Vector4(velocity.x,velocity.y,velocity.z,C);}}
    */

    //public Vector3 accelerationProper;

    public Vector3 realVel;

    public Vector3 basePosition;
    public Vector3 baseVelocity;

    public Vector4 baseLengthContractionVector = new Vector4(0,0,0,1);//xyz is direction, w is magnitude
    public Vector4 realLengthContractionVector = new Vector4(0,0,0,1);//xyz is direction, w is magnitude

    public int prevPosWriteIndex = 0;
    public PrevPosData[] prevPositions = new PrevPosData[128];

    public float currentBaseTime = 0;
    public float deltaBaseTime;
    public float properClock = 0;

    [System.Serializable]
    public struct PrevPosData
    {
        public Vector4 pos; //w coord is time
        public Vector4 vel; //w coord is length contraction factor (1/gamma)
    }

    void OnEnable()
    {
        frame = Frame.singleton;
    }

    void Start()
    {
        basePosition = position;
    }

    void FixedUpdate()
    {
        BoostFromBase();

        //basePosition += baseVelocity * deltaBaseTime;

        properClock += deltaBaseTime / Frame.Gamma(baseVelocity);

        //UpdatePrevPos();

        //currentRealTime += Time.fixedDeltaTime;

        //length contraction vector time
        baseLengthContractionVector = (Vector4)baseVelocity.normalized; 
        baseLengthContractionVector.w = 1/Frame.Gamma(baseVelocity);
        realLengthContractionVector = (Vector4)realVel.normalized; 
        realLengthContractionVector.w = 1/gamma;
    }

    public void BoostFromBase()
    {
        Matrix4x4 mat = frame.frameVelMatrix;

        Vector3 relativePos = basePosition - frame.framePos;

        //Vector4 newPos = mat * position4;
        Vector4 newPos = mat * new Vector4(relativePos.x, relativePos.y, relativePos.z, frame.currentBaseTime - currentBaseTime);
        Vector4 newVel = mat * new Vector4(baseVelocity.x,baseVelocity.y,baseVelocity.z,C);

        newVel *= C/newVel.w;
        //newPos -= newPos.w * newVel / C;

        realVel = newVel;

        position = (Vector3)newPos - (realVel/C)*newPos.w;

        newPos = new Vector4(position.x,position.y,position.z,0);

        float newBaseTime = frame.currentBaseTime + Vector3.Dot(relativePos,frame.frameVel)/(C*C); //dot the distacne with the frame velocity to see what time this object gets observed by the frame

        deltaBaseTime = newBaseTime - currentBaseTime;
        currentBaseTime = newBaseTime;

        basePosition += baseVelocity*deltaBaseTime;
    }

    public void AccelerateProperVel(Vector3 accel)
    {
        baseVelocity += accel / gamma;
    }

    /*
    void UpdatePrevPos()
    {
        prevPositions[prevPosWriteIndex].pos = basePosition;
        prevPositions[prevPosWriteIndex].pos.w = currentRealTime;

        prevPositions[prevPosWriteIndex].vel = baseVelocity;
        prevPositions[prevPosWriteIndex].vel.w = 1/Frame.Gamma(baseVelocity);

        prevPosWriteIndex++;
        if (prevPosWriteIndex >= prevPositions.Length) prevPosWriteIndex = 0;
    }
    */
}
