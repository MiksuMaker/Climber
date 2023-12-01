using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    #region Properties
    [SerializeField] PlayerInputObject inputComponent;
    [SerializeField] public MoveStatsObject moveStats;
    [HideInInspector]
    public Grabber grabber;
    public Rigidbody rb;

    //[Header("Walking")]
    //[SerializeField] public float walking_movementSpeed = 4f;
    //[Header("Climbing")]
    //[SerializeField] public float climbing_horizontalSpeed = 2f;
    //[SerializeField] public float climbing_verticalSpeed = 2f;
    //[SerializeField] public float velocityLimit = 2f;
    //[Tooltip("0 = No damping, 1 = No limitation")]
    //[SerializeField, Range(0f, 1f)] public float limitMultiplier = 1f;
    //[Header("Falling")]
    //[SerializeField] public float falling_movementSpeed = 2f;

    [SerializeField] Vector3 groundCheckPos = new Vector3(0f, 0.8f);
    [SerializeField] float groundCheckRadius = 0.5f;

    public MoveType currentMoveType = MoveType.walking;

    MovementMode currentMode;

    MovementMode walkingMode;
    MovementMode climbingMode;
    MovementMode fallingMode;

    #endregion

    #region Setup
    private void Start()
    {
        grabber = FindObjectOfType<Grabber>();
        grabber.OnClimbUpdate += UpdateClimbingStatus;

        // Fetch references
        rb = GetComponent<Rigidbody>();
        SetupMovementModes();
    }

    private void SetupMovementModes()
    {
        walkingMode = new MovePosition(this);

        climbingMode = new ClimbingPositionChange(this);

        fallingMode = new FallingMode(this);

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
            case MoveType.falling:
                currentMode = fallingMode;
                break;

            default:
                Debug.Log("This movement type has not been implemented yet!");
                break;
        }
    }

    private void UpdateClimbingStatus(bool isClimbing)
    {
        // Detect if still touching ground
        if (isClimbing)
        {
            ChangeMovementMode(MoveType.climbing);
            currentMode.Enter();
        }
        else
        {
            ChangeMovementMode(MoveType.walking);
            currentMode.Enter();
        }
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

    public bool CheckIfGrounded()
    {
        LayerMask groundMask = ~LayerMask.GetMask("Player");

        Collider[] c = Physics.OverlapSphere(transform.position + groundCheckPos,
                                             groundCheckRadius,
                                             groundMask);

        if (c.Length >= 1)
        {
            // Grounded
            return true;
        }
        else
        {
            // Not grounded
            return false;
        }
    }

    public bool CheckIfOverCOP()
    {
        //float height_modifier = 1f;
        float height_modifier = 0f;

        if (transform.position.y + height_modifier > grabber.center.y)
        {
            // Over
            return true;
        }
        else
        {
            // Under
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 checkPos = transform.position + groundCheckPos;
        Gizmos.color = Color.red;

        if (CheckIfGrounded())
        {
            Gizmos.color = Color.green;
        }

        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
    }
}