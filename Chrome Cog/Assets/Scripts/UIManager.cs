using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //Make this accessible to all scripts
    public static UIManager instance;

    // Text Mesh Pro variables for Canvas
    public TMP_Text lapCounterText, bestLapTimeText, currentLapTimeText;

    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
