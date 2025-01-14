﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SWITCHSETNAME", menuName = "ScriptableObjects/GameSwitches")]
public class Switches : ScriptableObject
{
    [ColorField(1f, 0f, 0f)] [SerializeField] [ReadOnlyInGame]
    private bool S_LOAD_EVERYTHING_ON_START = false;
    public bool LOAD_EVERYTHING_ON_START { get {return S_LOAD_EVERYTHING_ON_START; } }

    [ColorField(1f, 0f, 0f)] [SerializeField] [ReadOnlyInGame]
    private bool S_FADE_CHUNK_PROPS = false;
    public bool FADE_CHUNK_PROPS { get {return S_FADE_CHUNK_PROPS;} }

    [ColorField(1f, 1f, 0f)] [SerializeField] [ReadOnlyInGame] 
    private bool S_CACHE_CHUNK_PROPS = true;
    public bool CACHE_CHUNK_PROPS { get { return S_CACHE_CHUNK_PROPS; } }

    [ColorField(1f, 1f, 0f)] [SerializeField] [ReadOnlyInGame] 
    private bool S_DEFERRED_CHUNK_PROPS_LOADING = false;
    public bool DEFERRED_CHUNK_PROPS_LOADING { get { return S_DEFERRED_CHUNK_PROPS_LOADING; } }

    [ColorField(0f, 1f, 0f)] [SerializeField] [ReadOnlyInGame] 
    private bool S_LOG_CHUNK_LOADER = false;
    public bool LOG_CHUNK_LOADER { get { return S_LOG_CHUNK_LOADER; } }
}
