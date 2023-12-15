using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    #region Properties
    GameObject playerGO;
    ItemHandler itemHandler;
    [SerializeField]
    PlayerInputObject input;

    PlayerInventory_UI ui;

    [Header("Hotbar")]
    [SerializeField] int hotBarSize = 3;
    [SerializeField] int currentHotbarSlot = 0;
    //List<ItemData> itemsInHotbar = new List<ItemData>();
    ItemData[] itemsInHotbar;
    #endregion

    #region Setup
    private void Start()
    {
        itemHandler = FindObjectOfType<ItemHandler>();
        playerGO = itemHandler.gameObject;

        ui = FindObjectOfType<PlayerInventory_UI>();

        input.mouseWheelUpdate += UpdateHotbarSelection;

        SetupHotbar();
    }

    private void SetupHotbar()
    {
        ui.SetupHotbarUI(hotBarSize);

        itemsInHotbar = new ItemData[hotBarSize];
    }
    #endregion

    #region Inputs
    private void UpdateHotbarSelection(int change)
    {
        currentHotbarSlot += change;

        if (currentHotbarSlot < 0) { currentHotbarSlot = hotBarSize - 1; }
        else if (currentHotbarSlot >= hotBarSize) { currentHotbarSlot = 0; }

        ui.UpdateSelectedSlot(currentHotbarSlot);
    }

    public void UpdateCurrentItemSlot(ItemData data)
    {
        // Update the actual item
        itemsInHotbar[currentHotbarSlot] = data;

        // Update UI
        ui.UpdateItemSlot(currentHotbarSlot, data);
    }

    public bool DoesItemSlotHaveItem()
    {
        if (itemsInHotbar[currentHotbarSlot] != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public ItemData GetItem(bool andRemoveFromInventory = true)
    {
        ItemData wantedItem = itemsInHotbar[currentHotbarSlot];

        if (andRemoveFromInventory)
        {
            itemsInHotbar[currentHotbarSlot] = null;

            // Update UI
            UpdateCurrentItemSlot(null);
        }

        return wantedItem;
    }
    #endregion
}