using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{

    public static RaceManager instance;

    public Checkpoint[] allCheckpoints;

    // Stores all maps from using the checkpoints
    public int totalLaps;

    // Player Position
    public CarController playerCar;
    public List<CarController> allAICars = new List<CarController>();
    public int playerPosition;
    public float timeBetweenPosCheck = .2f;
    private float posCheckCounter;

    //Rubber Band System
    public float aiDefaultSpeed = 30f;
    public float playerDefaultSpeed = 30f;
    public float rubberBandSpeedMod = 3.5f;
    public float rubberBandAccel = .5f;

    //Race Time Countdown
    public bool isStarting;
    public float timeBetweenStartCount = 1f;
    private float startCounter;
    public int countDownCurrent = 3;

    //Player Position
    public int playerStartPosition;
    public int aiNumberToSpawn;
    public Transform[] startPoints;

    //Spawning Different Cars
    public List<CarController> carsToSpawn = new List<CarController>();

    //Finishing the race
    public bool raceCompleted;

    //Level Name
    public string raceCompletedScene;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Whenever we start the game
        for(int i = 0; i < allCheckpoints.Length; i++)
        {
            allCheckpoints[i].cpNumber = i;
        }

        //Timer Countdown true value
        isStarting = true;
        startCounter = timeBetweenStartCount;

        UIManager.instance.countDownText.text = countDownCurrent + "!";

        playerStartPosition = Random.Range(0, aiNumberToSpawn + 1);

        playerCar.transform.position = startPoints[playerStartPosition].position;
        playerCar.theRB.transform.position = startPoints[playerStartPosition].position;

        //Spawning Cars at different location
        for(int i = 0; i < aiNumberToSpawn + 1; i++)
        {
            if(i != playerStartPosition)
            {
                int selectedCar = Random.Range(0, carsToSpawn.Count);

                //Create a clone of the object and spawn it
                allAICars.Add(Instantiate(carsToSpawn[selectedCar], startPoints[i].position, startPoints[i].rotation));

                if (carsToSpawn.Count > aiNumberToSpawn - i)
                {
                    carsToSpawn.RemoveAt(selectedCar);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Timer Countdown
        if (isStarting)
        {
            startCounter -= Time.deltaTime;
            if(startCounter <= 0)
            {
                countDownCurrent--;
                startCounter = timeBetweenStartCount;

                UIManager.instance.countDownText.text = countDownCurrent + "!";

                if (countDownCurrent == 0)
                {
                    isStarting = false;

                    UIManager.instance.countDownText.gameObject.SetActive(false);
                    UIManager.instance.goText.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            posCheckCounter -= Time.deltaTime;
            if (posCheckCounter <= 0)
            {


                //Player Position Three different checks; One for Ai ahead of player. Another Player ahead of ai and distance check between player and AI's
                playerPosition = 1;

                foreach (CarController aiCar in allAICars)
                {
                    if (aiCar.currentLap > playerCar.currentLap)
                    {
                        playerPosition++;
                    }
                    else if (aiCar.currentLap == playerCar.currentLap)
                    {
                        if (aiCar.nextCheckpoint > playerCar.nextCheckpoint)
                        {
                            playerPosition++;
                        }
                        else if (aiCar.nextCheckpoint == playerCar.nextCheckpoint)
                        {
                            if (Vector3.Distance(aiCar.transform.position, allCheckpoints[aiCar.nextCheckpoint].transform.position) < Vector3.Distance(playerCar.transform.position, allCheckpoints[aiCar.nextCheckpoint].transform.position))
                            {
                                playerPosition++;
                            }
                        }
                    }
                }
                posCheckCounter = timeBetweenPosCheck;

                UIManager.instance.positionText.text = playerPosition + "/" + (allAICars.Count + 1);
            }

            //Rubber Band Management
            if (playerPosition == 1)
            {
                //Situation 1: Player ahead of ai Cars then ai Cars gets a boost
                foreach (CarController aiCar in allAICars)
                {
                    aiCar.maxSpeed = Mathf.MoveTowards(aiCar.maxSpeed, aiDefaultSpeed + rubberBandSpeedMod, rubberBandAccel * Time.deltaTime);
                }

                //speed gradually moves to player default speed - rubberspeedmod, (how fast we are changing it)
                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed - rubberBandSpeedMod, rubberBandAccel * Time.deltaTime);
            }
            else
            {
                //Situation 2: if Player is behind Ai cars then they will get a certain boost. Basically they will get a boost based on their position in the game.
                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed + (rubberBandSpeedMod * ((float)playerPosition / (allAICars.Count + 1))), rubberBandAccel * Time.deltaTime);
            }
        }
    }

    public void FinishRace()
    {
        raceCompleted = true;

        switch (playerPosition)
        {
            case 1:
                UIManager.instance.raceResultText.text = "You finished 1st";

                break;

            case 2:
                UIManager.instance.raceResultText.text = "You finished 2nd";

                break;

            case 3:
                UIManager.instance.raceResultText.text = "You finished 3rd";

                break;

            default:
                UIManager.instance.raceResultText.text = "You finished " + playerPosition + "th";

                break;
        }

        UIManager.instance.resultsScreen.SetActive(true);
    }

    public void ExitRace()
    {
        SceneManager.LoadScene(raceCompletedScene);
    }
}
