﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/SearchValuable")]
	public class SearchValuable : StateActions
	{
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null)
			{
				Valuable[] items = npc.sight.Scan<Valuable>();
				if(items.Length > 0) npc.valuable = items[0];
			}
		}
	}
}

