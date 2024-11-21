using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 6.0f; // Movement speed of the player
    [SerializeField] private float turnSpeed = 60.0f; // Speed at which the player rotates
    [SerializeField] private float gravity = 9.8f; // Gravity force applied to the player
    [SerializeField] private float jumpHeight = 3.0f; // Height of the jump
    [SerializeField] private Camera playerCamera; // Reference to the camera

    private CharacterController characterController; // This component handles movement and collisions.
    private Vector3 moveDirection = Vector3.zero; // to store the direction in which the player is currently moving. Initialized to zero.
    private float verticalVelocity = 0.0f; //  to track the player's vertical velocity, including the effects of gravity and jumping.
    
    void Start()
    {
        //  Initializes the characterController by fetching the CharacterController 
        characterController = GetComponent<CharacterController>();

        // If playerCamera is not assigned in the Inspector, it defaults to the main camera in the scene.
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        ApplyGravity();
        // bug on jump
        Jump();
        Debug.Log(characterController.isGrounded);
    }

    private void HandleMovement()
    {
        // Get input from the player
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isMoving = horizontal != 0 || vertical != 0;

        // Calculate the direction to move based on camera's forward and right vectors
        //This line gets the forward direction vector of the camera.
        Vector3 cameraForward = playerCamera.transform.forward;
        /*is needed to ensure that the player's movement direction is confined to the horizontal plane 
        (i.e., along the X and Z axes) and does not include any vertical component 
        (i.e., along the Y axis).*/
        cameraForward.y = 0; // Ensure movement is only horizontal

        //This line gets the right direction vector of the camera in X axis or horizonatal.  
        Vector3 cameraRight = playerCamera.transform.right;

        // This line calculates the movement direction for the player based on the input from the keyboard and the orientation of the camera.
        Vector3 move = cameraForward * vertical + cameraRight * horizontal;

        //This line normalizes the movement vector to ensure consistent movement speed regardless of direction.
        move = move.normalized; // Normalize to avoid faster diagonal movement

        //: This line checks if the player is currently providing input for movement.
        if (isMoving)
        {
            //This line applies the calculated movement to the player character.
            // Apply movement and move according (Vector3 move) direction
            characterController.Move(move * speed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        // Get input from the player
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isMoving = horizontal != 0 || vertical != 0;

        //: This line checks if the player is currently providing input for movement.
        if (isMoving)
        {
        
            // Calculate the direction to move based on camera's forward and right vectors
            //This line gets the forward direction vector of the camera.
            Vector3 cameraForward = playerCamera.transform.forward;
            /*is needed to ensure that the player's movement direction is confined to the horizontal plane 
            (i.e., along the X and Z axes) and does not include any vertical component 
            (i.e., along the Y axis).*/
            cameraForward.y = 0; // Ensure rotation is only horizontal

            //This line gets the right direction vector of the camera in X axis or horizonatal.  
            Vector3 cameraRight = playerCamera.transform.right;
            // // This line calculates the movement direction for the player based on the input from the keyboard and the orientation of the camera.

            Vector3 moveDirection = cameraForward * vertical + cameraRight * horizontal;
            // //This line normalizes the movement vector to ensure consistent movement speed regardless of direction.
            moveDirection = moveDirection.normalized;
            /*
            LookRotation(Vector3 forward): This method creates a rotation (Quaternion) that points in the direction of the provided forward vector.
             In this case, moveDirection is the direction the player should move based on the input and camera orientation.
            */
            // This line calculates the rotation the player should have based on the direction they need to move.
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            // Smoothly rotate the player towards the target direction
            /*
            Quaternion.Slerp(Quaternion from, Quaternion to, float t): This method performs a spherical linear interpolation between two quaternions. 
            It smoothly interpolates between the from quaternion (current rotation) and the to quaternion (target rotation), based on the interpolation factor t.
            transform.rotation is player rotation
            */
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        // Checks if the player character is not grounded 
        if (!characterController.isGrounded)
        {
            //stores increase gravity 
            verticalVelocity -= gravity * Time.deltaTime;
        }
        else
        {
            // Sets a small negative vertical velocity to ensure the player stays grounded.
            verticalVelocity = -0.1f; // Small value to keep the player grounded
        }

        // Update move direction with the vertical velocity 
        /* sets the vertical component of the movement direction to the current vertical velocity, 
        ensuring that the gravity effect is applied to the player's movement or when player is moving.*/
        moveDirection.y = verticalVelocity;

        // Apply movement
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void Jump()
    {
        // Check for jump input and if the player is grounded
        if (Input.GetButtonDown("Jump") && characterController.isGrounded)
        {
            // Apply jump force
            // Calculates and sets the vertical velocity to make the player jump.
            /*
            verticalVelocity is variable  which set y poistion of player
            jumpHeight * 2f * gravity: This calculation derives from the physics formula for jump velocity.
             To reach a specific height, the required initial velocity can be derived from the formula 
             𝑣=2×𝑔×ℎv= 2×g×h , where 𝑣
             v is the initial jump velocity, 
             𝑔
             g is the acceleration due to gravity, and 
             ℎ
             h is the desired jump height
             
            */
            verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }
    }
}


