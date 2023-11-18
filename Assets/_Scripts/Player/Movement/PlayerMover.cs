using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    #region Properties
    [SerializeField] PlayerInputObject inputComponent;
    public Rigidbody rb;

    [Header("Walking")]
    [SerializeField] public float walking_movementSpeed = 4f;
    [Header("Climbing")]
    [SerializeField] public float climbing_horizontalSpeed = 2f;
    [SerializeField] public float climbing_verticalSpeed = 2f;
    [SerializeField] public float velocityLimit = 2f;
    [Tooltip("0 = No damping, 1 = No limitation")]
    [SerializeField, Range(0f, 1f)] public float limitMultiplier = 1f;
    [Header("Falling")]
    [SerializeField] public float falling_movementSpeed = 2f;

    public MoveType currentMoveType = MoveType.walking;

    MovementMode currentMode;

    MovementMode walkingMode;
    MovementMode climbingMode;
    MovementMode fallingMode;

    #endregion

    #region Setup
    private void Start()
    {
        FindObjectOfType<Grabber>().OnClimbUpdate += UpdateClimbingStatus;

        // Fetch references
        rb = GetComponent<Rigidbody>();
        SetupMovementModes();
    }

    private void SetupMovementModes()
    {
        walkingMode = new MovePosition(this);
        //walkingMode = new AddForce(this);
        //climbingMode = new AddForce(this);
        climbingMode = new ClimbingMoveMode(this);

        currentMode = walkingMode;
    }

    private void FixedUpdate()
    {
        HandleMovement(inputComponent.moveValue);
    }
    #endregion

    #region Movement
    private void HandleMovement(Vector2 moveInput)
    {
        if (moveInput == Vector2.zero) { return; }

        currentMode.Move(moveInput);
    }
    #endregion

    #region Handle MovementModes
    public void ChangeMovementMode(MoveType desiredType)
    {
        currentMoveType = desiredType;

        switch (currentMoveType)
        {
            case MoveType.walking:
                currentMode = walkingMode;
                break;
            case MoveType.climbing:
                currentMode = climbingMode;
                break;

            default:
                Debug.Log("This movement type has not been implemented yet!");
                break;
        }
    }

    private void UpdateClimbingStatus(bool isClimbing)
    {
        if (isClimbing) { ChangeMovementMode(MoveType.climbing); }
        else { ChangeMovementMode(MoveType.walking); }
    }
    #endregion

    public bool DetectCollisionInDirection(Vector3 dir)
    {
        float checkDistance = 1f;
        Vector3 pos = transform.position;

        LayerMask grabbingLayerMask = ~LayerMask.GetMask("Player");
        RaycastHit rayHit;
        Vector3 rayDir = dir;
        Ray ray = new Ray(pos, rayDir);
        if (Physics.Raycast(ray, out rayHit, checkDistance, grabbingLayerMask))
        {
            return true;
        }
        else return false;
    }
}