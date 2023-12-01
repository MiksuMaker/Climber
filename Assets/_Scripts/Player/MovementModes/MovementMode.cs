using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    public virtual void Enter() { }

    public virtual void Move(Vector2 moveInput) { }
    public virtual void Move(Vector2 moveInput, float climbBoost) { }
}

public class MovePosition : MovementMode
{
    public MovePosition(PlayerMover _mover) : base(_mover)
    {
    }

    public override void Enter() 
    {
        mover.rb.drag = stats.walk_drag;
    }

    public override void Move(Vector2 moveInput)
    {
        moveInput = moveInput.normalized;

        Vector3 changeVector = mover.transform.right * moveInput.x 
                             + mover.transform.forward * moveInput.y;
        changeVector = changeVector * Time.fixedDeltaTime * stats.walking_speed;

        Vector3 nextPos = mover.transform.position + changeVector;
        mover.rb.MovePosition(nextPos);
    }

    public override void Move(Vector2 moveInput, float climbBoost)
    {
        moveInput = moveInput.normalized;

        Vector3 changeVector = mover.transform.right * moveInput.x
                             + Vector3.up * climbBoost
                             + mover.transform.forward * moveInput.y;
        changeVector = changeVector * Time.fixedDeltaTime * stats.walking_speed;

        Vector3 nextPos = mover.transform.position + changeVector;
        mover.rb.MovePosition(nextPos);
    }
}

public class AddForce : MovementMode
{
    protected float physicsMultiplier = 10f;

    public AddForce(PlayerMover _mover) : base(_mover) { }

    public override void Move(Vector2 moveInput)
    {
        // Use RigidBody.AddForce AND Velocity clamping

        moveInput = moveInput.normalized;

        Vector3 forceVector = mover.transform.right * moveInput.x 
                            + mover.transform.forward * moveInput.y;
        forceVector = forceVector * Time.fixedDeltaTime * stats.walking_speed * physicsMultiplier;

        //mover.rb.AddForce(forceVector, ForceMode.Acceleration);
        //mover.rb.AddForce(forceVector, ForceMode.Impulse);
        mover.rb.AddForce(forceVector, ForceMode.VelocityChange);

        LimitVelocity();
    }

    public virtual void LimitVelocity()
    {
        // Get only the horizontal velocity
        Vector3 hv = mover.rb.velocity;
        hv = new Vector3(hv.x, 0f, hv.z);

        float limit = (stats.walking_velocityLimit_horizontal * stats.walking_velocityLimit_horizontal);

        // Check if velocity is over limit
        if (hv.sqrMagnitude > limit)
        {
            // If so, limit it horizontally
            Vector3 legalVelocity = hv.normalized * stats.walking_velocityLimit_horizontal;

            // Decrease the velocity that goes over the limit by the multiplier
            Vector3 overVelocity = (hv - legalVelocity) * stats.walk_damper;

            // Add it to the final velocity
            hv = legalVelocity + overVelocity;
            Debug.Log("Velocity: " + hv.magnitude);
            hv = new Vector3(hv.x, mover.rb.velocity.y, hv.z);

            mover.rb.velocity = hv;

        }
    }
}

public class ClimbingMoveMode : AddForce
{
    public ClimbingMoveMode(PlayerMover _mover) : base(_mover) { }

