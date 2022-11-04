using System;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

public class InputManager : MonoBehaviour {

    #region Singleton
    private static InputManager instance;
    public static InputManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    [Separator("Inputs")]
    [HideInInspector] public Vector2 aimInput;
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public bool primaryInput;
    [HideInInspector] public bool secondaryInput;
    [HideInInspector] public bool alternateInput;
    [HideInInspector] public bool jumpInput;

    [Header("Settings")]
    [SerializeField][Range(0, 2)] public float mouseSensitivity;
    [SerializeField] bool cursorVisible = true;
    [SerializeField] CursorLockMode cursorLockMode;

    #region Inputs

    public void Aim(InputAction.CallbackContext context) {
        aimInput = context.ReadValue<Vector2>() * mouseSensitivity;
    }
    public void Move(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>().normalized;
    }
    public void Primary(InputAction.CallbackContext context) {
        if (context.started) {
            primaryInput = true;

            Cursor.visible = cursorVisible;
            Cursor.lockState = cursorLockMode;
        }
        if (context.canceled) {
            primaryInput = false;
        }
    }
    public void Secondary(InputAction.CallbackContext context) {
        if (context.started) {
            secondaryInput = true;
        }
        if (context.canceled) {
            secondaryInput = false;
        }
    }

    public void Alternate(InputAction.CallbackContext context) {
        if (context.started) {
            alternateInput = true;
        }
        if (context.canceled) {
            alternateInput = false;
        }
    }

    public void Jump(InputAction.CallbackContext context) {
        if (context.started) {
            jumpInput = true;
        }
        if (context.canceled) {
            jumpInput = false;
        }
    }

    #endregion
}