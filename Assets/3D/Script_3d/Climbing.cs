using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LayerMask whatisWall;
    [SerializeField] private LayerMask whatisGround;
    [SerializeField] private PlayerMovement pm;

    [Header("Climbing")]
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private float detectionLength = 1f;
    [SerializeField] private float sphereCastRadius = 0.5f;
    [SerializeField] private float maxWallLookingAngle = 60f;

    [Header("Climbing Controls")]
    [SerializeField] private KeyCode climbUpKey = KeyCode.W;
    [SerializeField] private KeyCode climbDownKey = KeyCode.S;

    private bool climbing;
    private RaycastHit frontWallHit;
    private bool wallFront;

    void Update()
    {
        WallCheck();
        HandleClimbing();

        if (climbing)
        {
            ClimbingMovement();
        }
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatisWall);

        float wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);
        if (!wallFront || wallLookAngle > maxWallLookingAngle)
        {
            StopClimbing();
        }
    }

    private void HandleClimbing()
    {
        if (wallFront && (Input.GetKey(climbUpKey) || Input.GetKey(climbDownKey)))
        {
            StartClimbing();
        }
        else
        {
            StopClimbing();
        }
    }

    private void StartClimbing()
    {
        climbing = true;
        pm.climbing = true;
        rb.useGravity = false;
    }

    private void StopClimbing()
    {
        climbing = false;
        pm.climbing = false;
        rb.useGravity = true;
    }

    private void ClimbingMovement()
    {
        float verticalInput = 0f;

        if (Input.GetKey(climbUpKey))
        {
            verticalInput = 1f; // Climb up
        }
        else if (Input.GetKey(climbDownKey))
        {
            verticalInput = -1f; // Climb down
        }

        Vector3 moveDirection = orientation.up * verticalInput;
        rb.linearVelocity = moveDirection * climbSpeed;
    }
}
