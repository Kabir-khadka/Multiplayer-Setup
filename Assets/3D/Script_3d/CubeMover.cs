using System.Collections;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject cube; // Assign the cube GameObject in the Inspector
    [SerializeField] private float moveDistance; // Distance to move in the X-axis
    [SerializeField] private float minMoveTime; // Minimum time to move in seconds
    [SerializeField] private float maxMoveTime; // Maximum time to move in seconds
    [SerializeField] private float moveSpeed; // Speed to move the cube to the target position
    [SerializeField] private float returnSpeed; // Speed to return the cube to the initial position
    

    private Vector3 initialPosition; // Initial position of the cube

    private void Start()
    {
        // Set the initial position of the cube
        initialPosition = cube.transform.position;
        Debug.Log($"Initial Position: {initialPosition}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the player has a tag "Player"
        {
           
            StartCoroutine(MoveCube());
        }
    }

    private IEnumerator MoveCube()
    {
        // Generate a random duration for the cube movement
        float moveDuration = Random.Range(minMoveTime, maxMoveTime);
        Vector3 targetPosition = initialPosition + new Vector3(moveDistance, 0f, 0f);
        float elapsedTime = 0f;

        // Move the cube to the target position
        while (elapsedTime < moveDuration)
        {
            cube.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime * moveSpeed; // Adjust the speed of movement
            yield return null;
        }
        cube.transform.position = targetPosition; // Ensure final position is set
        Debug.Log($"Cube moved to: {cube.transform.position}");

        // Wait for a short time before starting the return
        yield return new WaitForSeconds(1f);

        // Move the cube back to its initial position
        while (Mathf.Abs(cube.transform.position.x - initialPosition.x) > 0.01f)
        {
            cube.transform.position = Vector3.MoveTowards(cube.transform.position, initialPosition, returnSpeed * Time.deltaTime);
            yield return null;
        }
        cube.transform.position = initialPosition; // Ensure final position is set
        Debug.Log($"Cube returned to: {cube.transform.position}");
    }
}
