using UnityEngine;
using System.Collections;

public class TransformST : MonoBehaviour
{
    float C {get{return Frame.C;}}

    public float Cmult {get{
        return Mathf.Max(1 - Vector3.Dot(position,Frame.singleton.rimlerInverse), -100);
    }}

    public Vector3 position {get{return transform.position;} set{transform.position = value;}}
    public Vector4 position4 {get{return new Vector4(position.x,position.y,position.z,0);}}

    public float gamma {get{return 1 / Mathf.Sqrt(1 - m_velocity.sqrMagnitude/(C*C));}}
    public float oppositeGamma {get{return 1 / Mathf.Sqrt(1 + m_velocityProper.sqrMagnitude/(C*C));}} //gamma but it takes in the proper velocity and gives you that factor you need to mult by to get velocity

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

    public Vector3 accelerationProper;

    void OnEnable()
    {
        Frame.singleton.onBoost.AddListener(Boost);
    }

    void FixedUpdate()
    {
        //velocityProper += accelerationProper * Time.fixedDeltaTime * gamma;
        
        transform.position += velocity * Time.fixedDeltaTime;// * Cmult;
    }

    public void Boost(Matrix4x4 mat)
    {
        Vector4 newPos = mat * position4;
        Vector4 newVel = mat * velocity4;

        newVel *= C/newVel.w;
        //newPos -= newPos.w * newVel;

        velocity = newVel;
        position = newPos;
    }
}
