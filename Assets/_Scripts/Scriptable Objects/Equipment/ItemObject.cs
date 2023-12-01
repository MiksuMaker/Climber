using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/EquipmentObject")]
public class ItemObject : ScriptableObject
{
    [SerializeField]
    ItemType type;

    public virtual void Equip() { }
}

public enum ItemType
{
    hand,
    sword,
}