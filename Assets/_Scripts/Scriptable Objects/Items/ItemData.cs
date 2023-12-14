using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item Data/Default")]
public class ItemData : ScriptableObject
{
    [SerializeField]
    public ItemType type;

    //public virtual void Equip(ItemHandler handler, bool forLeftHand) 
    //{
    //    handler.EquipItem(this, forLeftHand);
    //}

    [SerializeField]
    public GameObject graphics;
}

public enum ItemType
{
    hand,
    meleeWeapon,
}