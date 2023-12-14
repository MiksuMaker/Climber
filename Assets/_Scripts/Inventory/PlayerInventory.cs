using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    #region Properties
    GameObject playerGO;
    ItemHandler itemHandler;

    PlayerInventory_UI ui;

    [Header("Hotbar")]
    [SerializeField] int hotBarSize = 3;
    #endregion

    #region Setup
    private void Start()
    {
        itemHandler = FindObjectOfType<ItemHandler>();
        playerGO = itemHandler.gameObject;

        ui = FindObjectOfType<PlayerInventory_UI>();

        SetupHotbar();
    }

    private void SetupHotbar()
    {
        ui.SetupHotbarUI(hotBarSize);
    }
    #endregion

    #region Item Handling


    #endregion
}