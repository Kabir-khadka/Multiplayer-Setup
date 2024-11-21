using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncy : MonoBehaviour
{
    [SerializeField] private float BouncyForce = 10.0f;
    
    // Start is called before the first frame update
    

    private void OnCollisionEnter(Collision collision){
        if(collision.gameObject.CompareTag("Player")){
            
           Rigidbody PlayerRb = collision.gameObject.GetComponent<Rigidbody>();
            if(PlayerRb != null){
                
                PlayerRb.AddForce(transform.up * BouncyForce, ForceMode.Impulse);
               
            }
        }
    }
}
