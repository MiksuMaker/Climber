using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inputs/PlayerInputObject")]
public class PlayerInputObject : ScriptableObject
{
    public delegate void IntegerUpdate(int value);
    public IntegerUpdate sensitivityUpdate;

    public Vector2 moveValue = Vector3.zero;
    public Vector2 lookValue = Vector2.zero;

    public bool jumpValue = false;

    public bool leftGrabInput = false;
    public bool rightGrabInput = false;

    public int sensitivityInput { set { sensitivityUpdate?.Invoke(value); } }
}