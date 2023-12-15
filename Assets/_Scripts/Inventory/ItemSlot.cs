using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField]
    Text text;

    public void UpdateText(string newText)
    {
        text.text = newText;
    }

    public void UpdateSelectedStatus(bool selected)
    {
        if (selected) { text.fontStyle = FontStyle.Bold; }
        else { text.fontStyle = FontStyle.Normal; }
    }
}
