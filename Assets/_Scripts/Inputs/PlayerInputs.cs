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

        if (mostRecentMovementInput != Vector2.zero) { Debug.Log(mostRecentMovementInput); }
    }

    private void HandleLookInput()
    {
        float mouseX = Input.GetAxis(axisX);
        float mouseY = Input.GetAxis(axisY);

        // Update PlayerInputComponent
        inputComponent.lookValue = new Vector2(mouseX, mouseY);
    }
    #endregion
}