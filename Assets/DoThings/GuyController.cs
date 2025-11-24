using UnityEngine;

public class GuyController : MonoBehaviour
{
    public Frame frame;

    [SerializeField] Vector3 accelInput;

    [SerializeField] Vector3 accel;
    [SerializeField] float maxAccel = 2;
    [SerializeField] float accelRate = 1;

    void FixedUpdate()
    {
        accelInput = Vector2.zero;
        if (Input.GetKey("a")) accelInput.x--;
        if (Input.GetKey("d")) accelInput.x++;
        if (Input.GetKey("s")) accelInput.y--;
        if (Input.GetKey("w")) accelInput.y++;
        if (Input.GetKey("e")) accelInput.z++;
        if (Input.GetKey("q")) accelInput.z--;

        accelInput = accelInput.normalized;

        accel = Vector3.MoveTowards(accel, accelInput*maxAccel, accelRate*Time.fixedDeltaTime);

        frame.acceleration = accel;
    }
}
