﻿using UnityEngine;
using System.Collections;

public class AutoSave : MonoBehaviour {
	void Start () {
        GameData gd = SaveLoad.LoadGame();
        BranchData.Singleton = gd.Branch;
        SaveLoad.SaveGame();
	}
}
