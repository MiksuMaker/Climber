using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    #region Properties
    [SerializeField] PlayerInputObject inputComponent;

    Rigidbody rb;

    [SerializeField] float movementSpeed = 2f;
    [SerializeField] float rotationSpeed = 2f;
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
        HandleRotation(inputComponent.lookValue.x);
    }
    #endregion

    #region Functions
    private void HandleMovement(Vector2 moveInput)
    {
        if (moveInput == Vector2.zero) { return; }

        moveInput = moveInput.normalized;

        //Vector3 changeVector = new Vector3(transform.right.x * moveInput.x,
        //                                   0f,
        //                                   transform.forward.z * moveInput.y)
        //                                   * movementSpeed
        //                                   * Time.fixedDeltaTime;

        Vector3 changeVector = transform.right * moveInput.x + transform.forward * moveInput.y;
        changeVector = changeVector * Time.deltaTime * movementSpeed;

        Debug.Log(changeVector);
        Debug.Log("Right: " + transform.right);
        Debug.Log("Forward: " + transform.forward);

        Vector3 nextPos = transform.position + changeVector;
        rb.MovePosition(nextPos);
    }

    private void HandleRotation(float rotation)
    {
        if (rotation == 0f) { return; }

        transform.Rotate(Vector3.up * rotation * rotationSpeed);
    }
    #endregion
}