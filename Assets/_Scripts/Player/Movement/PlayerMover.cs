using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    #region Properties
    [SerializeField] PlayerInputObject inputComponent;
    Rigidbody rb;

    [SerializeField] float movementSpeed = 2f;


    #endregion

    #region Setup
    private void Start()
    {
        // Fetch references
        rb = GetComponent<Rigidbody>();
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

        moveInput = moveInput.normalized;

        Vector3 changeVector = transform.right * moveInput.x + transform.forward * moveInput.y;
        changeVector = changeVector * Time.deltaTime * movementSpeed;

        Vector3 nextPos = transform.position + changeVector;
        rb.MovePosition(nextPos);
    }

    #endregion
}