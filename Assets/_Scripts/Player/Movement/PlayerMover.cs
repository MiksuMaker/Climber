using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    #region Properties
    [SerializeField] PlayerInputObject inputComponent;
    [SerializeField] GameObject head;
    float headPitch = 0f;
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

        //float change = lookInput.y * mouseSensitivity * Time.deltaTime;
        //float pitchValue = head.transform.localEulerAngles.x - change;

        ////Debug.Log("Pitch: " + head.transform.localEulerAngles.x);
        //float upPitchLimit = 250f;
        //float downPitchLimit = 70f;

        //if (!(pitchValue > upPitchLimit || pitchValue < downPitchLimit))
        //{
        //    Debug.Log("Outside");
        //}
        ////if (pitchValue < minPitch) { pitchValue = minPitch; Debug.Log("Smaller than min pitch"); }
        ////else if (pitchValue > maxPitch) { pitchValue = maxPitch; Debug.Log("Bigger than Max value"); }
        ////float pitchValue = Mathf.Clamp(head.transform.localEulerAngles.x - change, 60f, 70f);
        ////Debug.Log(pitchValue);
        //Vector3 pitch = new Vector3(pitchValue,
        //                        0f,
        //                        0f);

        float max = 80f;
        float min = -80f;

        headPitch += lookInput.y * mouseSensitivity * Time.deltaTime;
        headPitch = Mathf.Clamp(headPitch, min, max);
        head.transform.localRotation = Quaternion.Euler(-headPitch, 0f, 0f);

        //head.transform.localRotation = Quaternion.Euler(pitch);
    }
    #endregion
}