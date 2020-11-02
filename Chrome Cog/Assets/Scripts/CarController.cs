using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody theRB;

    public float maxSpeed;

    public float forwardAccel = 8f, reverseAccel = 4f;

    private float speedInput;

    public float turnStrength = 180f; 
    private float turnInput;

    private bool grounded;

    public Transform groundRayPoint, groundRayPoint2;
    public LayerMask whatIsGround;
    public float groundRayLength = .75f;

    private float dragOnGround;
    public float gravityMod = 5f;

    public Transform leftFrontWheel, rightFrontWheel;
    public float maxWheelTurn = 25f;

    public ParticleSystem[] dustTrail;
    public float maxEmission = 25f, emissionFadeSpeed = 20f;
    private float emissionRate;

    //Sound Effects
    public AudioSource engineSound;
    public AudioSource skidSound;
    public float skidFadeSpeed;

    //Checkpoint System
    private int nextCheckpoint;
    public int currentLap;

    //Lap time / Best Lap System
    public float lapTime, bestLapTime;

    //AI system
    public bool isAI;

    //point of the track where we went the ai to go to.
    public int currentTarget;
    //Store the position of track
    private Vector3 targetPoint;
    // Point Variance = randomize target point. Point Range = ai target point range
    public float aiAcceleratedSpeed = 1f, aiTurnSpeed = .8f, aiReachPointRange = 5f, aiPointVariance = 3f, aiMaxTurn = 15f;
    private float aiSpeedInput;
    private float aiSpeedMod;

    // Start is called before the first frame update
    void Start()
    {
        theRB.transform.parent = null;

        dragOnGround = theRB.drag;

        if(isAI)
        {
            targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
            RandomizeAITarget();

            aiSpeedMod = Random.Range(.8f, 1.1f);
        }

        UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;

        emissionRate = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        //Every update laptime will be update
        lapTime += Time.deltaTime;


        //AI System
        if (!isAI)
        {

            var ts = System.TimeSpan.FromSeconds(lapTime);
            UIManager.instance.currentLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);

            // Going forward and backward
            speedInput = 0f;
            if (Input.GetAxis("Vertical") > 0)
            {
                speedInput = Input.GetAxis("Vertical") * forwardAccel;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                speedInput = Input.GetAxis("Vertical") * reverseAccel;
            }

            turnInput = Input.GetAxis("Horizontal");

        }
        else
        {   
            //Making the checkpoints for the AI
            targetPoint.y = transform.position.y;

            if(Vector3.Distance(transform.position, targetPoint) < aiReachPointRange)
            {
                SetNextAITarget();
            }
            //Making the AI Move

            Vector3 targetDir = targetPoint - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);

            Vector3 localPos = transform.InverseTransformPoint(targetPoint);
            if(localPos.x < 0f)
            {
                angle = -angle;
            }

            turnInput = Mathf.Clamp(angle / aiMaxTurn, -1f, 1f);

            if(Mathf.Abs(angle) < aiMaxTurn)
            {
                aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, 1f, aiAcceleratedSpeed);
            }
            else
            {
                aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, aiTurnSpeed, aiAcceleratedSpeed);
            }

            speedInput = aiSpeedInput * forwardAccel * aiSpeedMod;
        }

        //Turning the wheels
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);

        //transform.position = theRB.position;

        //Particle Effect Control
        emissionRate = Mathf.MoveTowards(emissionRate, 0f, emissionFadeSpeed * Time.deltaTime);

        if (grounded && (Mathf.Abs(turnInput) > .5f || (theRB.velocity.magnitude < maxSpeed * .5f && theRB.velocity.magnitude != 0)))
        {
            emissionRate = maxEmission;
        }

        if (theRB.velocity.magnitude <= .5f)
        {
            emissionRate = 0;
        }

        for (int i = 0; i < dustTrail.Length; i++)
        {
            var emissionModule = dustTrail[i].emission;

            emissionModule.rateOverTime = emissionRate;
        }

        // Engine Sound
        if(engineSound != null)
        {
            engineSound.pitch = 1f + ((theRB.velocity.magnitude / maxSpeed) * 2f);
        }

        //Skid Sound
        if(skidSound != null)
        {
            if(Mathf.Abs(turnInput) > 0.5f)
            {
                skidSound.volume = 1f;

            }
            else
            {
                skidSound.volume = Mathf.MoveTowards(skidSound.volume, 0f, skidFadeSpeed * Time.deltaTime);
            }
        }
    }
    // This happens every 0.2 seconds. A fixed force
    private void FixedUpdate()
    {
        grounded = false;

        RaycastHit hit;
        Vector3 normalTarget = Vector3.zero;
        
        // These physics raycast will help adjust the car to the ramp.
        if(Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            normalTarget = hit.normal;
        }

        if(Physics.Raycast(groundRayPoint2.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            normalTarget = (normalTarget + hit.normal) / 2f;
        }

        //When on ground rotate to match the normal
        if (grounded)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
        }

        //Accelerates the car
        if (grounded) {

            theRB.drag = dragOnGround;

            theRB.AddForce(transform.forward * speedInput * 1000f);
        }
        else
        {
            theRB.drag = .1f;

            theRB.AddForce(-Vector3.up * gravityMod * 100f); 
        }
       
        
        if(theRB.velocity.magnitude > maxSpeed)
        {
            theRB.velocity = theRB.velocity.normalized * maxSpeed;
        }

        transform.position = theRB.position;

        //Turn left or right of car when flying

        if (grounded && speedInput != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));
        }
    }



    //Checkpoint System Checker
    public void CheckpointHit(int cpNumber)
    {
        //To check if its working
        //Debug.Log(cpNumber);

        //Make sure we gotta the next checkpoint
        if(cpNumber == nextCheckpoint)
        {
            nextCheckpoint++;

            //Once we had hit final checkpoint. Reset back to zero.
            if(nextCheckpoint == RaceManager.instance.allCheckpoints.Length)
            {
                nextCheckpoint = 0;
                //Called this method
                LapCompleted();
            }

        }

        //Stop the ai from doing loops
        if (isAI)
        {
            if(cpNumber == currentTarget)
            {
                SetNextAITarget();
            }
        }
    }

    public void SetNextAITarget()
    {
        currentTarget++;
        if (currentTarget >= RaceManager.instance.allCheckpoints.Length)
        {
            currentTarget = 0;
        }

        targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
        RandomizeAITarget();
    }


    public void LapCompleted()
    {
        //Update cureent lap
        currentLap++;

        // Lap Time / Best Lap Time
        if(lapTime < bestLapTime || bestLapTime == 0)
        {
            bestLapTime = lapTime;
        }

        lapTime = 0f;

        if (!isAI)
        {
            //Once we complete a lap better than best lap then update
            var ts = System.TimeSpan.FromSeconds(bestLapTime);
            UIManager.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);

            // When we add one map then we go to our UI and access the text component. Change it to dispaly current Lap
            UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
        }
    }

    public void RandomizeAITarget()
    {
        targetPoint += new Vector3(Random.Range(-aiPointVariance, aiPointVariance), 0f, Random.Range(-aiPointVariance, aiPointVariance));
    }
}
