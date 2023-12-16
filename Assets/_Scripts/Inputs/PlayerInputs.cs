using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    #region Properties
    // Controls
    KeyCode forward = KeyCode.W;
    KeyCode left = KeyCode.A;
    KeyCode backward = KeyCode.S;
    KeyCode right = KeyCode.D;

    KeyCode jump = KeyCode.Space;

    KeyCode hand_Left = KeyCode.Mouse0;
    KeyCode hand_Right = KeyCode.Mouse1;

    KeyCode pickDropThrow_Left= KeyCode.Q;
    KeyCode equipUnequip_Left = KeyCode.Tab;

    KeyCode pickDropThrow_Right = KeyCode.E;
    KeyCode equipUnequip_Right = KeyCode.F;

    KeyCode minus = KeyCode.KeypadMinus;
    KeyCode plus = KeyCode.KeypadPlus;

    string axisX = "Mouse X";
    string axisY = "Mouse Y";

    [SerializeField]
    PlayerInputObject inputComponent;

    [SerializeField] Vector2 moveVector;
    #endregion

    #region Setup
    private void Start()
    {
        if (inputComponent == null) { Debug.Log("PlayerInput is missing PlayerInputObject!"); }

        inputComponent.moveValue = Vector2.zero;
        inputComponent.lookValue = Vector2.zero;
    }

    private void Update()
    {
        if (inputComponent == null) { return; }

        HandleInputs();
    }
    #endregion

    #region Input Handling
    private void HandleInputs()
    {
        HandleMovementInputs();
        HandleLookInput();
        HandleJumpingInput();
        HandleMouseInput();

        HandleInventoryInput();

        HandleOtherInput();
    }

    private void HandleMovementInputs()
    {
        Vector2 mostRecentMovementInput = Vector2.zero;

        if (Input.GetKey(forward)) { mostRecentMovementInput += Vector2.up; }
        if (Input.GetKey(left)) { mostRecentMovementInput += Vector2.left; }
        if (Input.GetKey(backward)) { mostRecentMovementInput += Vector2.down; }
        if (Input.GetKey(right)) { mostRecentMovementInput += Vector2.right; }
        
        // Update PlayerInputComponent
        inputComponent.moveValue = mostRecentMovementInput;

        moveVector = mostRecentMovementInput;
    }

    private void HandleJumpingInput()
    {
        if (Input.GetKeyDown(jump)) { inputComponent.jumpValue = true; }
    }

    private void HandleLookInput()
    {
        float mouseX = Input.GetAxis(axisX);
        float mouseY = Input.GetAxis(axisY);

        // Update PlayerInputComponent
        inputComponent.lookValue = new Vector2(mouseX, mouseY);
    }

    private void HandleMouseInput()
    {
        if (Input.GetKeyDown(hand_Left)) { inputComponent.LeftHandInput = true; }
        else if (Input.GetKeyUp(hand_Left)) { inputComponent.LeftHandInput = false; }
        if (Input.GetKeyDown(hand_Right)) { inputComponent.RightHandInput = true; }
        else if (Input.GetKeyUp(hand_Right)) { inputComponent.RightHandInput = false; }

    }

    private void HandleInventoryInput()
    {
        if (Input.mouseScrollDelta.y != 0f) { inputComponent.MouseDelta = (int)Input.mouseScrollDelta.y; }

        if (Input.GetKeyDown(pickDropThrow_Right)) { inputComponent.RightPickupInput = true; }
        else if (Input.GetKeyUp(pickDropThrow_Right)) { inputComponent.RightPickupInput = false; }
        if (Input.GetKeyDown(equipUnequip_Right)) { inputComponent.RightEquipInput = true; }

        if (Input.GetKeyDown(pickDropThrow_Left)) { inputComponent.LeftPickupInput = true; }
        else if (Input.GetKeyUp(pickDropThrow_Left)) { inputComponent.LeftPickupInput = false; }
        if (Input.GetKeyDown(equipUnequip_Left)) { inputComponent.LeftEquipInput = true; }
    }

    private void HandleOtherInput()
    {
        // Mouse Sensitivity
        if (Input.GetKeyDown(minus)) { inputComponent.sensitivityInput = -1; }
        else if (Input.GetKeyDown(plus)) { inputComponent.sensitivityInput = 1; }
    }

    #endregion
}