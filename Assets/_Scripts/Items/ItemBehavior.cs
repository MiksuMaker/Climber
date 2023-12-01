using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior
{
    protected ItemHandler handler;

    public ItemBehavior(ItemHandler handler_)
    {
        handler = handler_;
    }

    public virtual void Handle_Tap(bool isLeft) { }
    public virtual void Handle_Hold(bool isLeft) { }
    public virtual void Handle_Release(bool isLeft) { }

    public virtual void Equip(bool isLeft) { }

    public virtual void Unequip(bool isLeft) { }
}
