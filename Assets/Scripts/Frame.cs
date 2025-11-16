using UnityEngine;
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

    public void BoostFrame(Vector3 vel)
    {
        Matrix4x4 mat = LorentzBoost(vel);

        onBoost?.Invoke(mat);
    }

    public static Matrix4x4 LorentzBoost(Vector3 vel)
    {
        vel = vel/C;
        float gamma = Gamma(vel);

        float gamSqrWeird = gamma*gamma/(1+gamma);

        Matrix4x4 mat = new Matrix4x4();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                float result = gamSqrWeird;
                if (i == 3 || j == 3) result = gamma;

                if (i < 3) result *= vel[i];
                if (j < 3) result *= vel[j];

                if (i == j && i < 3) result += 1;

                mat[i,j] = result;
            }
        }

        return mat;
    }
}
