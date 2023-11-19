using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inputs/PlayerInputObject")]
public class PlayerInputObject : ScriptableObject
{
    public delegate void IntegerUpdate(int value);
    public IntegerUpdate sensitivityUpdate;
    public delegate void BoolUpdate(bool value);
    public BoolUpdate leftMouseUpdate;
    public BoolUpdate rightMouseUpdate;

    public Vector2 moveValue = Vector3.zero;
    public Vector2 lookValue = Vector2.zero;

    public bool jumpValue = false;

    public bool LeftGrabInput { get { return leftGrabInput; } set { if (value != leftGrabInput) { leftMouseUpdate?.Invoke(value); } leftGrabInput = value; } }
    public bool RightGrabInput { get { return rightGrabInput; } set { if (value != rightGrabInput) { rightMouseUpdate?.Invoke(value); } rightGrabInput = value; } }
    //public bool leftGrabInput = false;
    //public bool rightGrabInput = false;

    private bool leftGrabInput = false;
    private bool rightGrabInput = false;

    public int sensitivityInput { set { sensitivityUpdate?.Invoke(value); } }
}