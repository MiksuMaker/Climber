using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climber : MonoBehaviour
{
    #region Properties
    Grabber grabber;
    Rigidbody rb;

    [Header("Climbing Specs")]
    [SerializeField] float passivePullStrength = 5f;
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
            PullTowardsCenterOfGravity();
        }
    }

    #region Climbing
    private void PullTowardsCenterOfGravity()
    {
        // Calculate the direction of pull
        Vector3 pullDir = grabber.center - transform.position;
        Debug.DrawRay(transform.position, pullDir, Color.yellow, 2f);

        // Pull the Player towards the center
        rb.MovePosition(transform.position + pullDir * Time.fixedDeltaTime);
    }
    #endregion

    private void OnDrawGizmos()
    {
        // Draw a sphere around
    }
}