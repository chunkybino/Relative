using UnityEngine;

public class DumbGoomba : MonoBehaviour
{
    [SerializeField] TransformST transformST;

    [SerializeField] float centerPos = 14;

    [SerializeField] Vector3 accel = new Vector3(0,0,2);
    [SerializeField] float accelMax = 1;
    int accelDir = 1;

    float deltaProperTime;
    float prevProperClock;

    void FixedUpdate()
    {
        deltaProperTime = transformST.properClock - prevProperClock;
        prevProperClock = transformST.properClock;

        if (transformST.basePosition.z > centerPos) {
            accelDir = -1;
        } else {
            accelDir = 1;
        }

        transformST.AccelerateProperVel(accelDir*accel*deltaProperTime);
    }
}
