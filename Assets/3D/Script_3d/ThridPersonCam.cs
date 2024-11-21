using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThridPersonCam : MonoBehaviour
{
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerObj;

    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private float rotationSpeed;

    
    private void start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
         // Calculate the view direction from the camera to the player
         // calulate roation of camera or  which direction camera is look at 
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        // we need forward value to rotate the player
        orientation.forward = viewDir.normalized;

        // Rotate the player object based on input
        float horizonatalInput = Input.GetAxis("Horizontal");
        float vecticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * vecticalInput + orientation.right * horizonatalInput;

        if(inputDir != Vector3.zero){
            // rotate the player 
            // Vector3.Slerp smooth translation from one vector to anothor vector
            // Vector3.Slerp(Vector3 CurrentVector, Vector3 InputVector, float rotationspeed )
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }
}