    public override void Move(Vector2 moveInput)
    {
        // First check if touching ground
        if (mover.CheckIfGrounded()) 
        {
            #region Grounded movement
            moveInput = moveInput.normalized;

            Vector3 changeVector = mover.transform.right * moveInput.x
                                 + mover.transform.forward * moveInput.y;
            changeVector = changeVector * Time.fixedDeltaTime * stats.walking_speed;

            Vector3 nextPos = mover.transform.position + changeVector;
            mover.rb.MovePosition(nextPos);
            return;
            #endregion
        }

        moveInput = moveInput.normalized;

        Vector3 inputVector = mover.transform.right * moveInput.x
                           + mover.transform.forward * moveInput.y;

        Vector3 moveVector = inputVector * stats.climbing_horizontal_towards;

        // Check if there is obstacle in the direction
        if (mover.DetectCollisionInDirection(inputVector))
        {
            // Add some upward climbforce to the equation
            moveVector += Vector3.up * stats.climbing_vertical_boost;
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

public class ClimbingPositionChange : MovePosition
{
    public ClimbingPositionChange(PlayerMover _mover) : base(_mover) { }

    public override void Enter()
    {
        mover.rb.drag = mover.moveStats.climb_drag;
    }

    public override void Move(Vector2 moveInput)
    {
        moveInput = moveInput.normalized;

        Vector3 pos = mover.transform.position;
        Vector3 forward = mover.transform.forward;
        Vector3 right = mover.transform.right;

        #region Wall Check
        // Check if movement is towards the wall
        float climbBoost = 0f;
        if (mover.DetectCollisionInDirection(forward))
        {
            // Add upwards movement determined whether over or under COP
            if (mover.CheckIfOverCOP())
            {
                climbBoost = stats.climbing_vertical_over;
            } 
            else
            {
                climbBoost = stats.climbing_vertical_boost;
            }
        }
        #endregion

        // Check if touching ground
        if (mover.CheckIfGrounded()) { base.Move(moveInput, climbBoost); return; }

        #region Towards or Away Check
        // Check if movement is towards or away from COP (Center of Pull)
        float sidewaysInput = moveInput.x;
        float forwardsInput = moveInput.y;

        Vector3 grabPos = mover.grabber.center;

        Vector3 dirToCOP = (
                            new Vector3(grabPos.x, 0f, grabPos.z)
                            -
                            new Vector3(pos.x, 0f, pos.z)
                            ).normalized;

        // Sideways
        float sideDOT = Vector3.Dot(dirToCOP, right);
        float forwardDOT = Vector3.Dot(dirToCOP, forward);

        //Debug.DrawRay(pos, dirToCOP, Color.red, 2f);
        //Debug.Log("Side DOT value: " + sideDOT);
        //Debug.Log("Forward DOT value: " + forwardDOT);

        // Forwards power
        float forwardPower = 0f;
        float sidewayPower = 0f;

        // Test if the multiplication is positive
        if ((forwardDOT * forwardsInput) > 0f)
        {
            // If so, add towards power
            forwardPower = stats.climbing_horizontal_towards;
        } else { forwardPower = stats.climbing_horizontal_away; }

        if ((sideDOT * sidewaysInput) > 0f)
        {
            sidewayPower = stats.climbing_horizontal_towards;
        } else { sidewayPower = stats.climbing_horizontal_away; }
        #endregion

        Vector3 moveVector = right * sidewayPower * sidewaysInput
                           + Vector3.up * climbBoost
                           + forward * forwardPower * forwardsInput;

        moveVector = pos + moveVector * Time.fixedDeltaTime;
        mover.rb.MovePosition(moveVector);

        LimitVelocity();
    }

    public virtual void LimitVelocity()
    {
        // Get only the horizontal velocity
        Vector3 hv = mover.rb.velocity;
        hv = new Vector3(hv.x, 0f, hv.z);

        float limit = (stats.climbing_velocityLimit_horizontal * stats.climbing_velocityLimit_horizontal);

        // Check if velocity is over limit
        if (hv.sqrMagnitude > limit)
        {
            // If so, limit it horizontally
            Vector3 legalVelocity = hv.normalized * stats.climbing_velocityLimit_horizontal;

            // Decrease the velocity that goes over the limit by the multiplier
            Vector3 overVelocity = (hv - legalVelocity) * stats.climb_damper;

            // Add it to the final velocity
            hv = legalVelocity + overVelocity;
            Debug.Log("Velocity: " + hv.magnitude);
            hv = new Vector3(hv.x, mover.rb.velocity.y, hv.z);

            mover.rb.velocity = hv;

        }
    }
}

public class FallingMode : MovePosition
{
    public FallingMode(PlayerMover _mover) : base(_mover) { }

    public override void Enter()
    {
        mover.rb.drag = stats.fall_drag;
    }

    public override void Move(Vector2 moveInput)
    {
        moveInput = moveInput.normalized;

        Vector3 changeVector = mover.transform.right * moveInput.x
                             + mover.transform.forward * moveInput.y;
        changeVector = changeVector * Time.fixedDeltaTime * stats.falling_movementSpeed;

        Vector3 nextPos = mover.transform.position + changeVector;
        mover.rb.MovePosition(nextPos);
    }

}