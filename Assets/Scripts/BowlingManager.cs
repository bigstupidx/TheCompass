﻿using UnityEngine;
using System.Collections;
/*
* Script to handle bowling minigame
* Game consists of 10 frames, 2 rounds per frame
* Hitting all pins on first throw is a strike (10pts + next 2 throws)
* Hitting all pins on second throw is a spare (10pts + next throw)
* If a strike is bowled on 10th round, player gets 2 additional throws
* If a spare is bowled on 10th round, player gets 1 additional throw
*/

public class BowlingManager : MonoBehaviour {

    GameObject explosion;//< spawn effect for ball dispenser

    ///TEXT OBJECTS
    public TextBoxManager theTextBox;
    public TextAsset textFile;
    public string speakerName;
    public int startLine;
    public int endLine;
    private bool timedDialogue = true;
    private float timeUntilFinished = 10f;

    //event declaration for spawning pins
    public delegate void SpawnAction(GameObject obj);
    public static event SpawnAction SpawnPin;

    //event declaration for cleaning up pins
    public delegate void CleanAction();
    public static event CleanAction DestroyPin;

    public Transform ballSpawner;
    public GameObject pinObject;
    public GameObject ballObject;

    //final score of the player so far
    int finalScore = 0;

    //the amount the player earns during each frame (2 throws)
    //10 frames
    int[] frameScore = new int[10] {0,0,0,0,0,0,0,0,0,0};

    //the amount earned in each round
    int[] round1Score = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    int[] round2Score= new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

    //if a particular frame was a strike
    //bool[] frameStrike = new bool[12] { false, false, false, false, false, false, false, false, false, false, false, false };



    //checks if this frame is on its second round
    bool round2 = false;

    //current frame (round)
    int currentFrame = 0;
    const int MAX_FRAMES = 10;
    //number of bonus throws for the player (increased if a strike on 10th round)
    int bonusThrows = 0;

    //subscribe BallDrop function to LostBall Event
    //When a ball hits a gutter, calls BallDrop() to bring it back
    void OnEnable()
    {Gutter.LostBall += BallDrop; Gutter.HitPin += IncreaseScore;}
    void OnDisable()
    {Gutter.LostBall -= BallDrop; Gutter.HitPin += IncreaseScore;}

    // Use this for initialization
    void Start()
    {
        explosion = Resources.Load("Explosion") as GameObject;
        // find the text box
        theTextBox = FindObjectOfType<TextBoxManager>();
        // show intro text
       
        // show intro text
        ShowText();
        //start first frame
        StartFrame();
    }



    //runs at the beginning of each frame, resets pins and points counter for frame
    void StartFrame()
    {
        //indicate that this frame is on its first round
        round2 = false;

        //if all 10 frames have been finished
        if(currentFrame > MAX_FRAMES)
        {
            //if bonus rounds are available
            if (bonusThrows > 0)
            {
                //handle bonus rounds
                StartFrame();
            }
            else //if there are no bonus rounds
            {
                //end the game
                EndGame();
            }         
        }
        else
        {

            Debug.Log("Frame "+ (currentFrame + 1)+" begin!");

            //spawn the ball
            StartCoroutine(RespawnBall());
            //spawn the pins
            StartCoroutine(RespawnPins());
           
        }
    }
    //start second round of current frame
    void Round2()
    {
        //indicate that this frame has started its second round
        round2 = true;

        Debug.Log("Frame " + (currentFrame+1) + " round 2!");
       
        //spawn the ball
        StartCoroutine(RespawnBall());
        
    }


