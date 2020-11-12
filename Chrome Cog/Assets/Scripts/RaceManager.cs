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

    }
}
