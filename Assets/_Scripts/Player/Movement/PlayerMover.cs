using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    #region Properties
    [SerializeField] PlayerInputObject inputComponent;
    [SerializeField] GameObject head;
    Rigidbody rb;

    [SerializeField] float movementSpeed = 2f;


    [SerializeField] float mouseSensitivity = 10f;
    #endregion

    #region Setup
    private void Start()
    {
        // Fetch references
        rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        HandleMovement(inputComponent.moveValue);
    }

    private void Update()
    {
        HandleRotation(inputComponent.lookValue.x);
        HandleLooking(inputComponent.lookValue);
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

    private void HandleRotation(float rotation)
    {
        if (rotation == 0f) { return; }

        transform.Rotate(Vector3.up * rotation * mouseSensitivity * Time.deltaTime);
    }

    private void HandleLooking(Vector2 lookInput)
    {
        if (lookInput == Vector2.zero) { return; }

        float change = lookInput.y * mouseSensitivity * Time.deltaTime;
        Vector3 pitch = new Vector3(head.transform.localEulerAngles.x - change,
                                    0f,
                                    0f);

        head.transform.localRotation = Quaternion.Euler(pitch);
    }
    #endregion
}