using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemBehavior
{
    protected ItemHandler handler;

    public string name = "Default";

    public ItemBehavior(ItemHandler handler_)
    {
        handler = handler_;
    }

    public virtual void Handle_Tap(bool isLeft) { }
    public virtual void Handle_Hold(bool isLeft) { }
    public virtual void Handle_Release(bool isLeft) { }

    public virtual void Equip(bool isLeft, ItemData stats) { }

    public virtual void Unequip(bool isLeft) { }
}
