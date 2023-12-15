using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    #region Properties
    [HideInInspector]
    public Grabber grabber;
    [HideInInspector]
    public PlayerLooker looker;
    PlayerInventory inventory;

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
        looker = GetComponent<PlayerLooker>();
        inventory = FindObjectOfType<PlayerInventory>();

        input.leftHandUpdate += CheckHandInput;
        input.rightHandUpdate += CheckHandInput;

        input.rightPickupItemUpdate += CheckPickupDropInput;
        input.rightEquipItemEvent += EquipItem;

        meleeWeaponBehavior = new MeleeWeaponBehavior(this);

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
                right_Item.behavior.Handle_Hold(false);
                input_right_waitingToBeFired = false;
            }
        }
    }
    #endregion

    #region Item Handling

    #region Old?
    public ItemBehavior GetBehavior(ItemData stats)
    {
        // Change behavior
        switch (stats.type)
        {
            case ItemType.hand:
                return handBehavior;
            // ================

            case ItemType.meleeWeapon:
                return meleeWeaponBehavior;
            // ================

            default:
                Debug.Log("Behavior for this type has not been implemented yet!");
                return handBehavior;
        }
    }

    //private ref ItemBehavior GetHand(bool isLeft)
    //{
    //    //if (isLeft) { return ref behavior_Left; }
    //    //else { return ref behavior_Right; }

    //    if (isLeft) { return ref left_Item.behavior; }
    //    else { return ref right_Item.behavior; }
    //}

    private ref ItemInUse GetHand(bool isLeft)
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
        if (GetHand(isLeft).data == defaultHand)
        {
            PickupItem(isLeft);
        }
        else
        {
            // Try to drop item your holding
            //Debug.Log("Dropping item");
            DropItem(isLeft);
        }
    }

    private void PickupItem(bool isLeft)
    {
        LayerMask itemLayerMask = LayerMask.GetMask("Item");

        (Vector3, Vector3) look = looker.GetLookStats();

        Ray ray = new Ray(look.Item1, look.Item2);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, defaultHand.grabDistance, itemLayerMask))
        {
            // Put the item on hand
            ItemData data = hit.collider.gameObject.GetComponent<ItemInWorld>().GetPickedUp();

            GetHand(isLeft).Assign(data, GetBehavior(data));
        }
        else
        {
            Debug.Log("No items found.");
        }

    }

    private void DropItem(bool isLeft)
    {
        Vector3 dropPos = transform.position;

        // Drop the item
        GameObject item = Instantiate(Resources.Load("ItemInWorld"), dropPos, Quaternion.identity, null) as GameObject;
        item.GetComponent<ItemInWorld>().AssignItem(GetHand(isLeft).data);



        // Reassign hand as object
        GetHand(isLeft).Assign(defaultHand, GetBehavior(defaultHand));
    }

    private void EquipItem(bool isLeft)
    {
        // Check if currently holding an item
        if (GetHand(isLeft).data == defaultHand)
        {
            // NOT HOLDING AN ITEM --> Check selected hotbar slot for items

            if (inventory.DoesItemSlotHaveItem())
            {
                // Equip it into the hand
                ItemData wantedItem = inventory.GetItem();
                GetHand(isLeft).Assign(wantedItem, GetBehavior(wantedItem));
            }
            else
            {
                Debug.Log("Nothing to equip.");
            }
        }
        else
        {
            // HOLDING AN ITEM --> Put it into the selected slot

            // ---> Check first if there is an item there
            //      ---> If so, swap them
            if (inventory.DoesItemSlotHaveItem())
            {
                // Swap them
                Debug.Log("Swapping not yet implemented.");
            }
            else
            {
                // Just place it into hotbar
                inventory.UpdateCurrentItemSlot(GetHand(isLeft).data);

                // Remove it from hand
                GetHand(isLeft).Assign(defaultHand, GetBehavior(defaultHand));
            }

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

    public GameObject graphics;

    public void Assign(ItemData _data, ItemBehavior _behavior)
    {
        data = _data;
        behavior = _behavior;

        // Update graphics
        graphics = _data.graphics;
    }
}