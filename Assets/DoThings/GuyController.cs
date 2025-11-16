using UnityEngine;

public class GuyController : MonoBehaviour
{
    public Frame frame;

    [SerializeField] Vector2 accelInput;

    [SerializeField] float accelRate = 0.5f;

    void FixedUpdate()
    {
        accelInput = Vector2.zero;
        if (Input.GetKey("a")) accelInput.x--;
        if (Input.GetKey("d")) accelInput.x++;
        if (Input.GetKey("s")) accelInput.y--;
        if (Input.GetKey("w")) accelInput.y++;

        accelInput = accelInput.normalized;

        frame.acceleration = accelInput*accelRate;
    }
}
