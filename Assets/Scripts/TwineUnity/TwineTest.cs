﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TwineTest : MonoBehaviour {
	#region Variable Declaration
	public TextAsset DialogueFile;

	/* UI Stuff */
	public Button ChoiceButtonPrefab;
	public Button ChoiceButtonDisabledPrefab;
	public RectTransform ChoicePanel;
	public float VerticalSpacing = -45f;
	public Text PassageTextDisplay;
	public string NextScene;
	public string ContinueText;

    public string AlternateStartTag = "AlternateStart";
    public string AlternateEndTag = "AlternateEnd";
    public int EnemiesKilledThreshhold = 5;
    public float AlternateEndDelayTime = 3;

    public string GameOverTag = "GameOver";
	//The audio source for the voice over
	public AudioSource voiceOverAudioSource;
	//The beeping audioSource
	public AudioSource typingBeepAudioSource;
	//bool so only one thread is active during a passage
	private bool _activatedCoRoutine;
	//number of seconds coroutine waits until it plays the beep again

	public float beepSecWait;
	[HideInInspector]
	public string PassageText;
	[HideInInspector]
	public List<Button> Choices;
	[HideInInspector]
	public TwineDialogue TDialogue;
	[HideInInspector]
	public PassageNode CurrentPassage;
    [HideInInspector]
    public Button ContinueButton;
    [HideInInspector]
    public bool AlternateEndVisited = false;
    

	bool _currentlyTyping;
	#endregion

    void AlternateEndDelay()
    {
        PassageTextDisplay.text = "";
        PassageText = CurrentPassage.GetContent();
        _currentlyTyping = true;
        AlternateEndVisited = true;
    }

	void Start () {
		TDialogue = TwineReader.Parse(DialogueFile);
        List<PassageNode> AlternateStarts = TDialogue.GetPassagesTagged(AlternateStartTag);
        //BranchData.Singleton.EnemiesKilled = 5; //Testing the alternate beginning.
		CurrentPassage = AlternateStarts.Count > 0 && BranchData.Singleton.EnemiesKilled >= EnemiesKilledThreshhold ?
                            AlternateStarts[0] : TDialogue.StartPassage;
		PassageText = CurrentPassage.GetContent();
		_currentlyTyping = true;
		_activatedCoRoutine = false;
	}
	void Update()
	{
		if (Input.anyKeyDown && _currentlyTyping)
		{
			_activatedCoRoutine = false;
			_currentlyTyping = false;
			PassageTextDisplay.text = PassageText;
			AddChoiceButtons();
		}
		if (_currentlyTyping)
		{
			if (!_activatedCoRoutine) 
			{
				StartCoroutine (TypingBeep (beepSecWait));
				voiceOverAudioSource.Play ();
				_activatedCoRoutine = true;
			}
			string currentText = PassageTextDisplay.text;
			if (currentText.Length < PassageText.Length)
			{
				PassageTextDisplay.text = PassageText.Substring(0, currentText.Length + 1);
			}
			else
			{
				_activatedCoRoutine = false;
				_currentlyTyping = false;
				AddChoiceButtons();
			}
		}
	}
	public void AddChoiceButtons()
	{
		List<string> choiceList = CurrentPassage.GetChoices();
		List<string> disabledChoices = CurrentPassage.GetChoicesVisited(CurrentPassage.GetChoicesTagged(choiceList, TDialogue.ShowOnceTag));
		for (int i = 0; i < choiceList.Count; i++)
		{
			if (!disabledChoices.Contains(choiceList[i]))
			{
				Button newChoice = (Button)Instantiate(ChoiceButtonPrefab, ChoicePanel, false);
				newChoice.GetComponent<ChoiceButton>().tt = this;
				newChoice.GetComponentInChildren<Text>().text = choiceList[i];
				Choices.Add(newChoice);
				if (i == 0)
					newChoice.tag = "FirstButtonOfMenu";
			}
			else
			{
				Button newChoice = (Button)Instantiate(ChoiceButtonDisabledPrefab, ChoicePanel, false);
				newChoice.GetComponentInChildren<Text>().text = choiceList[i];
				Choices.Add(newChoice);
				if (i == 0)
					newChoice.tag = "FirstButtonOfMenu";
			}
		}
		if (Choices.Count <= 0)
		{
            if (CurrentPassage.GetTags().Contains(GameOverTag))
            {
                //TODO: IMPLEMENT GAME OVER SEQUENCE.
            }
            else
            {
                ContinueButton = (Button)Instantiate(ChoiceButtonPrefab, ChoicePanel, false);
                ContinueButton.GetComponentInChildren<Text>().text = ContinueText;
                ContinueButton.GetComponent<ChoiceButton>().tt = this;
                ContinueButton.tag = "FirstButtonOfMenu";
            }
		}
	}
	public void ChoiceSelect(string choiceContent)
	{
		if (Choices.Count > 0)
		{
			PassageNode decision = CurrentPassage.GetDecision(choiceContent);
            TDialogue.SetCurrentPassage(decision);
			if (decision != null)
			{
				CurrentPassage = decision;
				foreach (Button b in Choices)
				{
					Destroy(b.gameObject);
				}
				Choices.Clear();
				PassageTextDisplay.text = "";
				PassageText = CurrentPassage.GetContent();
				_currentlyTyping = true;
			}
			else
			{
				Debug.Log("ChoiceSelect failed!");
			}
		}
		else
		{
            List<PassageNode> AlternateEnds = TDialogue.GetPassagesTagged(AlternateEndTag);
            Debug.Log("AlternateEnds: "+AlternateEnds.Count);
            Debug.Log("Visited: " + BranchData.Singleton.ColorVisited);
            if (AlternateEnds.Count > 0 && BranchData.Singleton.ColorVisited && !AlternateEndVisited)
            {
                Destroy(ContinueButton.gameObject);
                PassageTextDisplay.text = "...";
                _currentlyTyping = false;
                CurrentPassage = AlternateEnds[0];
                Invoke("AlternateEndDelay", AlternateEndDelayTime);
            }
            else
            {
                SceneManager.LoadScene(NextScene);
            }
		}
	}
	//Coroutine that handles beeping for typing
	IEnumerator TypingBeep(float sec)
	{
		while (_currentlyTyping)
		{
			typingBeepAudioSource.PlayOneShot (typingBeepAudioSource.clip);
			yield return new WaitForSeconds(sec);
		}
	}
}
