using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    #region Properties
    [SerializeField] PlayerInputObject inputComponent;
    public Rigidbody rb;

    [SerializeField] public float movementSpeed = 2f;
    [SerializeField] public float velocityLimit = 2f;
    [Tooltip("0 = No damping, 1 = No limitation")]
    [SerializeField, Range(0f, 1f)] public float limitMultiplier = 1f;

    public MoveType currentMode = MoveType.walking;

    MovementMode walkingMode;
    MovementMode climbingMode;
    MovementMode fallingMode;

    #endregion

    #region Setup
    private void Start()
    {
        // Fetch references
        rb = GetComponent<Rigidbody>();
        SetupMovementModes();
    }

    private void SetupMovementModes()
    {
        walkingMode = new MovePosition(this);
        //walkingMode = new AddForce(this);
        climbingMode = new AddForce(this);
    }

    private void FixedUpdate()
    {
        HandleMovement(inputComponent.moveValue);
    }
    #endregion

    #region Movement
    private void HandleMovement(Vector2 moveInput)
    {
        if (moveInput == Vector2.zero) { return; }

        switch (currentMode)
        {
            case MoveType.walking:
                walkingMode.Move(moveInput);
                break;
            default: Debug.Log("This movement type has not been implemented yet!");
                break;
        }
    }
    #endregion
}