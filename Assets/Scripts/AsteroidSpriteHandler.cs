﻿using UnityEngine;
using System.Collections;

public class AsteroidSpriteHandler : MonoBehaviour {

    SpriteRenderer rend;
    Sprite[] debrisTextures;
    int numOfTextures;

	// Use this for initialization
	void Start ()
    {
        rend = GetComponent<SpriteRenderer>();
        if(gameObject.name == "SplitterShard(Clone)")
        {
            debrisTextures = Resources.LoadAll<Sprite>("SplitterShardSprites");
        }
        else
        {
            debrisTextures = Resources.LoadAll<Sprite>("AsteroidSprites");
        }
        
        numOfTextures = debrisTextures.Length;
        //Debug.Log("Number of tex: "+numOfTextures);   
        rend.sprite = debrisTextures[Random.Range(0,numOfTextures-1)]; 
	}
	
	
}
