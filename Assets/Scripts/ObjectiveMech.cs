﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ObjectiveMech : MonoBehaviour {

    //public GameObject transitionTextBox;

    //Put the name of the scene to jump to here
    public string nextLevelName;
    public LoadingTransition loadingTransition;
    public GameObject transitionBox;
	public GameObject commentaryObject;
	public bool isCommentaryTrigger;
    void Start()
    {
        loadingTransition = transitionBox.GetComponent<LoadingTransition>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            
               
            
           
            // win condition met if part collected
            if (col.GetComponent<TractorBeamControls>().partCollected)
            {
                
                loadingTransition.startCommentaryDialogue();
               

                /*
                Debug.Log("COMPLETE,COMPLETE,COMPLETE");
                if (nextLevelName != null)
                {
                    SceneManager.LoadSceneAsync(nextLevelName);
                }
                else
                {
                    Debug.Log("No scene set for next level on waypoint");
                }
                */
                

            }
       
        }
        else if(col.tag == "TetheredPart")
        {
			if (isCommentaryTrigger == true) 
			{
				if (commentaryObject != null) 
				{
					commentaryObject.GetComponent<TwoObjectsCollideCommentary> ().activateCommentary ();
				} else
					Debug.Log ("Commentary object not set in inspector.");
			}
			else
				loadingTransition.startCommentaryDialogue();
			/*
            Debug.Log("COMPLETE,COMPLETE,COMPLETE");
            if (nextLevelName != null)
            {
                SceneManager.LoadSceneAsync(nextLevelName);
            }
            else
            {
                Debug.Log("No scene set for next level on waypoint");
            }*/
        }

        else if (col.tag == "RepairStation")
        {

			loadingTransition.startCommentaryDialogue();
			/*
            Debug.Log("COMPLETE,COMPLETE,COMPLETE");
            if (nextLevelName != null)
            {
                SceneManager.LoadSceneAsync(nextLevelName);
            }
            else
            {
                Debug.Log("No scene set for next level on waypoint");
            }*/
        }

    }
}
