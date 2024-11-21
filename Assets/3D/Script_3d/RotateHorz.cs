using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHorz : MonoBehaviour
{
    
    [SerializeField] private float rotationSpeed = 30f;
    private Vector3 initalPosition; 
    // Start is called before the first frame update
    void Start()
    {
        initalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
        transform.position = initalPosition;
        
    }
}
