using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/SpringDataObject")]
public class SpringDataObject : ScriptableObject
{
    [Header("Climbing Specs")]
    public float maxDistance = 2f;
    public float minDistance = 0f;
    [Space]
    public float springForce = 20f;
    public float damper = 20f;
    public float massScale = 5f;
    [Space]

    public float breakForce = 1500f;
}