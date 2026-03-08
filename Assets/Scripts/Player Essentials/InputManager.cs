using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static float SteeringInput { get; private set; }

    public static bool AccelerateInputPressed { get; private set; }

    public static bool AccelerateInputHeld { get; private set; }

    public static bool DecelerateInputPressed { get; private set; }

    public static bool DecelerateInputHeld { get; private set; }

    public static bool PauseInputPressed { get; private set; }

    public static bool PauseInputHeld { get; private set; }

    public static bool IsGameplayInputEnabled { get; set; }

    // Because domain reload is off, static variables must be initialized in Awake
    void Awake()
    {
        SteeringInput = 0;
        AccelerateInputPressed = false;
        AccelerateInputHeld = false;
        DecelerateInputPressed = false;
        DecelerateInputHeld = false;
        PauseInputPressed = false;
        PauseInputHeld = false;
        IsGameplayInputEnabled = true;
    }

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