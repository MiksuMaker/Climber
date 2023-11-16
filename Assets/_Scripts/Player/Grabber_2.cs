using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber_2 : MonoBehaviour
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

    public Hand leftHand;
    public Hand rightHand;

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
    #endregion

    #region Functions

    #endregion
}