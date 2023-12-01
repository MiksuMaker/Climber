using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    #region Properties
    [SerializeField] PlayerInputObject inputComponent;
    PlayerMover mover;
    Rigidbody rb;

    [SerializeField] bool infiniteJumps = false;

    [Header("Jumping")]
    [SerializeField] float jumpPower = 10f;
    #endregion

    #region Setup
    private void Start()
    {
        // Fetch references
        rb = GetComponent<Rigidbody>();
        mover = GetComponent<PlayerMover>();
    }

    private void FixedUpdate()
    {
        HandleJumping();
    }
    #endregion

    #region Functions
    private void HandleJumping()
    {
        if (!inputComponent.jumpValue) { return; }

        inputComponent.jumpValue = false;

        // Check if jumping is permitted AND change mode
        if (infiniteJumps || mover.CheckIfGrounded()) 
        {
            mover.ChangeMovementMode(MoveType.jumping); 
        }
        else 
        { 
            return; 
        }

        Jump();
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }
    #endregion
}