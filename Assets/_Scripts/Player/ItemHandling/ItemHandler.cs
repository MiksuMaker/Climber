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

    float left_pressdownTime = 0f;
    float right_pressdownTime = 0f;

    //[Header("Behaviours")]
    ItemBehavior handBehavior;
    ItemBehavior meleeWeaponBehavior;
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

        behavior_Left = handBehavior;
        behavior_Right = handBehavior;
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
                { behavior_Left.Handle_Tap(true); }
                else // Release
                { behavior_Left.Handle_Release(true); }

                input_left_waitingToBeFired = false;
            }
            else
            {
                if (input_right_waitingToBeFired) // Tap
                { behavior_Right.Handle_Tap(false); }
                else // Release
                { behavior_Right.Handle_Release(false); }

                input_right_waitingToBeFired = false;
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
                behavior_Left.Handle_Hold(true);
                input_left_waitingToBeFired = false;
            }
        }
        if (input_right_waitingToBeFired)
        {
            right_pressdownTime += Time.deltaTime;

            if (right_pressdownTime >= itemInputHoldThreshold)
            {
                // Activate hold
                behavior_Right.Handle_Hold(false);
                input_right_waitingToBeFired = false;
            }
        }
    }
    #endregion

    #region Items
    public void EquipItem(ItemObject stats, bool isLeft)
    {
        // Change behavior
        switch (stats.type)
        {
            case ItemType.hand:
                GetHand(isLeft) = handBehavior;
                break;
            // ================

            case ItemType.meleeWeapon:

                break;
                // ================
        }

        // Update stats
        GetHand(isLeft).Equip(isLeft, stats);
    }

    private ref ItemBehavior GetHand(bool isLeft)
    {
        if (isLeft) { return ref behavior_Left; }
        else { return ref behavior_Right; }
    }
    #endregion
}