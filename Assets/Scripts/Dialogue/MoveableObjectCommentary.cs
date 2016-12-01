﻿using UnityEngine;
using System.Collections;

public class MoveableObjectCommentary : MonoBehaviour
{


	public TextAsset textFile;
	public string speakerName;
	public int startLine;
	public int endLine;

	public AudioClip audioClip;
	public TextBoxManager theTextBox;

	public bool destroyWhenActivated,timedDialogue;
	public float timeUntilFinished;
	public GameObject MoveableObject;
	private MoveableObject _moveableObjectScript;
	private bool _triggeredOnce;
	// Use this for initialization
	void Start ()
	{
		_triggeredOnce = false;
		theTextBox = FindObjectOfType<TextBoxManager>();
		_moveableObjectScript = MoveableObject.GetComponent<MoveableObject> ();
	}

	// Update is called once per frame
	void Update ()
	{
		if (_moveableObjectScript.isTractored == true && !_triggeredOnce) 
		{
			_triggeredOnce = true;
			theTextBox.startCommentaryDialogue ();
			theTextBox.setVoiceOverSourceClip(audioClip);
			theTextBox.ReloadScript (textFile);
			theTextBox.currentLine = startLine;
			theTextBox.endAtLine = endLine;
			//theTextBox.EnableTextBox();
			theTextBox.setSpeakerNameText (speakerName);

			if (timedDialogue) 
			{
				theTextBox.activateTimedCommentary (timeUntilFinished);
			}
			if (destroyWhenActivated) 
			{
				Destroy (gameObject);
			}
		}
	}
}