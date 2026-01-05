using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class Frame : MonoBehaviour
{
    public static Frame singleton;

    public static float C = 10;
    
    public float speedOfLight = 10;

    public Vector3 acceleration;

    public bool isInterial {get{return acceleration != Vector3.zero && rimler != Vector3.zero;}}

    public Vector3 rimler {get{return rimlerInverse/rimlerInverse.sqrMagnitude;}}
    public Vector3 rimlerInverse {get{return -acceleration/(C*C);}}

    public static float Gamma(Vector3 velocity)
    {
        return 1 / Mathf.Sqrt(1 - velocity.sqrMagnitude/(C*C));
    }
    public static float OppositeGamma(Vector3 properVelocity)
    {
        return 1 / Mathf.Sqrt(1 + properVelocity.sqrMagnitude/(C*C));
    }

    public UnityEvent<Matrix4x4> onBoost = new UnityEvent<Matrix4x4>();

    [SerializeField] Vector3 boostVel;
    [SerializeField] bool doBoost;

    public Vector3 framePos;
    public Vector3 frameVel;
    public Vector3 frameProperVel;

    public float frameGamma;

    public Matrix4x4 frameVelMatrix;
    public Matrix4x4 frameVelMatrixInverse;

    public float currentBaseTime;
    public float currentProperTime;

    void Awake()
    {
        CheckSingleton();
    }
    void OnEnable()
    {
        CheckSingleton();
    }
    void CheckSingleton()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(this);
        }
        else
        {
            singleton = this;
        }
    }

    void OnValidate()
    {
        C = speedOfLight;
    }

    void Update()
    {
        if (doBoost)
        {
            doBoost = false;
            BoostFrame(boostVel);
        }
    }

    void FixedUpdate()
    {
        frameProperVel += acceleration * Time.fixedDeltaTime;
        frameVel = frameProperVel*OppositeGamma(frameProperVel);

        framePos += frameProperVel * Time.fixedDeltaTime;

        frameGamma = Gamma(frameVel);

        frameVelMatrix = LorentzBoost(frameVel);
        frameVelMatrixInverse = LorentzBoost(-frameVel);

        currentBaseTime += frameGamma * Time.fixedDeltaTime;
        currentProperTime += Time.fixedDeltaTime;
    }

    public void BoostFrame(Vector3 vel)
    {
        Matrix4x4 mat = LorentzBoost(vel);

        onBoost?.Invoke(mat);
    }

    public static Matrix4x4 LorentzBoost(Vector3 vel)
    {
        vel = vel/C;
        float gamma = 1/Mathf.Sqrt(1-vel.sqrMagnitude); //dont call the gamma function here since velocity has already been normalized to C, instead write out formula again

        float gamSqrWeird = gamma*gamma/(1+gamma);

        Matrix4x4 mat = new Matrix4x4(
            new Vector4(1+gamSqrWeird*(vel.x*vel.x), gamSqrWeird*(vel.x*vel.y),   gamSqrWeird*(vel.x*vel.z),   -gamma*vel.x), 
            new Vector4(gamSqrWeird*(vel.x*vel.y),   1+gamSqrWeird*(vel.y*vel.y), gamSqrWeird*(vel.y*vel.z),   -gamma*vel.y), 
            new Vector4(gamSqrWeird*(vel.x*vel.z),   gamSqrWeird*(vel.y*vel.z),   1+gamSqrWeird*(vel.z*vel.z), -gamma*vel.z),
            new Vector4(-gamma*vel.x, -gamma*vel.y, -gamma*vel.z, gamma)
        );

        return mat;
    }
}
