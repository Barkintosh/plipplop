﻿using UnityEngine;
namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/CollectSubject")]
    public class CollectSubject : AIAction
    {
		public NonPlayableCharacter.ESubject subject;
		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null)
			{
				switch (subject)
				{
					case NonPlayableCharacter.ESubject.VALUABLE:
						if (npc.valuable != null)
						{
							npc.Collect(npc.valuable);
							npc.emo.Show(Emotion.EVerb.HATE, npc.valuable.Name());
						}
						break;
					case NonPlayableCharacter.ESubject.FOOD:
						if (npc.food != null)
						{
							npc.Collect(npc.food);
							npc.emo.Show(Emotion.EVerb.SEARCH, npc.food.name);
						}
						break;
				}
			}
        }
    }
}
