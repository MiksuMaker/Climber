using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBehavior : ItemBehavior
{
    public HandBehavior(ItemHandler handler) : base(handler) { }

    public override void Handle_Tap(bool isLeft)
    {
        Debug.Log("Tap");
    }

    public override void Handle_Hold(bool isLeft)
    {
        //Debug.Log("Hold");

        handler.grabber.UpdateHandGrabbing(true, isLeft);
    }

    public override void Handle_Release(bool isLeft)
    {
        //Debug.Log("Release");

        handler.grabber.UpdateHandGrabbing(false, isLeft);
    }
}
