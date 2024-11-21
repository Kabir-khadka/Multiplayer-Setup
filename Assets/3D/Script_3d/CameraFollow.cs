using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The target the camera follows (usually the player)
    [SerializeField] private float distance = 5.0f; // Default distance from the target
    [SerializeField] private float height = 1.0f; // Height above the target
  
    [SerializeField] private float rotationSpeed =  60.0f; // Speed of camera rotation
    [SerializeField] private float verticalRotationLimit = 80.0f; // Limit for vertical rotation
    [SerializeField] private float distanceReductionAngle = 15.0f; // Angle at which distance is reduced
    [SerializeField] private float distanceReductionFactor = 0.5f; // Reduction factor for distance
    [SerializeField] private float distanceSmoothTime = 0.2f; // Time to smooth distance changes

    private float currentRotationAngle; // Stores the current horizontal rotation angle of the camera.
    private float currentVerticalAngle; // tores the current vertical rotation angle of the camera.
    private float currentDistance; // Stores the current distance of the camera from the target.
    private float distanceVelocity = 0.0f; // Smooth time velocity

    private bool isPlayerMoving; // Indicates whether the player is moving based on input.

    void Start()
    {
        if (!target) return;
        currentRotationAngle = transform.eulerAngles.y; // Sets currentRotationAngle to the current Y-axis rotation of the camera.
        currentVerticalAngle = transform.eulerAngles.x; // Sets currentVerticalAngle to the current X-axis rotation of the camera.
        currentDistance = distance; // nitializes currentDistance with the default distance.
    }

    void Update()
    {
        // Sets to true if either horizontal or vertical input is detected (player is moving).
        isPlayerMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
    }

    void LateUpdate()
    {
        if (!target) return; // Exit if there's no target to follow

        
        RotateOnYAxis(); // Adjusts horizontal rotation based on mouse input.
        
        RotateOnXAxis(); //  Adjusts vertical rotation based on mouse input.

        // Calculate the desired camera position
         // Creates a rotation quaternion based on current angles.
        Quaternion currentRotation = Quaternion.Euler(currentVerticalAngle, currentRotationAngle, 0);

        // Sets target distance.
        float targetDistance = distance;
        
        /*
        Mathf.Abs returns the absolute value of a given number. 
        The absolute value is the non-negative value of a number, which means it removes any negative sign.
        example : 
                float negativeValue = -5.0f;
                float positiveValue = Mathf.Abs(negativeValue); // Result: 5.0f
        */
        // If the vertical rotation angle exceeds 15 degrees, the camera's distance will be reduced.
        if (Mathf.Abs(currentVerticalAngle) > distanceReductionAngle)
        {
            targetDistance *= distanceReductionFactor;
        }

        // Smoothly interpolate distance

        /*
        currentDistance: The camera's current distance from the target. float 
        targetDistance: The new desired distance (which may be adjusted based on the vertical angle). flaot
        ref distanceVelocity: Reference to the variable that tracks the smoothing velocity. float
        distanceSmoothTime: Time in seconds over which to smooth the distance change. flaot
        */
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceVelocity, distanceSmoothTime);

        // Calculate the camera position
        Vector3 direction = new Vector3(0, 0, -currentDistance);
        Vector3 targetPosition = target.position + Vector3.up * height;
        transform.position = targetPosition + currentRotation * direction;

        // Make the camera look at the target
        transform.LookAt(target.position + Vector3.up * height);
    }

    private void RotateOnYAxis()
    {
        // Get mouse input for Y-axis rotation (horizontal)
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        currentRotationAngle += mouseX; // Updates the current horizontal rotation angle.
    }

    private void RotateOnXAxis()
    {
        // Get mouse input for X-axis rotation (vertical)
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        currentVerticalAngle -= mouseY; //Updates the current vertical rotation angle (inverted for natural feel).
        //Clamps the angle within the set limits.
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -verticalRotationLimit, verticalRotationLimit);
    }
}
