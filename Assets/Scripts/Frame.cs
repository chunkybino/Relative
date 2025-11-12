using UnityEngine;
using UnityEngine.Events;

public class Frame : MonoBehaviour
{
    public static Frame singleton;

    public static float C = 10;
    
    public float speedOfLight = 10;

    public Vector3 acceleration;

    public bool isInterial {get{return acceleration != Vector3.zero;}}

    public Vector3 rimler;
    public Vector3 rimlerInverse;

    public static float Gamma(Vector3 velocity)
    {
        return 1 / Mathf.Sqrt(1 - velocity.sqrMagnitude/(C*C));
    }

    public UnityEvent<Matrix4x4> onBoost = new UnityEvent<Matrix4x4>();

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

    [SerializeField] Vector3 boostVel;
    [SerializeField] bool doBoost;
    [SerializeField] Matrix4x4 recentBoost;

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

        rimlerInverse = -acceleration/(C*C);
        rimler = rimlerInverse/rimlerInverse.sqrMagnitude;
    }

    void FixedUpdate()
    {
        if (isInterial)
        {
            BoostFrame(acceleration * Time.fixedDeltaTime);
        }
    }

    public void BoostFrame(Vector3 vel)
    {
        Matrix4x4 mat = LorentzBoost(vel);
        recentBoost = mat;

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
