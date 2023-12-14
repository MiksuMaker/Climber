using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inputs/PlayerInputObject")]
public class PlayerInputObject : ScriptableObject
{
    //  Delegates and Events
    public delegate void IntegerUpdate(int value);
    public IntegerUpdate sensitivityUpdate;
    public delegate void BoolUpdate(bool value);
    public delegate void SidedBoolUpdate(bool isLeft, bool value);
    public SidedBoolUpdate leftHandUpdate;
    public SidedBoolUpdate rightHandUpdate;

    public delegate void ButtonEvent();
    public delegate void SidedButtonEvent(bool isLeft);
    public SidedBoolUpdate leftPickupItemUpdate;
    public SidedButtonEvent leftEquipItemEvent;


    public Vector2 moveValue = Vector3.zero;
    public Vector2 lookValue = Vector2.zero;

    public bool jumpValue = false;

    public bool LeftHandInput { get { return leftHandInput; } set { if (value != leftHandInput) { leftHandUpdate?.Invoke(true, value); } leftHandInput = value; } }
    public bool RightHandInput { get { return rightHandInput; } set { if (value != rightHandInput) { rightHandUpdate?.Invoke(false, value); } rightHandInput = value; } }

    private bool leftHandInput = false;
    private bool rightHandInput = false;



    public bool LeftPickupInput { get { return leftPickupInput; } set { if (value != leftPickupInput) { leftPickupItemUpdate?.Invoke(true, value); } leftPickupInput = value; } }
    private bool leftPickupInput = false;

    //public bool LeftEquipInput { get { return leftEquipInput; } set { if (value != leftEquipInput) { leftEquipItemEvent?.Invoke(); } leftEquipInput = value; } }
    public bool LeftEquipInput { get { return leftEquipInput; } set { if (value == true) { leftEquipItemEvent?.Invoke(true); } leftEquipInput = false; } }
    private bool leftEquipInput = false;

    public int sensitivityInput { set { sensitivityUpdate?.Invoke(value); } }
}