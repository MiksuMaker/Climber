using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class Grabber : MonoBehaviour
{
    #region Properties
    public delegate void OnHandGrab(Vector3 pos, bool isLeftHand);
    public OnHandGrab OnGrab;
    public delegate void OnHandRelease(bool isLeftHand);
    public OnHandRelease OnRelease;

    [SerializeField]
    PlayerInputObject inputComponent;
    HandAnimator handAnimator;
    [SerializeField] GameObject head;
    [SerializeField] GameObject crossHairObj;

    int grabbingLayerMask;

    [Header("Grabbing")]
    [SerializeField] public HandData handObject;


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

    [HideInInspector]
    public Transform idleHandPos_L { get { return handAnimator.HandPos_L; } }
    [HideInInspector]
    public Transform idleHandPos_R { get { return handAnimator.HandPos_R; } }

    [Header("Hand placement")]
    public float beforeGrabPos_Length = 0.5f;
    public float overGrabPos_Height = 0.5f;
    public float grabbing_Time = 0.1f;
    public float retract_Time = 1f;

    [SerializeField]
    public AnimationCurve grabCurve;
    [SerializeField]
    bool underLedgeGrab_On = false;
    bool legalGrabPos = false;
    #endregion

    #region Setup
    private void Start()
    {
        handAnimator = GetComponent<HandAnimator>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        grabbingLayerMask = ~LayerMask.GetMask("Player");

        leftHand = new Hand(handObject.handGraphics, this);
        rightHand = new Hand(handObject.handGraphics, this, false);

        //inputComponent.leftHandUpdate += UpdateLeftGrabbing;
        //inputComponent.rightHandUpdate += UpdateRightGrabbing;
    }

    private void Update()
    {
        CheckGrabDistance();
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

    public void UpdateHandGrabbing(bool value, bool isLeft)
    {
        HandleGrabbing(value, (isLeft ? leftHand : rightHand));
    }

    #region Aiming
    private void CheckGrabDistance()
    {
        Vector3 pos = head.transform.position;
        raycastHitWithinReach = false;
        Vector3 rayDir = head.transform.forward;
        Ray ray = new Ray(pos, rayDir);
        if (Physics.Raycast(ray, out rayHit, handObject.grabDistance, grabbingLayerMask))
        {

            // Check if the normal is acceptable
            if (underLedgeGrab_On || !(Vector3.Dot(rayHit.normal, Vector3.down) >= 1))
            {
                legalGrabPos = true;
                raycastHitWithinReach = true;
                PlaceCrossHair(rayHit);
            }
            else
            {
                // Too steep --> Can't hold on
                legalGrabPos = false;
            }
        }


        if (!raycastHitWithinReach) { PlaceCrossHair(rayHit, false); }
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
            }
        }
        else
        {
            // Ungrab it
            hand.UnplaceHand();
            UpdateCurrentClimbCenter();
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

    public void ToggleHandVisibility(bool isLeft, bool onOff)
    {
        if (isLeft) { leftHand.ToggleHandVisibility(onOff); }
        else { rightHand.ToggleHandVisibility(onOff); }
    }
    #endregion

}

public class Hand
{
    public bool isGrabbing = false;
    public Vector3 grabPos { get; private set; }
    public Vector3 grabNormal;
    public GameObject graphics = null;
    public bool isLeftHand = true;
    Transform parent;

    // Animation
    Grabber grabber;
    IEnumerator grabAnimator;

    public Hand(GameObject handGraphics, Grabber g, bool isLeft = true)
    {
        isLeftHand = isLeft;
        grabber = g;
        graphics = GameObject.Instantiate(handGraphics);
        graphics.transform.position = (isLeft ? g.idleHandPos_L : g.idleHandPos_R).position;
        graphics.transform.rotation = (isLeft ? g.idleHandPos_L : g.idleHandPos_R).rotation;
        //graphics.transform.parent = g.transform;
        parent = (isLeft ? g.idleHandPos_L : g.idleHandPos_R);
        graphics.transform.parent = parent;
    }

    public void PlaceHand(Vector3 placementPos, Vector3 normal)
    {
        isGrabbing = true;
        graphics.transform.parent = null;

        // V2
        grabPos = placementPos;
        grabNormal = normal;
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
        StartHandMovement(false);

        // If grabbing, destroy the spring
        grabber.OnRelease?.Invoke(isLeftHand);

    }

    public void ToggleHandVisibility(bool onOff)
    {
        if (onOff) { graphics.SetActive(true); }
        else { graphics.SetActive(false); }
    }

    #region Animation
    private void StartHandMovement(bool activeGrab)
    {
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
        Vector3 a = origin;
        Vector3 b = grabPos +
                                (grabber.transform.position - grabPos).normalized
                                * grabber.beforeGrabPos_Length; // Before the grabbing "animation"
        Vector3 c = grabPos
                            + grabNormal * grabber.overGrabPos_Height; // Over the grabPos
        Vector3 d = destination;

        // ROTATION
        Quaternion ogRot = graphics.transform.rotation;
        Quaternion desiredRot = Quaternion.identity;
        if (activeGrab)
        { desiredRot = CalcRotation(grabPos, grabNormal); }
        else
        { desiredRot = (isLeftHand ? grabber.idleHandPos_L : grabber.idleHandPos_R).rotation; }

        float timeTick = 0.01f;
        float timeSpent = 0f;
        float handMoveTime;
        //handMoveTime = activeGrab ? 0.1f : 1f;
        handMoveTime = activeGrab ? grabber.grabbing_Time : grabber.retract_Time;

        float progress, realProgress;
        //bool halfway = false;
        #region WHILE LOOP
        while (timeSpent < handMoveTime)
        {
            progress = (timeSpent / handMoveTime);

            // If retracting hand, calculate the path anew
            if (!activeGrab)
            {
                destination = (isLeftHand ? grabber.idleHandPos_L : grabber.idleHandPos_R).position;
                path = destination - origin;

                desiredRot = (isLeftHand ? grabber.idleHandPos_L : grabber.idleHandPos_R).rotation;
            }

            if (activeGrab)
            {
                // V2, BEZIER CURVE
                graphics.transform.position = Bezier.CalcBezierPos(a, b, c, d, grabber.grabCurve.Evaluate(progress));

                // Rotate
                graphics.transform.rotation = Quaternion.Lerp(ogRot, desiredRot, Easing.EaseOutQuart(progress));
            }
            else
            {
                // Move the hand towards destination
                realProgress = Easing.EaseOutQuart(progress);
                graphics.transform.position = origin + (path * realProgress);

                // Rotate
                graphics.transform.rotation = Quaternion.Lerp(ogRot, desiredRot, realProgress);
            }

            timeSpent += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        #endregion

        // FINALIZE
        if (activeGrab)
        {
            // Finalize placement
            graphics.transform.position = destination;
            graphics.transform.rotation = desiredRot;

            // Create the spring
            grabber.OnGrab?.Invoke(grabPos, isLeftHand);
            Debug.DrawRay(grabPos, grabNormal, Color.red, 0.2f);
        }
        else
        {
            graphics.transform.position = (isLeftHand ? grabber.idleHandPos_L : grabber.idleHandPos_R).position;
            //graphics.transform.parent = grabber.transform;
            graphics.transform.parent = parent;
        }
    }
    #endregion
}
