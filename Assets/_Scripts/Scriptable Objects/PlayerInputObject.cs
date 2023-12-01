using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inputs/PlayerInputObject")]
public class PlayerInputObject : ScriptableObject
{
    public delegate void IntegerUpdate(int value);
    public IntegerUpdate sensitivityUpdate;
    public delegate void BoolUpdate(bool value);
    public BoolUpdate leftHandUpdate;
    public BoolUpdate rightHandUpdate;

    public delegate void ButtonEvent();
    public ButtonEvent leftEquipItem;
    public ButtonEvent leftPickupItem;


    public Vector2 moveValue = Vector3.zero;
    public Vector2 lookValue = Vector2.zero;

    public bool jumpValue = false;

    public bool LeftHandInput { get { return leftHandInput; } set { if (value != leftHandInput) { leftHandUpdate?.Invoke(value); } leftHandInput = value; } }
    public bool RightHandInput { get { return rightHandInput; } set { if (value != rightHandInput) { rightHandUpdate?.Invoke(value); } rightHandInput = value; } }

    private bool leftHandInput = false;
    private bool rightHandInput = false;

    public int sensitivityInput { set { sensitivityUpdate?.Invoke(value); } }
}