    //Handles all functions for ending a frame
    void EndFrame()
    {
        //calculate final score so far
        ScoreHandler();

        //if anything is subscribed to DestroyPin
        //clean up remaining pins by calling DestroyPin Event
        if (DestroyPin != null)
        { DestroyPin();}
            
        //Advance counter to next frame
        currentFrame++;
        //reduce bonus throws if any
        if(bonusThrows > 0)
        {
            bonusThrows--;
        }
        //start next frame
        StartFrame();
    }

    
    //handles proper behavior when a ball is destroyed:
    //Gutterball or ball lands in back pit
    void BallDrop()
    {
        //End the round
        StartCoroutine(EndRound());
    }
    //Handles the end of a round
    //checks whether to continue to round 2 or end frame
    IEnumerator EndRound()
    {
        //wait for remaining pins to fall
        yield return new WaitForSeconds(3f);

        //if this is round one or a bonus round
        if(!round2 || bonusThrows > 0)
        {
            //if this was a bonus throw
            if(bonusThrows > 0)
            {
                //add to the final frame
                frameScore[9] += round1Score[currentFrame];
            }
            else
            {
                //add the score for round one of this frame to the total frame score
                frameScore[currentFrame] += round1Score[currentFrame];
            }
           
            //output number of pins hit
            Debug.Log("You hit " + round1Score[currentFrame] + " pins!");
            //if the total is at 10, its a STRIKE!
            if (frameScore[currentFrame] == 10)
            {
                //if this is the 10th (final) frame
                if (currentFrame == 9)
                {
                    bonusThrows += 2;
                }
                //End this frame, there are no remaining pins
                EndFrame();
            }
            else
            {
                if(bonusThrows > 0)
                {
                    //End this frame, bonus rounds only give you one throw
                    EndFrame();
                }
                else
                {
                    //start round 2
                    Round2();
                }
                
            }
        }
        else
        {
            //add the score for round two of this frame to the total frame score
            //round 1 will already have been added
            frameScore[currentFrame] += round2Score[currentFrame];
            //output number of pins hit
            Debug.Log("You hit " + round2Score[currentFrame] + " pins!");
            //if the total is at 10, its a spare
            if (frameScore[currentFrame] == 10)
            {
                Debug.Log("SPARE!");
                //if this is the 10th (final) frame
                if (currentFrame == 9)
                {
                    bonusThrows += 1;
                }
            }
            //End this frame
            EndFrame();

        }
       

    }

      
    void ScoreHandler()
    {
        //CALCULATE FINAL SCORING

        //check for strike 2 frames ago (if this isnt the first or second frame)
        if (currentFrame > 1 && round1Score[currentFrame - 2] == 10)
        {
            frameScore[currentFrame - 2] += frameScore[currentFrame];
        }

        //check for strike or spare last frame (if the last frame had a total score of 10)
        //(if this isnt the first frame or 2nd bonus round)
        if (currentFrame > 0 && currentFrame < 11 && frameScore[currentFrame-1] == 10)
        {
            frameScore[currentFrame - 1] += frameScore[currentFrame];     
        }

        //if this frame was a strike
        if(!round2)
        {
            //start at 1 since a strike just occurred
            int strikeCounter = 1;
            //count back from the current frame to see how many strikes occured in sequence
            for(int i = currentFrame-1; i >= 0; i-- )
            {
                //if the frame being checked was a strike
                if(round1Score[i] == 10)
                {
                    //increase the strike counter
                    strikeCounter++;
                }
                else
                {break;}
            }

            //output the proper strike term based on streak
            switch(strikeCounter)
            {
                case 1:
                    Debug.Log("Strike!!!");
                    break;
                case 2:
                    Debug.Log("Double!!!");
                    break;
                case 3:
                    Debug.Log("Turkey!!!");
                    break;
                case 6:
                    Debug.Log("Wild Turkey!!!");
                    break;
                case 9:
                    Debug.Log("Gold Turkey!!!");
                    break;
                case 12:
                    Debug.Log("PERFECT GAME!!!");
                    break;
                default:
                    Debug.Log("strike counter is at: "+strikeCounter);
                    break;
            }

           
        }

        //DEBUG CODE!!!
        //calculate current final score
        //add all scores together
        finalScore = 0;
        for (int i = 0; i < MAX_FRAMES; i++)
        {
            //if this is the last frame
            if (i == MAX_FRAMES - 1)
            {
                //tack on the last 2 round1 scores (amount for bonus rounds)
                Debug.Log("Frame " + (i + 1) + " rounds: "
                    + round1Score[i] + "/" + round2Score[i] + "/" +round1Score[10] + "/" + round1Score[11]
                    + ". Total Frame score: " + frameScore[i]);
                finalScore += frameScore[i];
            }
            else
            {
                Debug.Log("Frame " + (i + 1) + " rounds: "
                    + round1Score[i] + "/" + round2Score[i] + ". Total Frame score: " + frameScore[i]);
                finalScore += frameScore[i];
            }
        }
        Debug.Log("Score for this frame: "+frameScore[currentFrame]);
        Debug.Log("Current total score: "+finalScore);
    }

    //increases the player score
    //adds points to the proper round at the index for the frame
    void IncreaseScore()
    {
        //increase the round score by one
        if (!round2)
        {
            round1Score[currentFrame]++;
        }
        else
        {
            round2Score[currentFrame]++;
        }   
    }


    IEnumerator RespawnBall()
    {
        yield return new WaitForSeconds(2f);
        //if the ball object has been assigned
        if (ballObject != null)
        {
            Instantiate(explosion, ballSpawner.position, Quaternion.identity);
            //instantiate ball
            //GameObject ball = Instantiate(ballObject, ballSpawner.position, ballSpawner.rotation) as GameObject;
            Instantiate(ballObject, ballSpawner.position, ballSpawner.rotation);
        }
        yield return null;
    }

    IEnumerator RespawnPins()
    {
        //wait 2 seconds
        yield return new WaitForSeconds(2f);
        //if anything is subscribed to SpawnPins
        if (SpawnPin != null)
        {
            //spawn pins using the pin object
            SpawnPin(pinObject);
        }
        yield return null;
    }

   
    void EndGame()
    {
        Debug.Log("GAME COMPLETE");
        //calculate final score
        //add all scores together
        for (int i = 0; i <= MAX_FRAMES; i++)
        {
            Debug.Log("Frame 1: "+ frameScore[i]);
            finalScore += frameScore[i];
        }

        //display score
        Debug.Log("Final Score: " + finalScore);
        //save score?

        //retry or exit
    }

    void ShowText()
    {
        if (Time.timeScale != 0)
        {
           
                theTextBox.startCommentaryDialogue();
                //theTextBox.setVoiceOverSourceClip(audioClip);
                theTextBox.ReloadScript(textFile);
                theTextBox.currentLine = startLine;
                theTextBox.endAtLine = endLine;
                //theTextBox.EnableTextBox();
                theTextBox.setSpeakerNameText(speakerName);

                if (timedDialogue)
                {
                    theTextBox.activateTimedCommentary(timeUntilFinished);
                }
            
        }
    }
}
