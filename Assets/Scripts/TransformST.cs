using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class TransformST : MonoBehaviour
{
    Frame frame;

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

    public Vector3 realPosition;
    //public Vector3 realVel;

    public Vector3 accelStartPos;
    public Vector3 accelStartVel;

    public Vector3 fakeVel;

    public float rimlerNum;

    Vector3 frameAccel;

    void OnEnable()
    {
        frame = Frame.singleton;

        frame.onBoost.AddListener(Boost);
    }
    void Start()
    {
        realPosition = position;
    }

    void FixedUpdate()
    {
        velocityProper += accelerationProper * Time.fixedDeltaTime * gamma;

        if (frameAccel != frame.acceleration)
        {
            frameAccel = frame.acceleration;

            accelStartPos = position;
            accelStartVel = fakeVel;
        }

        if (frame.isInterial && !float.IsNaN(frame.rimler.x))
        {
            Vector3 rimlerDistance = position - frame.rimler;
            rimlerNum = Vector3.Dot(rimlerDistance, -frame.rimlerInverse);
            
            fakeVel -= frame.acceleration * Time.deltaTime;

            position += fakeVel * Time.fixedDeltaTime * rimlerNum;

            /*
            float velSlope = (accelStartVel.x)/C;
            float rimler = frame.rimler.x;

            float newX = (accelStartPos.x - rimler)*(1/Mathf.Sqrt(1-(velSlope*velSlope)))/math.cosh((C/rimler)*frame.timeAccel + Arctanh(velSlope)) + rimler;

            float newVelX = -(C/rimler)*(newX-rimler)*math.tanh((C/rimler)*frame.timeAccel + Arctanh(velSlope));

            Vector3 newPos = position;
            newPos.x = newX;
            position = newPos;

            rimlerNum = Mathf.Abs((newPos.x-rimler)/rimler);

            fakeVel.x = newVelX/rimlerNum;

            float Arctanh(float t)
            {
                return Mathf.Log(Mathf.Sqrt((1+t)/(1-t)));
            }
            */
        }
        else
        {
            position += fakeVel * Time.fixedDeltaTime;
        }
        
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
