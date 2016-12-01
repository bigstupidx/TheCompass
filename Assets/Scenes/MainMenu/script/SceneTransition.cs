﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class SceneTransition : MonoBehaviour
{
    const string FALLBACK_SCENE = "Level 0 Tutorial";
    public static string nextScene { private get; set; }
    Animator anim;

    void Awake()
    {
        if (nextScene == null)
            nextScene = FALLBACK_SCENE;

        anim = GetComponent<Animator>();
    }

    public void LoadScene(string scene)
    {
        Debug.Log("asdada");
        nextScene = scene;
        anim.SetTrigger(-556287998); // "Exit Scene"
    }

    void LoadNextScene()
    {
        Debug.Log("LoadNextScene()");
        if (nextScene != null)
        {
            Debug.Log("NEXT SCENE");
            SaveLoad.SaveGameWithScene(nextScene);
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log("FALLBACK");
            SaveLoad.SaveGameWithScene(FALLBACK_SCENE);
            SceneManager.LoadScene(FALLBACK_SCENE);
        }
    }

    public void LoadSavedGame()
    {
        GameData data = (GameData)SaveLoad.LoadFile(SaveLoad.defaultFilePath);
        Debug.Log(data.Scene);
        if (data != null)
        {
            LoadScene(data.Scene);
        }
        else
        {
            LoadScene(null);
        }
    }

    public void Quit()
    {
        Debug.Log("you are leave");
        Application.Quit();
    }
}
