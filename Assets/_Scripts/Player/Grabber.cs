using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    #region Properties
    PlayerInputObject inputComponent;
    [SerializeField] GameObject head;
    [SerializeField] GameObject crossHairObj;

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
        bool hitFound = false;
        Vector3 rayDir = head.transform.forward;
        Ray ray = new Ray(pos, rayDir);
        //if (Physics.Raycast(pos, out hit, ray, grabDistance, grabbingLayerMask))
        if (Physics.Raycast(ray, out hit, grabDistance, grabbingLayerMask))
        {
            hitColor = Color.green;
            hitFound = true;

            PlaceCrossHair(hit);
        }

        if (!hitFound) { PlaceCrossHair(hit, false); }
        
        Debug.DrawRay(pos, rayDir * grabDistance, hitColor, 0.1f);
    }

    private void PlaceCrossHair(RaycastHit hit, bool hitFound = true)
    {
        if (!hitFound) { crossHairObj.transform.position = head.transform.position; return; }

        crossHairObj.transform.position = hit.point;
    }
    #endregion
}