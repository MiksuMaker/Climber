using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    #region Properties
    [SerializeField] PlayerInputObject inputComponent;
    Rigidbody rb;

    [Header("Jumping")]
    [SerializeField] float jumpPower = 10f;
    #endregion

    #region Setup
    private void Start()
    {
        // Fetch references
        rb = GetComponent<Rigidbody>();
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

        Jump();
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }
    #endregion
}