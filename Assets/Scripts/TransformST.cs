using UnityEngine;
using System.Collections;
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

    public Vector3 fakeVel;
    public Vector3 realVel;

    public Vector4 realVel4 {get{return new Vector4(realVel.x,realVel.y,realVel.z,C);}}

    [SerializeField] Transform skewParent;

    [SerializeField] Vector3 basePosition;

    void OnEnable()
    {
        frame = Frame.singleton;

        frame.onBoost.AddListener(Boost);
    }

    void Start()
    {
        basePosition = position;

        SpawnSkewerThing();
    }
    void SpawnSkewerThing()
    {
        GameObject obj = new GameObject(gameObject.name);
        skewParent = obj.transform;
        transform.SetParent(skewParent);

        skewParent.position = transform.position;
        transform.localPosition = new Vector3(0,0,0);
    }

    void FixedUpdate()
    {
        BoostFromBase();

        /*
        if (frame.isInterial)
        {
            fakeVel -= frame.acceleration * Time.fixedDeltaTime;
            realVel = fakeVel * Frame.OppositeGamma(fakeVel);

            ContractRimler(frame.rimler);
        }
        else
        {
            skewParent.position += realVel * Time.fixedDeltaTime;
            //position += realVel * Time.fixedDeltaTime;

            SetContract();
        }
        */
    }

    public void ContractRimler(Vector3 rimler)
    {
        Vector3 rimlerDis = position - rimler;
        float rimlerDot = Vector3.Dot(rimlerDis,-rimler)/rimler.magnitude;

        //Vector3 parVel = rimler.normalized*Vector3.Dot(realVel, rimler.normalized);
        //Vector3 perpVel = realVel-parVel;

        skewParent.position += realVel * (rimlerDot/rimler.magnitude) * Time.fixedDeltaTime;

        SetContract();

        /*
        skewParent.localScale = new Vector3(Mathf.Sqrt(1 - realVel.sqrMagnitude/(C*C)), 1, 1);

        //Quaternion currentQuart = skewParent.rotation;
        Quaternion directionQuart = Quaternion.FromToRotation(new Vector3(1,0,0), realVel);
        skewParent.rotation = directionQuart;
        transform.localRotation = Quaternion.Inverse(directionQuart);
        */
        
        //skewParent.position = position;
        //transform.localPosition = Vector3.zero;
    }

    void SetContract()
    {
        skewParent.localScale = new Vector3(Mathf.Sqrt(1 - realVel.sqrMagnitude/(C*C)), 1, 1);

        //Quaternion currentQuart = skewParent.rotation;
        Quaternion directionQuart = Quaternion.FromToRotation(new Vector3(1,0,0), realVel);
        skewParent.rotation = directionQuart;
        transform.localRotation = Quaternion.Inverse(directionQuart);
    }

    public void Boost(Matrix4x4 mat)
    {
        //Vector4 newPos = mat * position4;
        Vector4 newPos = mat * skewParent.position;
        Vector4 newVel = mat * realVel4;

        newVel *= C/newVel.w;
        newPos -= newPos.w * newVel;

        realVel = newVel;

        skewParent.position = newPos;
        //position = newPos;
    }

    public void BoostFromBase()
    {
        Matrix4x4 mat = frame.frameVelMatrix;

        Vector3 relativePos = basePosition - frame.framePos;

        //Vector4 newPos = mat * position4;
        Vector4 newPos = mat * (Vector4)relativePos;
        Vector4 newVel = mat * new Vector4(0,0,0,C);

        //print(newPos.w);

        newVel *= C/newVel.w;
        //newPos -= newPos.w * newVel / C;

        realVel = newVel;

        //skewParent.position = newPos;
        //position = newPos;

        if (realVel != Vector3.zero)
        {
            Vector3 posInVelDirection = realVel*Vector3.Dot(relativePos, realVel)/realVel.sqrMagnitude;
            Vector3 posOutVelDirection = relativePos - posInVelDirection;

            skewParent.position = posInVelDirection/gamma + posOutVelDirection;
        }
        else
        {
            skewParent.position = newPos;
        }

        SetContract();
    }
}
