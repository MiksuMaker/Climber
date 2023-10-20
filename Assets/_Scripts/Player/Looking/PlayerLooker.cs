using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLooker : MonoBehaviour
{
    #region Properties
    [SerializeField] PlayerInputObject inputComponent;

    [SerializeField] GameObject head;

    [SerializeField]
    float lookSpeed = 5f;
    #endregion

    #region Setup
    private void Update()
    {
        HandleLooking(inputComponent.lookValue);
    }
    #endregion

    #region Functions
    private void HandleLooking(Vector2 lookInput)
    {
        if (lookInput == Vector2.zero) { return; }

        //head.transform.Rotate(head.transform.right, -lookInput.y);
        //Vector3 r = head.transform.rotation.eulerAngles;
        //Debug.Log("Rot: " + r);
        //r = new Vector3(r.x - (lookInput.y * 5f),
        //                r.y,
        //                r.z);
        //head.transform.localRotation = Quaternion.Euler(r);
        //Debug.Log("Rot: " + r);

        float change = lookInput.y * lookSpeed * Time.deltaTime;
        Vector3 pitch = new Vector3(head.transform.eulerAngles.x + change,
                                    head.transform.eulerAngles.y,
                                    head.transform.eulerAngles.z);
        head.transform.rotation = Quaternion.Euler(pitch);
    }
    #endregion
}