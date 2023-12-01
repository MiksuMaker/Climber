using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    #region Properties
    [HideInInspector]
    public Grabber grabber;

    [SerializeField] HandObject defaultHand;

    [Header("Hands")]
    [SerializeField] public ItemObject item_Left;
    [SerializeField] public ItemObject item_Right;

    [Header("Behaviors")]
    ItemBehavior behavior_Left;
    ItemBehavior behavior_Right;

    [Header("Inputs")]
    [SerializeField] PlayerInputObject input;
    [SerializeField] float itemInputHoldThreshold = 0.2f;
    bool input_left_waitingToBeFired = false;
    bool input_right_waitingToBeFired = false;

    bool input_left_holding = false;
    bool input_right_holding = false;
    float left_pressdownTime = 0f;
    float right_pressdownTime = 0f;

    //[Header("Behaviours")]
    ItemBehavior handBehavior;
    ItemBehavior swordBehavior;
    #endregion

    #region Setup
    private void Start()
    {
        grabber = GetComponent<Grabber>();

        input.leftHandUpdate += CheckLeftInput;
        input.rightHandUpdate += CheckRightInput;

        SetupBehaviors();

        SetupEquipment();
    }

    private void SetupBehaviors()
    {
        handBehavior = new HandBehavior(this);
    }

    private void SetupEquipment()
    {
        if (item_Left == null) { item_Left = defaultHand; }
        if (item_Right == null) { item_Right = defaultHand; }
    }

    private void Update()
    {
        HandleHoldInputs();
    }
    #endregion

    #region Hands
    private void CheckLeftInput(bool active)
    {
        HandleHandInput(active, true);
    }

    private void CheckRightInput(bool active)
    {
        HandleHandInput(active, false);
    }

    private void HandleHandInput(bool active, bool isLeft)
    {
        if (active)
        {
            // Start countdown on how long it has been active
            if (isLeft) { input_left_waitingToBeFired = true; left_pressdownTime = 0f; }
            else { input_right_waitingToBeFired = true; right_pressdownTime = 0f; }
        }
        else
        {
            // RELEASE
            // -> Check how long the mouse has been held for
            if (isLeft) 
            {
                if (input_left_waitingToBeFired) // Tap
                { item_Left.Handle_Tap(); }
                else // Release
                { item_Left.Handle_Release(); }

                input_left_waitingToBeFired = false;

                //Debug.Log("Time held down: " + left_pressdownTime.ToString("0.00"));
            }
        }
    }

    private void HandleHoldInputs()
    {
        if (input_left_waitingToBeFired)
        {
            left_pressdownTime += Time.deltaTime;

            if (left_pressdownTime >= itemInputHoldThreshold)
            {
                // Activate hold
                item_Left.Handle_Hold();
                input_left_waitingToBeFired = false;
            }
        }

        //if (input_right_holding)
        //{
        //    right_pressdownTime += Time.deltaTime;
        //    if (right_pressdownTime >= itemInputHoldThreshold)
        //    {
        //        // Activate hold
        //        item_Right.Handle_Hold();
        //        input_right_holding = false;
        //    }
        //}
    }
    #endregion

    #region Items

    #endregion
}