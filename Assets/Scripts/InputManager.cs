using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static float SteeringInput { get; private set; } = 0f;

    public static bool AccelerateInputPressed { get; private set; } = false;

    public static bool AccelerateInputHeld { get; private set; } = false;

    public static bool DecelerateInputPressed { get; private set; } = false;

    public static bool DecelerateInputHeld { get; private set; } = false;

    void LateUpdate()
    {
        AccelerateInputPressed = false;
        DecelerateInputPressed = false;
    }

    public void OnSteeringInput(InputAction.CallbackContext context)
    {
        SteeringInput = -context.ReadValue<float>();
    }

    public void OnAccelerateInput(InputAction.CallbackContext context)
    {
        AccelerateInputPressed = context.started || AccelerateInputPressed;
        AccelerateInputHeld = context.performed;
    }

    public void OnDecelerateInput(InputAction.CallbackContext context)
    {
        DecelerateInputPressed = context.started || DecelerateInputPressed;
        DecelerateInputHeld = context.performed;
    }
}