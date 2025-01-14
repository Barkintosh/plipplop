﻿using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/OrientTowardThing")]
	public class OrientTowardThing : AIAction
    {
		public NonPlayableCharacter.ESubject thing;
		public override void Execute(NonPlayableCharacter target)
        {
			NonPlayableCharacter npc = target;
			if (npc == null) return;
			switch (thing)
			{
				case NonPlayableCharacter.ESubject.PLAYER:
					if (npc.player != null)
						npc.movement.OrientToward(npc.player.transform.position);
					break;
				case NonPlayableCharacter.ESubject.VALUABLE:
					if(npc.valuable != null)
						npc.movement.OrientToward(npc.valuable.transform.position);
					break;
				case NonPlayableCharacter.ESubject.ACTIVITY:
					if (npc.activity != null)
						npc.movement.OrientToward(npc.activity.transform.position);
					break;
				case NonPlayableCharacter.ESubject.CHAIR:
					if (npc.chair != null)
						npc.movement.OrientToward(npc.chair.transform.position);
					break;
				case NonPlayableCharacter.ESubject.FOOD:
					if (npc.food != null)
						npc.movement.OrientToward(npc.food.transform.position);
					break;
				case NonPlayableCharacter.ESubject.FEEDER:
					if (npc.feeder != null)
						npc.movement.OrientToward(npc.feeder.transform.position);
					break;
				case NonPlayableCharacter.ESubject.CHARACTER:
					if (npc.character != null)
						npc.movement.OrientToward(npc.character.transform.position);
					break;
			}
		}
	}
}
