using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    #region Properties
    [SerializeField] PlayerInputObject inputComponent;
    public Rigidbody rb;

    [SerializeField] public float movementSpeed = 2f;

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
    }

    private void FixedUpdate()
    {
        HandleMovement(inputComponent.moveValue);
    }
    #endregion

    #region Functions
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

public enum MoveType
{
    walking, climbing, falling
}

public abstract class MovementMode
{
    protected PlayerMover mover;

    public virtual void Move(Vector3 moveInput) { }
}

public class MovePosition : MovementMode
{
    public MovePosition(PlayerMover _mover)
    {
        mover = _mover;
    }

    public override void Move(Vector3 moveInput)
    {
        moveInput = moveInput.normalized;

        Vector3 changeVector = mover.transform.right * moveInput.x + mover.transform.forward * moveInput.y;
        changeVector = changeVector * Time.deltaTime * mover.movementSpeed;

        Vector3 nextPos = mover.transform.position + changeVector;
        mover.rb.MovePosition(nextPos);
    }
}