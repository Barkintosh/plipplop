﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior
{
	[System.Serializable]
	public abstract class Condition : ScriptableObject
    {
        public abstract bool Check(AIState state, NonPlayableCharacter target);
    }
}
