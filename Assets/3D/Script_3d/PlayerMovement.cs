using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float climbSpeed;
    [SerializeField] private float desiredMovementSpeed;
    [SerializeField] public bool climbing;
    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;

    bool readyToJump;
    bool isFalling;


    [SerializeField] private KeyCode jumpKey = KeyCode.Space;


    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    public bool grounded;

    [SerializeField] private Transform orientation;
    [SerializeField] private Climbing ClimbingScript;
    [SerializeField] private Transform respawnPoint;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody playerRb;

    // animation
    [SerializeField] private Animator PlayerAnim;

    // Reference to GameManager
    [SerializeField] private GameManager gameManager;


    private void Start()
    {
        if (!IsOwner) return;
        playerRb = GetComponent<Rigidbody>();
        playerRb.freezeRotation = true;
        PlayerAnim = GetComponent<Animator>();
        readyToJump = true;

    }

    private void Update()
    {
        if (!IsOwner) return;
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        // Respawn if player falls below a certain point
        if (transform.position.y < -5)
        {
            PlayerRespawn();
        }
        // handle drag
        if (grounded)
            playerRb.linearDamping = groundDrag;
        else
            playerRb.linearDamping = 0;


        // Check if player is moving to toggle animation
        CheckMovementAnimation();

        // Handle falling and landing animations
        HandleJumpAndFallAnimations();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        MovePlayer();
        if (climbing)
        {
            desiredMovementSpeed = climbSpeed;
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            playerRb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            playerRb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            playerRb.linearVelocity = new Vector3(limitedVel.x, playerRb.linearVelocity.y, limitedVel.z);
        }
    }
    private void Jump()
    {
        // Set jumping animation before applying jump force
        PlayerAnim.SetBool("IsJumping", true);

        // reset y velocity to prevent adding up forces
        playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);

        // Apply upward force for the jump
        playerRb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Set the respawn point when colliding with an object tagged "Respawn"
        if (other.gameObject.CompareTag("Respawn"))
        {
            respawnPoint = other.transform;
        }

        // Check if player collides with the Goal
        if (other.gameObject.CompareTag("Finish"))
        {
            if (gameManager != null)
            {
                gameManager.Complete();
            }
            else
            {
                Debug.LogWarning("GameManager is not assigned.");
            }
        }
    }

    private void PlayerRespawn()
    {
        // Respawn the player at the last set respawn point
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            Debug.Log("Player respawned to: " + respawnPoint.position);
        }
        else
        {
            Debug.LogWarning("Respawn point not set.");
        }
    }


    private void CheckMovementAnimation()
    {
        if (moveDirection != Vector3.zero)
        {
            PlayerAnim.SetBool("IsMoving", true);
        }
        else
        {
            PlayerAnim.SetBool("IsMoving", false);

        }
    }

    private void HandleJumpAndFallAnimations()
    {
        // Check if the player is grounded
        if (grounded)
        {
            // Reset jumping and falling animations when grounded
            PlayerAnim.SetBool("IsJumping", false);
            PlayerAnim.SetBool("IsFalling", false);
            PlayerAnim.SetBool("IsGrounded", true);

            // Reset falling state when grounded
            isFalling = false;
        }
        else
        {
            // The player is in the air
            PlayerAnim.SetBool("IsGrounded", false);

            // If player is moving upwards (jumping)
            if (playerRb.linearVelocity.y > 0)
            {
                PlayerAnim.SetBool("IsJumping", true);
                PlayerAnim.SetBool("IsFalling", false); // Prevent falling animation during the jump
            }
            // If player is moving downwards (falling)
            else if (playerRb.linearVelocity.y < 0)
            {
                PlayerAnim.SetBool("IsJumping", false); // Stop jumping animation when falling
                PlayerAnim.SetBool("IsFalling", true);  // Enable falling animation
            }
        }

    }



}