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

    [Header("Hand placement")]
    //[SerializeField] public Vector3 idleHandPos;
    //[SerializeField] public float sidePos = 1f;

    [SerializeField] public Transform idleHandPos_L;
    [SerializeField] public Transform idleHandPos_R;

    public float beforeGrabPos_Length = 0.5f;
    public float overGrabPos_Height = 0.5f;
    public float grabbing_Time = 0.1f;

    #endregion

    #region Setup
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        grabbingLayerMask = ~LayerMask.GetMask("Player");

        leftHand = new Hand(handGraphics, this);
        rightHand = new Hand(handGraphics, this, false);

        inputComponent.leftMouseUpdate += UpdateLeftGrabbing;
        inputComponent.rightMouseUpdate += UpdateRightGrabbing;
    }

    private void Update()
    {
        CheckGrabDistance();
        ////HandleGrabbing(inputComponent.leftGrabInput, leftHand);
        ////HandleGrabbing(inputComponent.rightGrabInput, rightHand);
    }
    #endregion

    private void UpdateLeftGrabbing(bool value)
    {
        HandleGrabbing(value, leftHand);
    }
    private void UpdateRightGrabbing(bool value)
    {
        HandleGrabbing(value, rightHand);
    }

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

    #region Animation
    public void StartHandAnimation(IEnumerator coroutine, ref IEnumerator current)
    {
        if (current != null)
        {
            StopCoroutine(current);
            current = null;
        }
        current = coroutine;
        StartCoroutine(current);
    }
    #endregion

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position + idleHandPos, 0.2f);
    }
}

public class Hand
{
    public bool isGrabbing = false;
    //public Vector3 grabPos { get { return graphics.transform.position; } }
    public Vector3 grabPos { get; private set; }
    public Vector3 grabNormal;
    public GameObject graphics = null;
    public bool isLeftHand = true;

    // Animation
    Grabber grabber;
    IEnumerator grabAnimator;

    public Hand(GameObject handGraphics, Grabber g, bool isLeft = true)
    {
        isLeftHand = isLeft;
        grabber = g;
        graphics = GameObject.Instantiate(handGraphics);
        graphics.SetActive(false);
    }

    public void PlaceHand(Vector3 placementPos, Vector3 normal)
    {
        isGrabbing = true;
        graphics.SetActive(true);
        graphics.transform.parent = null;

        // V1
        //graphics.transform.position = placementPos;
        //graphics.transform.rotation = CalcRotation(placementPos, normal);

        // V2
        grabPos = placementPos;
        grabNormal = normal;
        //graphics.transform.position = placementPos;
        //graphics.transform.rotation = CalcRotation(placementPos, normal);
        StartHandMovement(true);

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
        //graphics.SetActive(false);
        StartHandMovement(false);
    }

    #region Animation
    private void StartHandMovement(bool activeGrab)
    {
        //Debug.Log(activeGrab);
        grabber.StartHandAnimation(GrabAnimator(activeGrab), ref grabAnimator);
    }

    IEnumerator GrabAnimator(bool activeGrab)
    {
        if (!graphics.activeSelf) { graphics.SetActive(true); }

        // Get hand graphics position
        Vector3 origin = graphics.transform.position;

        Vector3 destination = Vector3.zero;
        if (activeGrab)
        { destination = grabPos; } // Hand on the wall
        else
        { destination = grabber.transform.position; } // Return hand next to body
        Vector3 path = (destination - origin);

        // BEZIER CURVE
        //Vector3 b = grabPos + grabNormal * 2f;
        Vector3 a = origin;
        //Vector3 b = grabPos + (grabber.transform.position - grabPos).normalized * 0.3f; // Before the grabbing "animation"
        Vector3 b = grabPos + (grabber.transform.position - grabPos).normalized * grabber.beforeGrabPos_Length; // Before the grabbing "animation"
        Vector3 c = grabPos 
                            //+ (grabPos - grabber.transform.position).normalized 
                            //+ grabNormal * 0.5f; // Over the grabPos
                            + grabNormal * grabber.overGrabPos_Height; // Over the grabPos
        Vector3 d = destination;

        // ROTATION
        Quaternion ogRot = graphics.transform.rotation;
        Quaternion desiredRot = Quaternion.identity;
        if (activeGrab)
        { desiredRot = CalcRotation(grabPos, grabNormal); }
        else
        { desiredRot = (isLeftHand ? grabber.idleHandPos_L : grabber.idleHandPos_R).rotation; }


        float timeSpent = 0f;
        float handMoveTime;
        //handMoveTime = activeGrab ? 0.1f : 1f;
        handMoveTime = activeGrab ? grabber.grabbing_Time : 1f;

        #region WHILE LOOP
        while (timeSpent < handMoveTime)
        {
            float progress = (timeSpent / handMoveTime);

            // If retracting hand, calculate the path anew
            if (!activeGrab)
            {
                //destination = grabber.transform.position + (isLeftHand ? grabber.idleHandPos_L : grabber.idleHandPos_R).localPosition;
                destination = (isLeftHand ? grabber.idleHandPos_L : grabber.idleHandPos_R).position;
                path = destination - origin;

                desiredRot = (isLeftHand ? grabber.idleHandPos_L : grabber.idleHandPos_R).rotation;
            }

            if (activeGrab)
            {
                // V2, BEZIER CURVE
                graphics.transform.position = Bezier.CalcBezierPos(a, b, c, d, Easing.EaseOutQuart(progress));
            }
            else
            {
                // V1
                // Move the hand towards destination
                graphics.transform.position = origin + (path * Easing.EaseOutQuart(progress));
            }

            // Rotate
            graphics.transform.rotation = Quaternion.Lerp(ogRot, desiredRot, Easing.EaseOutQuart(progress));

            timeSpent += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        #endregion

        // FINALIZE
        if (activeGrab)
        {
            // Finalize placement
            graphics.transform.position = destination;
            graphics.transform.rotation = desiredRot;
        }
        else
        {
            graphics.transform.position = (isLeftHand ? grabber.idleHandPos_L : grabber.idleHandPos_R).position;
            graphics.transform.parent = grabber.transform;
            //graphics.SetActive(false);
        }
    }
    #endregion
}
