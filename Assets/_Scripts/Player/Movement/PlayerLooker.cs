using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLooker : MonoBehaviour
{
    #region Properties
    [SerializeField] PlayerInputObject inputComponent;
    [SerializeField] GameObject head;
    float headPitch = 0f;

    [SerializeField] float mouseSensitivity = 10f;
    #endregion

    #region Setup
    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleRotation(inputComponent.lookValue.x);
        HandleLooking(inputComponent.lookValue);
    }
    #endregion

    #region Functions

    private void HandleRotation(float rotation)
    {
        if (rotation == 0f) { return; }

        transform.Rotate(Vector3.up * rotation * mouseSensitivity * 100f * Time.deltaTime);
    }

    private void HandleLooking(Vector2 lookInput)
    {
        //Debug.DrawRay(head.transform.position, head.transform.forward, Color.red, 0.1f);

        if (lookInput == Vector2.zero) { return; }

        float max = 80f;
        float min = -80f;

        headPitch += lookInput.y * mouseSensitivity * 100f * Time.deltaTime;
        headPitch = Mathf.Clamp(headPitch, min, max);
        head.transform.localRotation = Quaternion.Euler(-headPitch, 0f, 0f);

    }
    #endregion

}