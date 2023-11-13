using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inputs/PlayerInputObject")]
public class PlayerInputObject : ScriptableObject
{
    public Vector2 moveValue = Vector3.zero;
    public Vector2 lookValue = Vector2.zero;

    public bool jumpValue = false;
}