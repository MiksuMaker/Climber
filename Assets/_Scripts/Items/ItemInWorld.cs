using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInWorld : MonoBehaviour
{
    #region Properties
    [SerializeField]
    ItemData data;
    #endregion

    #region Setup

    #endregion

    #region Functions
    private void OnTriggerEnter(Collider other)
    {
        // Get collected
    }
    #endregion
}