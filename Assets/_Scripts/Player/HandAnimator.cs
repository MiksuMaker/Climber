using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimator : MonoBehaviour
{
    #region Properties
    public Transform HandPos_L;
    public Transform HandPos_R;

    [Header("Models")]
    public GameObject itemGraphics_L;
    public GameObject itemGraphics_R;

    [Header("Animations")]
    [SerializeField]
    Animator handAnimator_R;
    #endregion

    #region Setup

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
}