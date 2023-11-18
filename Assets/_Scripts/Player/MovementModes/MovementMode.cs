using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType
{
    walking, climbing, falling
}

public class MovementMode
{
    protected PlayerMover mover;
    protected MoveStatsObject stats { get { return mover.moveStats; } }

    public MovementMode(PlayerMover _mover)
    {
        mover = _mover;
    }

    public virtual void Move(Vector3 moveInput) 
    { }
}

public class MovePosition : MovementMode
{
    public MovePosition(PlayerMover _mover) : base(_mover)
    {
    }

    public override void Move(Vector3 moveInput)
    {
        moveInput = moveInput.normalized;

        Vector3 changeVector = mover.transform.right * moveInput.x 
                             + mover.transform.forward * moveInput.y;
        changeVector = changeVector * Time.fixedDeltaTime * stats.walking_movementSpeed;

        Vector3 nextPos = mover.transform.position + changeVector;
        mover.rb.MovePosition(nextPos);
    }
}

public class AddForce : MovementMode
{
    protected float physicsMultiplier = 10f;

    public AddForce(PlayerMover _mover) : base(_mover) { }

    public override void Move(Vector3 moveInput)
    {
        // Use RigidBody.AddForce AND Velocity clamping

        moveInput = moveInput.normalized;

        Vector3 forceVector = mover.transform.right * moveInput.x 
                            + mover.transform.forward * moveInput.y;
        forceVector = forceVector * Time.fixedDeltaTime * stats.climbing_horizontal_towards * physicsMultiplier;

        mover.rb.AddForce(forceVector, ForceMode.Force);

        LimitVelocity();
    }

    public virtual void LimitVelocity()
    {
        // Check if velocity is over limit
        if (mover.rb.velocity.sqrMagnitude > (stats.velocityLimit * stats.velocityLimit))
        {
            // If so, limit it
            Vector3 legalVelocity = mover.rb.velocity.normalized * stats.velocityLimit;

            // Decrease the velocity that goes over the limit by the multiplier
            Vector3 overVelocity = (mover.rb.velocity - legalVelocity) * stats.overLimitMultiplier;

            // Add them together for the final velocity
            mover.rb.velocity = legalVelocity + overVelocity;

            Debug.Log("limiting velocity");
        }
    }
}

public class ClimbingMoveMode : AddForce
{
    public ClimbingMoveMode(PlayerMover _mover) : base(_mover) { }

    public override void Move(Vector3 moveInput)
    {
        // First check if touching ground
        if (mover.CheckIfGrounded()) 
        {
            #region Grounded movement
            moveInput = moveInput.normalized;

            Vector3 changeVector = mover.transform.right * moveInput.x
                                 + mover.transform.forward * moveInput.y;
            changeVector = changeVector * Time.fixedDeltaTime * stats.walking_movementSpeed;

            Vector3 nextPos = mover.transform.position + changeVector;
            mover.rb.MovePosition(nextPos);
            return;
            #endregion
        }

        Debug.Log("Climbing!");

        moveInput = moveInput.normalized;

        Vector3 inputVector = mover.transform.right * moveInput.x
                           + mover.transform.forward * moveInput.y;

        Vector3 moveVector = inputVector * stats.climbing_horizontal_towards;

        // Check if there is obstacle in the direction
        if (mover.DetectCollisionInDirection(inputVector))
        {
            // Add some upward climbforce to the equation
            moveVector += Vector3.up * stats.climbing_vertical_below;
        }

        moveVector *= Time.fixedDeltaTime * physicsMultiplier;

        mover.rb.AddForce(moveVector, ForceMode.Force);

        LimitVelocity();
    }

    public virtual void CheckIfClimbingUp(Vector3 inputVector)
    {
        // First check if the Player is lower than the center of pull
        //if (mover.transform.position.y <= )

        if (mover.DetectCollisionInDirection(inputVector))
        {
            
        }
    }
}