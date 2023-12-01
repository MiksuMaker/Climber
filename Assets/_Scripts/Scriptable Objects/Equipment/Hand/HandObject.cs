using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/HandObject")]
public class HandObject : ItemObject
{
    [Header("Grabbing")]
    public float grabDistance = 4f;

    [SerializeField]
    public GameObject handGraphics;
    [Space]
    public SpringDataObject spring;

    #region Functions

    #endregion
}