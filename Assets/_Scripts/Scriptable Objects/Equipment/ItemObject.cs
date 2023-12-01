using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/EquipmentObject")]
public class ItemObject : ScriptableObject
{
    [SerializeField]
    public ItemType type;

    public virtual void Equip(ItemHandler handler, bool forLeftHand) 
    {
        handler.EquipItem(this, forLeftHand);
    }
}

public enum ItemType
{
    hand,
    meleeWeapon,
}