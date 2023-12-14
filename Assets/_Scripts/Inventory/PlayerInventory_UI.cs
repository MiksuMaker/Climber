using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory_UI : MonoBehaviour
{
    #region Properties
    [SerializeField]
    GameObject HotbarElement;

    List<GameObject> hotbarSlots = new List<GameObject>();
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
                hotbarSlots.Add(transform.GetChild(i).gameObject);
                continue; 
            }

            // Else create new one
            hotbarSlots.Add(Instantiate(HotbarElement, transform));
        }
    }
    #endregion

    #region Functions

    #endregion
}