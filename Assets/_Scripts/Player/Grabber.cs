using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    #region Properties
    public delegate void OnHandGrab(Vector3 pos, bool isLeftHand);
    public OnHandGrab OnGrab;
    public delegate void OnHandRelease(bool isLeftHand);
    public OnHandRelease OnRelease;

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

    public Hand leftHand;
    public Hand rightHand;

    [HideInInspector]
    public bool isClimbing = false;
    public delegate void OnClimbStatusUpdate(bool climbing);
    public event OnClimbStatusUpdate OnClimbUpdate;
    [HideInInspector]
    public Vector3 center = Vector3.zero;
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
        //Color hitColor = Color.red;

        Vector3 pos = head.transform.position;
        //RaycastHit lastHit;
        raycastHitWithinReach = false;
        Vector3 rayDir = head.transform.forward;
        Ray ray = new Ray(pos, rayDir);
        if (Physics.Raycast(ray, out rayHit, grabDistance, grabbingLayerMask))
        {
            //hitColor = Color.green;
            raycastHitWithinReach = true;

            PlaceCrossHair(rayHit);
        }

        if (!raycastHitWithinReach) { PlaceCrossHair(rayHit, false); }

        //Debug.DrawRay(pos, rayDir * grabDistance, hitColor, 0.1f);
    }

    private void PlaceCrossHair(RaycastHit hit, bool hitFound = true)
    {
        if (!hitFound) { crossHairObj.transform.position = head.transform.position; return; }

        crossHairObj.transform.position = hit.point;

        //Debug.DrawRay(crossHairObj.transform.position, hit.normal * 2f, Color.red, 0.1f);
    }
    #endregion

    #region Grabbing
    private void HandleGrabbing(bool grabInput, Hand hand)
    {
        // Check inputs
        if (grabInput)
        {
            // Check if not currently grabbing AND if anything is in reach
            if (!hand.isGrabbing && raycastHitWithinReach)
            {
                // Grab it with the hand
                hand.PlaceHand(rayHit.point, rayHit.normal);
                UpdateCurrentClimbCenter();
                OnGrab?.Invoke(hand.grabPos, hand.isLeftHand);
            }
        }
        else
        {
            // Ungrab it
            hand.UnplaceHand();
            UpdateCurrentClimbCenter();
            OnRelease?.Invoke(hand.isLeftHand);
        }
    }

    private void UpdateCurrentClimbCenter()
    {
        switch (leftHand.isGrabbing, rightHand.isGrabbing)
        {
            case (false, false): isClimbing = false; break;
            case (true, false): isClimbing = true; center = leftHand.grabPos; break;
            case (false, true): isClimbing = true; center = rightHand.grabPos; break;
            case (true, true):
                isClimbing = true;
                center = leftHand.grabPos + ((rightHand.grabPos - leftHand.grabPos) / 2f);
                //center = leftHand.grabPos;
                Debug.DrawRay(center, Vector3.up, Color.magenta, 5f);
                break;
        }

        OnClimbUpdate?.Invoke(isClimbing);
    }
    #endregion

}

public class Hand
{
    public bool isGrabbing = false;
    public Vector3 grabPos { get { return graphics.transform.position; } }
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
        isGrabbing = true;
        graphics.SetActive(true);
        graphics.transform.position = placementPos;
        //graphics.transform.up = normal;
        graphics.transform.rotation = CalcRotation(placementPos, normal);

        graphics.transform.localScale = isLeftHand ? new Vector3(1f, 1f, 1f) : new Vector3(-1f, 1f, 1f);
    }

    private Quaternion CalcRotation(Vector3 place, Vector3 normal)
    {
        Vector3 right = Vector3.Cross(normal, Camera.main.transform.forward);
        Vector3 forward = Vector3.Cross(right, normal);

        // Flip rotation if downwards
        if (forward.y < 0f) { forward = new Vector3(forward.x, -forward.y, forward.z); }

        return Quaternion.LookRotation(forward.normalized, normal.normalized);
    }

    public void UnplaceHand()
    {
        isGrabbing = false;
        graphics.SetActive(false);
    }
}
