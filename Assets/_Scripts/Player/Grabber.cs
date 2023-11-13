using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    #region Properties
    PlayerInputObject inputComponent;
    [SerializeField] GameObject head;

    int grabbingLayerMask;


    [Header("Grabbing")]
    [SerializeField] float grabDistance = 5f;
    #endregion

    #region Setup
    private void Start()
    {
        grabbingLayerMask =~ LayerMask.GetMask("Player");
    }

    private void Update()
    {
        CheckGrabDistance();
    }
    #endregion

    #region Functions
    private void CheckGrabDistance()
    {
        Color hitColor = Color.red;

        Vector3 pos = head.transform.position;
        RaycastHit hit;
        Vector3 rayDir = head.transform.forward;
        if (Physics.Raycast(pos, rayDir, grabDistance, grabbingLayerMask))
        {
            hitColor = Color.green;
        }

        Debug.DrawRay(pos, rayDir * grabDistance, hitColor, 0.1f);
        //Debug.DrawRay(head.transform.position, rayDir * grabbingLayerMask, hitColor, 1f);
    }
    #endregion
}