using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory_UI : MonoBehaviour
{
    #region Properties
    [SerializeField]
    GameObject HotbarElement;

    List<ItemSlot> hotbarSlots = new List<ItemSlot>();
    //[Header("Hotbar Properties")]
    //[SerializeField]
    //int sizeOfHotbar = 3;
    #endregion

    #region Setup
    public void SetupHotbarUI(int size)
    {
        int currentChildCount = transform.childCount;

        for (int i = 0; i < size; i++)
        {
            if (i < currentChildCount)
            {
                hotbarSlots.Add(transform.GetChild(i).gameObject.GetComponent<ItemSlot>());
                continue;
            }

            // Else create new one
            hotbarSlots.Add(Instantiate(HotbarElement, transform).GetComponent<ItemSlot>());
        }
    }
    #endregion

    #region Functions
    public void UpdateSelectedSlot(int selectedSlot)
    {
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            if (selectedSlot != i)
            {
                hotbarSlots[i].UpdateSelectedStatus(false);
            }
            else
            {
                hotbarSlots[i].UpdateSelectedStatus(true);
            }
        }
    }

    public void UpdateItemSlot(int selectedSlot, ItemData data)
    {
        // Change name
        string slotText;

        if (data == null) { slotText = "Empty"; }
        else if (data.type == ItemType.hand) { slotText = "Empty"; }
        else { slotText = data.name; }

        hotbarSlots[selectedSlot].UpdateText(slotText);
    }
    #endregion
}