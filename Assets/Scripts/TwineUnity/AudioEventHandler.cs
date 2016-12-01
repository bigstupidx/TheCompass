﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioEventHandler : MonoBehaviour {

	//public string ColorChangeTag = "FlagColor";
	public AudioSource audioSource;
	private PassageNode _node;
	private int _pid;
	private AudioClip _clip;
	public AudioClip[] audioClips;
	//Dictionary<int, AudioClip> Passage;
	// Use this for initialization
	void Start()
	{
		//Passage = new Dictionary<int, AudioClip>();
	}
	void OnEnable () {
		TwineDialogue.OnChange += GetAndPlayAudio;
	}
	void OnDisable () {
		TwineDialogue.OnChange -= GetAndPlayAudio;
	}
	void GetAndPlayAudio()
	{
		_node = TwineDialogue.Singleton.CurrentPassage;
		_pid = _node.GetID ();
		Debug.Log ("PID: " + _pid);
		//Passage.TryGetValue (_pid, out _clip);
		audioSource.clip = audioClips[_pid];
	}
}