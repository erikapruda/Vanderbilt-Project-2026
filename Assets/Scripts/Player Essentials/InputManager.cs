using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static float SteeringInput { get; private set; } = 0f;

    public static bool AccelerateInputPressed { get; private set; } = false;

    public static bool AccelerateInputHeld { get; private set; } = false;

    public static bool DecelerateInputPressed { get; private set; } = false;

    public static bool DecelerateInputHeld { get; private set; } = false;

    public static bool PauseInputPressed { get; private set; } = false;

    public static bool PauseInputHeld { get; private set; } = false;

    public static bool IsGameplayInputEnabled { get; set; } = true;

    void LateUpdate()
    {
        AccelerateInputPressed = false;
        DecelerateInputPressed = false;
        PauseInputPressed = false;
    }

    public void OnSteeringInput(InputAction.CallbackContext context)
    {
        SteeringInput = IsGameplayInputEnabled ? context.ReadValue<float>() : 0f;
    }

    public void OnAccelerateInput(InputAction.CallbackContext context)
    {
        AccelerateInputPressed = IsGameplayInputEnabled && (context.started || AccelerateInputPressed);
        AccelerateInputHeld = IsGameplayInputEnabled && context.performed;
    }

    public void OnDecelerateInput(InputAction.CallbackContext context)
    {
        DecelerateInputPressed = IsGameplayInputEnabled && (context.started || DecelerateInputPressed);
        DecelerateInputHeld = IsGameplayInputEnabled && context.performed;
    }

    public void OnPauseInput(InputAction.CallbackContext context)
    {
        DecelerateInputPressed = context.started || PauseInputPressed;
        DecelerateInputHeld = context.performed;
    }
}