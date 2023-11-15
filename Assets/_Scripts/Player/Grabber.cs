using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    #region Properties
    [SerializeField]
    PlayerInputObject inputComponent;
    [SerializeField] GameObject head;
    [SerializeField] GameObject crossHairObj;

    int grabbingLayerMask;


    [Header("Grabbing")]
    [SerializeField] GameObject handGraphics;
    [SerializeField] float grabDistance = 5f;

    bool raycastHitWithinReach = false;
    RaycastHit rayHit;

    Hand leftHand;
    Hand rightHand;
    #endregion

    #region Setup
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        grabbingLayerMask = ~LayerMask.GetMask("Player");

        leftHand = new Hand(handGraphics);
        rightHand = new Hand(handGraphics, false);
    }

    private void Update()
    {
        CheckGrabDistance();
        HandleGrabbing(inputComponent.leftGrabInput, leftHand);
        HandleGrabbing(inputComponent.rightGrabInput, rightHand);
    }
    #endregion

    #region Aiming
    private void CheckGrabDistance()
    {
        Color hitColor = Color.red;

        Vector3 pos = head.transform.position;
        //RaycastHit lastHit;
        raycastHitWithinReach = false;
        Vector3 rayDir = head.transform.forward;
        Ray ray = new Ray(pos, rayDir);
        if (Physics.Raycast(ray, out rayHit, grabDistance, grabbingLayerMask))
        {
            hitColor = Color.green;
            raycastHitWithinReach = true;

            PlaceCrossHair(rayHit);
        }

        if (!raycastHitWithinReach) { PlaceCrossHair(rayHit, false); }

        Debug.DrawRay(pos, rayDir * grabDistance, hitColor, 0.1f);
    }

    private void PlaceCrossHair(RaycastHit hit, bool hitFound = true)
    {
        if (!hitFound) { crossHairObj.transform.position = head.transform.position; return; }

        crossHairObj.transform.position = hit.point;

        Debug.DrawRay(crossHairObj.transform.position, hit.normal * 2f, Color.red, 0.1f);
    }
    #endregion

    #region Grabbing
    private void HandleGrabbing(bool grabInput, Hand hand)
    {
        // Check inputs
        if (grabInput)
        {
            // Check if not currently grabbing AND if anything is in reach
            if (!hand.grabbing && raycastHitWithinReach)
            {
                // Grab it with the hand
                hand.PlaceHand(rayHit.point, rayHit.normal);
            }
        }
        else
        {
            // Ungrab it
            hand.UnplaceHand();
        }
    }
    #endregion

}

public class Hand
{
    public bool grabbing = false;
    public Vector3 grabPos = Vector3.zero;
    public GameObject graphics = null;
    public bool isLeftHand = true;

    public Hand(GameObject handGraphics, bool isLeft = true)
    {
        graphics = GameObject.Instantiate(handGraphics);
        graphics.SetActive(false);
        isLeftHand = isLeft;
    }

    public void PlaceHand(Vector3 placementPos, Vector3 normal)
    {
        grabbing = true;
        graphics.SetActive(true);
        graphics.transform.position = placementPos;
        graphics.transform.up = normal;

        graphics.transform.localScale = isLeftHand ? new Vector3(1f, 1f, 1f) : new Vector3(-1f, 1f, 1f);
    }

    public void UnplaceHand()
    {
        grabbing = false;
        graphics.SetActive(false);
    }
}
