using UnityEngine;

public class GuyController : MonoBehaviour
{
    public Frame frame;

    [SerializeField] PlayerInput input;

    [SerializeField] Vector3 accelInput;

    [SerializeField] Vector3 accel;
    [SerializeField] float maxAccel = 2;
    [SerializeField] float accelRate = 4;

    //[SerializeField] TransformST referenceAnchor; //object that represents the basis reference frame, used when we need to reset ourself using space

    [SerializeField] Transform cameraTransform;
    [SerializeField] float lookSpeed = 1;

    //makes it so that object rendering is is based off on distance from camera and light travel time
    //really disorienting
    [SerializeField] bool onLSD; 
    [SerializeField] bool advancedTimeBack; 
    [SerializeField] bool colorByTime; 

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //look around
        Vector2 angleVector = input.mouseDelta * lookSpeed;

        if (cameraTransform != null)
        {
            cameraTransform.eulerAngles = cameraTransform.eulerAngles + new Vector3(-angleVector.y,angleVector.x,0);
        }

        accelInput = Vector2.zero;
        if (input.left) accelInput.x--;
        if (input.right) accelInput.x++;
        if (input.forward) accelInput.z++;
        if (input.backward) accelInput.z--;

        if (input.down) accelInput.y--;
        if (input.up) accelInput.y++;

        accelInput = accelInput.normalized;

        float currentCameraAngle = cameraTransform.eulerAngles.y * Mathf.Deg2Rad;

        Vector3 cameraDirectionZ = new Vector3(Mathf.Sin(currentCameraAngle), 0, Mathf.Cos(currentCameraAngle));
        Vector3 cameraDirectionX = new Vector3(Mathf.Cos(currentCameraAngle), 0, -Mathf.Sin(currentCameraAngle));

        accelInput = cameraDirectionX*accelInput.x + new Vector3(0,accelInput.y,0) + cameraDirectionZ*accelInput.z;

        accel = Vector3.MoveTowards(accel, accelInput*maxAccel, accelRate*Time.deltaTime);

        frame.acceleration = accel;
        
        if (input.brake)// && referenceAnchor != null)
        {
            accel = Vector3.zero;
            frame.frameProperVel = Vector3.zero;
        }

        
        if (Input.GetKeyDown("8"))
        {   
            ToggleLSD();
        }
        if (Input.GetKeyDown("9"))
        {   
            ToggleAdvancedTimeBack();
        }
        if (Input.GetKeyDown("0"))
        {   
            ToggleColorByTime();
        }
    }

    void ToggleLSD()
    {
        if (!onLSD)
        {
            Shader.EnableKeyword("LSD_ON");
            Shader.DisableKeyword("LSD_OFF");
        }
        else
        {
            Shader.EnableKeyword("LSD_OFF");
            Shader.DisableKeyword("LSD_ON");
        }
        onLSD = !onLSD;
    }
    void ToggleAdvancedTimeBack()
    {
        if (!advancedTimeBack)
        {
            Shader.EnableKeyword("ADVANCED_TIMEBACK_ON");
            Shader.DisableKeyword("ADVANCED_TIMEBACK_OFF");
        }
        else
        {
            Shader.EnableKeyword("ADVANCED_TIMEBACK_OFF");
            Shader.DisableKeyword("ADVANCED_TIMEBACK_ON");
        }
        advancedTimeBack = !advancedTimeBack;
    }
    void ToggleColorByTime()
    {
        if (!colorByTime)
        {
            Shader.EnableKeyword("COLOR_BY_TIME_ON");
            Shader.DisableKeyword("COLOR_BY_TIME_OFF");
        }
        else
        {
            Shader.EnableKeyword("COLOR_BY_TIME_OFF");
            Shader.DisableKeyword("COLOR_BY_TIME_ON");
        }
        colorByTime = !colorByTime;
    }
}
