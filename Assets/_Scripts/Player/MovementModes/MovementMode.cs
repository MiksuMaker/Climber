using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType
{
    walking, climbing, falling
}

public abstract class MovementMode
{
    protected PlayerMover mover;

    public MovementMode(PlayerMover _mover)
    {
        mover = _mover;
    }

    public virtual void Move(Vector3 moveInput) { }
}

public class MovePosition : MovementMode
{
    public MovePosition(PlayerMover _mover) : base(_mover)
    {
    }

    public override void Move(Vector3 moveInput)
    {
        moveInput = moveInput.normalized;

        Vector3 changeVector = mover.transform.right * moveInput.x + mover.transform.forward * moveInput.y;
        //changeVector = changeVector * Time.deltaTime * mover.movementSpeed;
        changeVector = changeVector * Time.fixedDeltaTime * mover.movementSpeed;

        Vector3 nextPos = mover.transform.position + changeVector;
        mover.rb.MovePosition(nextPos);
    }
}

public class AddForce : MovementMode
{
    float physicsMultiplier = 10f;

    public AddForce(PlayerMover _mover) : base(_mover) { }

    public override void Move(Vector3 moveInput)
    {
        // Use RigidBody.AddForce AND Velocity clamping

        moveInput = moveInput.normalized;

        Vector3 forceVector = mover.transform.right * moveInput.x + mover.transform.forward * moveInput.y;
        //forceVector = forceVector * Time.deltaTime * mover.movementSpeed;
        forceVector = forceVector * Time.fixedDeltaTime * mover.movementSpeed * physicsMultiplier;

        //Vector3 nextPos = mover.transform.position + changeVector;
        //mover.rb.MovePosition(nextPos);

        //mover.rb.AddForce(forceVector, ForceMode.Force);
        //mover.rb.AddForce(forceVector, ForceMode.Acceleration);
        mover.rb.AddForce(forceVector, ForceMode.Impulse);
        //mover.rb.AddForce(forceVector, ForceMode.VelocityChange);

        LimitVelocity();
    }

    public virtual void LimitVelocity()
    {
        // Check if velocity is over limit
        if (mover.rb.velocity.sqrMagnitude > (mover.velocityLimit * mover.velocityLimit))
        {
            // If so, limit it
            Vector3 legalVelocity = mover.rb.velocity.normalized * mover.velocityLimit;

            // Decrease the velocity that goes over the limit by the multiplier
            Vector3 overVelocity = (mover.rb.velocity - legalVelocity) * mover.limitMultiplier;

            // Add them together for the final velocity
            mover.rb.velocity = legalVelocity + overVelocity;

            Debug.Log("limiting velocity");
        }
    }
}
