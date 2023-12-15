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

    //private void
    #endregion
}