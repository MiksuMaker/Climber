using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climber : MonoBehaviour
{
    #region Properties
    Grabber grabber;
    #endregion

    #region Setup
    private void Start()
    {
        grabber = GetComponent<Grabber>();
    }
    #endregion

    private void FixedUpdate()
    {
        // Check grab status
        
    }

    #region Climbing
    private void UpdateGrabStatus()
    {

    }
    #endregion
}