using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static bool disableInput;
    bool m_inputDisabled;

    public PlayerInputActions inputs;

    public bool up {get{return m_up.IsPressed();}}
    public bool left {get{return m_left.IsPressed();}}
    public bool down {get{return m_down.IsPressed();}}
    public bool right {get{return m_right.IsPressed();}}
    public bool forward {get{return m_forward.IsPressed();}}
    public bool backward {get{return m_backward.IsPressed();}}

    public bool brake {get{return m_brake.IsPressed();}}

    public bool enableLSD {get{return m_enableLSD.IsPressed();}}

    public Vector2 mouseDelta {get{return m_mouse.ReadValue<Vector2>();}}

    [HideInInspector] public InputAction m_up;
    [HideInInspector] public InputAction m_down;
    [HideInInspector] public InputAction m_left;
    [HideInInspector] public InputAction m_right;
    [HideInInspector] public InputAction m_forward; 
    [HideInInspector] public InputAction m_backward;

    [HideInInspector] public InputAction m_mouse;

    [HideInInspector] public InputAction m_brake;

    [HideInInspector] public InputAction m_enableLSD;

    void Awake()
    {
        inputs = new PlayerInputActions();
    }

    void OnEnable()
    {
        if (inputs == null) inputs = new PlayerInputActions();
        inputs.Player.Enable();

        SetUpActions();
    }
    void OnDisable()
    {
        inputs.Player.Disable();
    }

    void SetUpActions()
    {
        m_up = inputs.Player.Up;
        m_left = inputs.Player.Left;
        m_down = inputs.Player.Down;
        m_right = inputs.Player.Right;
        m_forward = inputs.Player.Forward;
        m_backward = inputs.Player.Backward;

        m_brake = inputs.Player.Brake;

        m_enableLSD = inputs.Player.EnableLSD;

        m_mouse = inputs.Player.MouseDelta;
    }

    void Update()
    {
        if (disableInput != m_inputDisabled)
        {
            m_inputDisabled = disableInput;

            if (disableInput) {
                inputs.Player.Disable();
            } else {
                inputs.Player.Enable();
            }
        }
    }
}
