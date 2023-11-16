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
    [SerializeField] float passivePullStrength = 5f;
    [SerializeField, Range(0f, 1f)] float pullVelocityMultiplier = 0.8f; 

    [Header("Distances")]
    [SerializeField] float passivePullRange = 2f; // Range which within there is no passive pull
    [SerializeField] float maxPullRange = 3f;     // Keep the distance
    #endregion

    #region Setup
    private void Start()
    {
        grabber = GetComponent<Grabber>();
        rb = GetComponent<Rigidbody>();
    }
    #endregion

    private void FixedUpdate()
    {
        // Check grab status
        if (grabber.isClimbing)
        {
            CalculatePullValues();
            PullTowardsCenterOfGravity();
        }
    }

    #region Climbing
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
        if (distanceOfPull > maxPullRange)
        {
            // Reel the Player in, keep them inside the maxPullRange
            float distanceOverRange = distanceOfPull - maxPullRange;
            
            // Move the Player in the pull direction that much
            rb.MovePosition(transform.position + pullDir * distanceOverRange * pullVelocityMultiplier);
        }
        // If player is close enough, do nothing

        // Pull the Player towards the center
        //rb.MovePosition(transform.position + pullDir * passivePullStrength * Time.fixedDeltaTime);
    }
    #endregion

    private void OnDrawGizmos()
    {
        // Draw a sphere around
        if (grabber != null && grabber.isClimbing)
        {
            float dist = (grabber.center - transform.position).magnitude;

            if (dist < passivePullRange) 
            {
                //Gizmos.Draw
            }
            else if (dist < maxPullRange) 
            {

            }
        }
    }
}