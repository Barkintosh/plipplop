﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ChaseValuable")]
    public class ChaseValuable : StateActions
    {
        public override void Execute(StateManager state)
        {
            NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null && npc.valuable != null)
			{
                npc.agentMovement.Chase(npc.valuable.transform);
			}
        }
    }
}
