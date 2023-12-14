using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInWorld : MonoBehaviour
{
    #region Properties
    [SerializeField]
    //[OnChangedCall("UpdateItem")]
    ItemData data;
    #endregion

    #region Setup
    
    #endregion

    #region Functions
    public ItemData GetPickedUp()
    {
        // Destroy for now
        Destroy(gameObject);

        return data;
    }
    #endregion

    [ContextMenu("Update Item Looks")]
    public void UpdateItem()
    {
        if (data == null) return;

        if (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
            //Destroy(transform.GetChild(0).gameObject);
        }

        Instantiate(data.graphics, this.transform);

        gameObject.name = data.name;
    }
}