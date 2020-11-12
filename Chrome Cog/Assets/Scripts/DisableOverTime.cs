using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOverTime : MonoBehaviour
{
    //This script is just to set game objects to false. Basically make them disappear in the screen like UI text
    public float timeToDisable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeToDisable -= Time.deltaTime;
        if(timeToDisable <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
