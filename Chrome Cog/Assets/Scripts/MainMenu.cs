using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public GameObject raceSetupPanel, trackSelectPanel, racerSelectPanel;

    public Image trackSelectImage, racerSelectImage;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (RaceInfoManager.instance.enteredRace)
        {
            trackSelectImage.sprite = RaceInfoManager.instance.trackSprite;
            racerSelectImage.sprite = RaceInfoManager.instance.racerSprite;

            OpenRaceSetUp();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void StartGame()
    {
        RaceInfoManager.instance.enteredRace = true;
        SceneManager.LoadScene(RaceInfoManager.instance.trackToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    public void OpenRaceSetUp()
    {
        raceSetupPanel.SetActive(true);
    }

    public void CloseRaceSetUp()
    {
        raceSetupPanel.SetActive(false);
    }

    public void OpenTrackSelect()
    {
        trackSelectPanel.SetActive(true);
        CloseRaceSetUp();
    }

    public void CloseTrackSelect()
    {
        trackSelectPanel.SetActive(false);
        OpenRaceSetUp();
    }

    public void OpenRacerSelect()
    {
        racerSelectPanel.SetActive(true);
        CloseRaceSetUp();
    }

    public void CloseRacerSelect()
    {
        racerSelectPanel.SetActive(false);
        OpenRaceSetUp();
    }
}
