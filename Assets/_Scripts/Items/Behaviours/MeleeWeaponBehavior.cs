using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponBehavior : ItemBehavior
{
    public MeleeWeaponBehavior(ItemHandler handler_) : base(handler_) { name = "Sword"; }

    public override void Handle_Tap(bool isLeft) 
    {
        Debug.Log("Weapon Tap");
    }

    public override void Handle_Hold(bool isLeft) 
    {

        Debug.Log("Weapon Hold");
    }

    public override void Handle_Release(bool isLeft) 
    {
        Debug.Log("Weapon Release");

    }

}