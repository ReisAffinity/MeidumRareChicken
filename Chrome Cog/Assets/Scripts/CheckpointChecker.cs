using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointChecker : MonoBehaviour
{

    //Reference to Car
    public CarController theCar;

    //Checkpoint checker when car enters checkpoint area

    private void OnTriggerEnter(Collider other)
    {
        // Need to make sure its an checkpoint area.
        if(other.tag == "Checkpoint")
        {
            // To check if it is hitting the checkpoint
            //Debug.Log("Hit cp " + other.GetComponent<Checkpoint>().cpNumber); 

            theCar.CheckpointHit(other.GetComponent<Checkpoint>().cpNumber);
        }
    }


}
