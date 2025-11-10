using UnityEngine;
using System.Collections;

public class TransformST : MonoBehaviour
{
    float C {get{return Frame.C;}}

    public Vector3 position {get{return transform.position;} set{transform.position = value;}}
    public Vector4 position4 {get{return new Vector4(position.x,position.y,position.z,0);}}

    public float gamma = 1;

    public Vector3 velocity;
    public Vector3 velocityProper;

    public Vector4 velocity4 {get{return new Vector4(velocity.x,velocity.y,velocity.z,C);}}

    void OnEnable()
    {
        Frame.singleton.onBoost.AddListener(Boost);
    }

    void Update()
    {
        gamma = Frame.Gamma(velocity);
        velocityProper = gamma*velocity;
    }

    void FixedUpdate()
    {
        transform.position += velocity * Time.fixedDeltaTime;
    }

    public void Boost(Matrix4x4 mat)
    {
        Vector4 newPos = mat * position4;
        Vector4 newVel = mat * velocity4;

        newVel *= C/newVel.w;
        newPos -= newPos.w * newVel;

        velocity = newVel;
        position = newPos;
    }
}
