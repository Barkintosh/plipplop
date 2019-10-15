﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Library
{
    [Header("Strings")]
    public string gameName = "Plip plop";

    [Header("Prefabs")]
    public GameObject legsPrefab;
    public GameObject facePrefab;
    public GameObject aperture;
    public GameObject controllerSensor;
    public GameObject baseControllerPrefab;

}
