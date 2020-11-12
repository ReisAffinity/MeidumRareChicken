using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    // Update is called once per frame
    void Update()
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
        if(playerPosition == 1)
        {
            //Situation 1: Player ahead of ai Cars then ai Cars gets a boost
            foreach(CarController aiCar in allAICars)
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
