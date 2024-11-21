using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAway : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float pushForce = 15f; // Force applied to the player when colliding

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Ensure the object collided with is tagged "Player"
        {
            // Get the Rigidbody component of the player
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Calculate the push direction (right direction in world space)
                Vector3 pushDirection = transform.right; // Assuming the right direction is the local right of the cube
                // Apply force to the player to push them in the right direction
                playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);

                // Debugging output
                Debug.Log($"Player pushed in direction: {pushDirection} with force: {pushForce}");
            }
            else
            {
                Debug.LogWarning("Player does not have a Rigidbody component.");
            }
        }
    }
}
