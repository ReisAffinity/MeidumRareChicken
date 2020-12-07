using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //Make this accessible to all scripts
    public static UIManager instance;

    // Text Mesh Pro variables for Canvas
    public TMP_Text lapCounterText, bestLapTimeText, currentLapTimeText, positionText, countDownText, goText, raceResultText;

    //Pause and Results Screen
    public GameObject resultsScreen;
    public GameObject pauseScreen;

    public bool isPaused;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }

    public void PauseUnpause()
    {
        isPaused = !isPaused;

        pauseScreen.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void ExitRace()
    {
        Time.timeScale = 1f;
        RaceManager.instance.ExitRace();
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}
