using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimator : MonoBehaviour
{
    #region Properties
    ItemHandler itemHandler;

    public Transform HandPos_L;
    public Transform HandPos_R;

    [Header("Models")]
    public GameObject itemGraphics_L;
    public GameObject itemGraphics_R;

    [Header("Animations")]
    [SerializeField]
    Animator handAnimator_L;
    [SerializeField]
    Animator handAnimator_R;

    HandAnimation currentAnimationState_L = HandAnimation.idle;
    HandAnimation currentAnimationState_R = HandAnimation.idle;

    // Animation names
    string EMPTY_IDLE = "Hand_Empty_IDLE";
    string EMPTY_WALK = "Hand_Empty_WALK";
    string HOLDING_ITEM = "Hand_Item_IDLE";



    #endregion

    #region Setup
    private void Start()
    {
        itemHandler = GetComponent<ItemHandler>();
    }
    #endregion

    #region Functions
    public void ChangeItemGraphics(bool isLeft, ItemData data)
    {
        // PLACEHOLDER CODE

        // Needs to be changed so that both systems don't use
        // duplicate systems

        if (isLeft)
        {
            if (itemGraphics_L != null) Destroy(itemGraphics_L);


            if (data.type != ItemType.hand)
                itemGraphics_L = Instantiate(data.graphics, HandPos_L.transform.position, HandPos_L.transform.rotation, HandPos_L);
        }
        else
        {
            if (itemGraphics_R != null) Destroy(itemGraphics_R);
            if (data.type != ItemType.hand)
            {
                itemGraphics_R = Instantiate(data.graphics, HandPos_R.transform.position, HandPos_R.transform.rotation, HandPos_R);
                itemGraphics_R.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
    #endregion

    #region Animations
    public void ChangeBothAnimationStates(HandAnimation type)
    {
        ChangeAnimationState(true, type);
        ChangeAnimationState(false, type);
    }

    public void ChangeAnimationState(bool isLeft, HandAnimation type)
    {
        if ((isLeft ? currentAnimationState_L : currentAnimationState_R) == type)
        { return; } // Already playing that animation

        // Check if that hand is holding an item
        bool holdingItem = itemHandler.CheckIfHoldingItem(isLeft);

        string animationName = "";

        switch (holdingItem, type)
        {
            case (_, HandAnimation.idle): animationName = EMPTY_IDLE; break;
            case (_, HandAnimation.walk): animationName = EMPTY_WALK; break;
            //case HandAnimation.idle: animationName = IDLE; break;

            default: Debug.Log("Animation not yet implemented."); break;
        }

        //animationName += (isLeft ? "L" : "R");

        (isLeft ? handAnimator_L : handAnimator_R).Play(animationName);

        //(isLeft ? handAnimator_L : handAnimator_R).CrossFade(animationName, 0.5f);
        if (isLeft) { currentAnimationState_L = type; }
        else { currentAnimationState_R = type; handAnimator_R.playbackTime = 0.5f; }

    }
    #endregion
}

public enum HandAnimation
{
    idle, walk,
    holdingItem, falling
}