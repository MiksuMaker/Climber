using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/MoveStatsObject")]
public class MoveStatsObject : ScriptableObject
{
    [Header("WALKING")]
    public float walking_speed = 4f;
    public float walking_jump_power = 4f;
    [Space]
    [SerializeField] public float walking_velocityLimit_horizontal = 2f;
    [SerializeField, Range(0f, 1f)] public float walk_damper = 1f;

    [Header("CLIMBING")]
    public float climbing_horizontal_towards = 5f;
    public float climbing_horizontal_away = 2f;
    public float climbing_vertical_boost = 10f;
    public float climbing_vertical_over = 5f;

    [Header("Climbing Limits")]
    [SerializeField] public float climbing_velocityLimit_horizontal = 2f;
    [SerializeField] public float climbing_velocityLimit_vertical= 2f;
    [SerializeField, Range(0f, 1f)] public float climb_damper = 1f;

    [Header("Climbing Jumping")]
    public Vector3 climbing_jump_dir = new Vector3(0f, 1f, 0.5f);
    public float climbing_jump_power = 2f;

    [Header("FALLING")]
    public float falling_movementSpeed = 2f;
}