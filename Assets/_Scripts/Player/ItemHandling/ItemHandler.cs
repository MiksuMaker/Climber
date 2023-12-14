using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    #region Properties
    [HideInInspector]
    public Grabber grabber;

    [SerializeField] HandData defaultHand;

    [Header("Items in Use")]
    [SerializeField] public ItemInUse left_Item;
    [SerializeField] public ItemInUse right_Item;

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

        input.leftHandUpdate += CheckHandInput;
        input.rightHandUpdate += CheckHandInput;

        input.leftPickupItemUpdate += CheckPickupDropInput;
        input.leftEquipItemEvent += EquipItem;

        SetupInitialItems();
    }

    private void SetupInitialItems()
    {
        left_Item = new ItemInUse();
        right_Item = new ItemInUse();

        handBehavior = new HandBehavior(this);

        //behavior_Left = handBehavior;
        //behavior_Right = handBehavior;

        left_Item.Assign(defaultHand, handBehavior);
        right_Item.Assign(defaultHand, handBehavior);
    }

    private void Update()
    {
        HandleHoldInputs();
    }
    #endregion

    #region Hands

    private void CheckHandInput(bool isLeft, bool active)
    {
        HandleHandInput(active, isLeft);
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
                { left_Item.behavior.Handle_Tap(true); }
                else // Release
                { left_Item.behavior.Handle_Release(true); }

                input_left_waitingToBeFired = false;
            }
            else
            {
                if (input_right_waitingToBeFired) // Tap
                { right_Item.behavior.Handle_Tap(false); }
                else // Release
                { right_Item.behavior.Handle_Release(false); }

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
                left_Item.behavior.Handle_Hold(true);
                input_left_waitingToBeFired = false;
            }
        }
        if (input_right_waitingToBeFired)
        {
            right_pressdownTime += Time.deltaTime;

            if (right_pressdownTime >= itemInputHoldThreshold)
            {
                // Activate hold
                left_Item.behavior.Handle_Hold(false);
                input_right_waitingToBeFired = false;
            }
        }
    }
    #endregion

    #region Item Handling

    #region Old?
    public void EquipItem(ItemData stats, bool isLeft)
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
        //if (isLeft) { return ref behavior_Left; }
        //else { return ref behavior_Right; }

        if (isLeft) { return ref left_Item.behavior; }
        else { return ref right_Item.behavior; }
    }

    private ref ItemInUse GetItem(bool isLeft)
    {
        if (isLeft) { return ref left_Item; }
        else { return ref right_Item; }
    }
    #endregion

    private void CheckPickupDropInput(bool isLeft, bool value)
    {
        // For now, just check the input as bool, check HOLDING status later

        if (value == true)
        {
            PickupDropItem(isLeft);
        }
    }

    private void PickupDropItem(bool isLeft)
    {
        // Check if currently holding an item
        if (GetItem(isLeft).data == defaultHand)
        {
            PickupItem(isLeft);
        }
        else
        {
            // Try to drop item your holding
            Debug.Log("Dropping item");
        }
    }

    private void PickupItem(bool isLeft)
    {
        // Check if looking at an item in world
        if (true)
        {
            Debug.Log("Picking up item");
        }
        else
        {
            Debug.Log("Nothing to pickup.");
        }
    }


    private void EquipItem(bool isLeft)
    {
        // Check if currently holding an item
        if (GetItem(isLeft).data == defaultHand)
        {
            Debug.Log("Nothing to equip.");
        }
        else
        {
            Debug.Log("Equipping " + GetItem(isLeft).data.name);
        }
    }

    #endregion
}

[System.Serializable]
public class ItemInUse
{
    public ItemData data;

    // Condition

    // Behavior
    public ItemBehavior behavior;

    public void Assign(ItemData _data, ItemBehavior _behavior)
    {
        data = _data;
        behavior = _behavior;
    }
}