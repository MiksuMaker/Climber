using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climber : MonoBehaviour
{
    #region Properties
    Grabber grabber;
    Rigidbody rb;

    Vector3 pullDir = Vector3.zero;
    float distanceOfPull = 0f;

    [Header("Climbing Specs")]
    [SerializeField] float maxDist = 1f;
    [SerializeField] float minDist = 0f;
    [SerializeField] float springForce = 5f;
    [SerializeField] float damper = 5f;
    [SerializeField] float massScale = 1f;
    //float passivePullStrength = 5f;
    //float pullVelocityMultiplier = 0.8f; 

    float passivePullRange = 2f; // Range which within there is no passive pull
    float maxPullRange = 3f;     // Keep the distance

    //[Header("Springs")]
    SpringJoint leftSpring;
    SpringJoint rightSpring;


    #endregion

    #region Setup
    private void Start()
    {
        grabber = GetComponent<Grabber>();
        rb = GetComponent<Rigidbody>();

        // Assign delegates
        grabber.OnGrab += CreateSpring;
        grabber.OnRelease += DestroySpring;
    }
    #endregion

    private void FixedUpdate()
    {
        // Check grab status
        if (grabber.isClimbing)
        {
            //CalculatePullValues();
            //PullTowardsCenterOfGravity();
        }
    }

    #region Climbing V1
    private void CalculatePullValues()
    {
        // Calculate the direction of pull
        pullDir = grabber.center - transform.position;
        Debug.DrawRay(transform.position, pullDir, Color.yellow, 2f);

        // Calculate distance
        distanceOfPull = pullDir.magnitude;

        // Normalize pull direction
        pullDir = pullDir.normalized;
    }

    private void PullTowardsCenterOfGravity()
    {
        // If Player is within passivePullRange, draw them in
        //if (distanceOfPull > passivePullRange && distanceOfPull < maxPullRange)
        //{

        //}
        //else 
        //if (distanceOfPull > maxPullRange)
        //{
        //    // Reel the Player in, keep them inside the maxPullRange
        //    float distanceOverRange = distanceOfPull - maxPullRange;
            
        //    // Move the Player in the pull direction that much
        //    rb.MovePosition(transform.position + pullDir * distanceOverRange * pullVelocityMultiplier);
        //}
        // If player is close enough, do nothing

        // Pull the Player towards the center
        //rb.MovePosition(transform.position + pullDir * passivePullStrength * Time.fixedDeltaTime);
    }
    #endregion

    #region Climbing V2

    #endregion

    #region Springs
    private void CreateSpring(Vector3 anchorPoint, bool isLeftHand)
    {
        // Destroy the previous joint if there is any
        DestroySpring(isLeftHand);

        // Create new spring joint
        SpringJoint joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = anchorPoint;

        joint.maxDistance = maxDist; joint.minDistance = minDist;

        // Spring values
        joint.spring = springForce;
        joint.damper = damper;
        joint.massScale = massScale;

        // Assign as known hand
        if (isLeftHand) { leftSpring = joint; }
        else { rightSpring = joint; }
    }

    private void DestroySpring(bool isLeftHand)
    {
        if (isLeftHand) { if (leftSpring != null) { Destroy(leftSpring); } }
        else { if (rightSpring != null) { Destroy(rightSpring); } }
    }
    #endregion

    private void OnDrawGizmos()
    {
        // Draw a sphere around
        if (grabber != null && grabber.isClimbing)
        {
            
        }
    }
}