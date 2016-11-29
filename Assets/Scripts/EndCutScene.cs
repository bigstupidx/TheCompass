﻿using UnityEngine;
using System.Collections;

/*
 * Author: Timothy Touch
 * Attach this script to the child object with a collider with "Is Trigger" checked.
 * Parent should have "TriggerCutScene" script
 * 
 * This script is used to signal when the player has reached their destination and effectively
 * reenabling player control.
 * 
 */

public class EndCutScene : MonoBehaviour
{

    private string _playerName = "PlayerPlaceholder";
    private GameObject _parent;
    public bool endLevel = false;
    public bool disableCameraFollow = false;
    //checks if player is already in cutscene
    private bool endpointActive = false;
    private bool nextCutscene = false;
    //if this starts another cutscene path
    public Transform target;
    private Collider2D _player;
    private bool _reachedDestination = false;
    public float speed = 10;
    public float pauseTime;
    private bool _wasTriggered = false;
    private CameraController camControl;
    private LoadingTransition loadingTransition;
    public GameObject transitionBox;
    // Use this for initialization
    void Start()
    {
        _parent = this.transform.parent.gameObject;
        if(transitionBox != null)
        {
            loadingTransition = transitionBox.GetComponent<LoadingTransition>();
        }
        if(disableCameraFollow == true)
        {
            camControl = Camera.main.GetComponent<CameraController>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (nextCutscene == true && !_reachedDestination && _player != null)
        {
            _player.SendMessage("cutSceneMovePlayer", speed);
        }
    }
    public void SetEndpointActive()
    {
        endpointActive = true;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && endpointActive == true)
        {
            if(disableCameraFollow == true)
            {
                camControl.DisableCamFollow();
            }
            _parent.SendMessage("setReachedDestination", true);
            other.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            other.transform.position = transform.position;
            other.transform.rotation = Quaternion.identity * Quaternion.Euler(0,0,-90);
            //show ending dialogue if this is an end trigger
            if (endLevel && transitionBox != null)
            {
                loadingTransition.startCommentaryDialogue();
            }
           
            if (target != null && !_wasTriggered)
            {
                _player = other;
                StartCoroutine(StartCutscene());
            }
        }
    }

    IEnumerator StartCutscene()
    {
       
            yield return new WaitForSeconds(pauseTime);
            nextCutscene = true;
            target.GetComponent<EndCutScene>().SetEndpointActive();
            setReachedDestination(false);
            _player.SendMessage("setPlayerDestination", (Vector2)target.position);
            _wasTriggered = true;
        
    }

    public void setReachedDestination(bool hasReached)
    {
        _reachedDestination = hasReached;
        /*
        if (_player != null && _reachedDestination == true)
        {
            
                _player.SendMessage("setDisablePlayerControl", false);
            
            
        }
        */
    }
}
