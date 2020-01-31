﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : Dictionary<string, System.Action> 
{
    public Cheats()
    {
        Add("RESET", ResetScene);
        Add("ZIK", delegate { SoundPlayer.Play("bgm_test"); });
        Add("GLUE", delegate { Game.i.player.Paralyze(); });
        Add("FREE", delegate { Game.i.player.Deparalyze(); });
        Add("FAMINE", SetAllHunderToHundred);
        Add("BIGHEADS", BigHeads);
        Add("LONGHEADS", LongHeads);
        Add("MAUVAISESTETES", MauvaisesTetes);
        Add("HELLO", SpawnNPC);
        Add("IMONFIRE", SpawnFire);
        Add("STEP", SpawnPoof);
		Add("BOSS", Bossfight);
	}

	public void BigHeads()
	{
		foreach(NonPlayableCharacter npc in Object.FindObjectsOfType<NonPlayableCharacter>())
		{
			npc.skeleton.GetSocketBySlot(Clothes.ESlot.HEAD).bone.localScale = Vector3.one * Random.Range(3f, 6f);
		}
	}

	public void Bossfight()
	{
		foreach (NonPlayableCharacter npc in Object.FindObjectsOfType<NonPlayableCharacter>())
		{
			ICarryable torch = Game.Instantiate(Game.i.library.torchPrefab).GetComponent<ICarryable>();
			npc.agentMovement.Chase(Game.i.player.GetCurrentController().transform);
			npc.Carry(torch);
			npc.sight.multiplier = 0f;
			npc.emo.Show("Angry");
		}
		Game.i.aperture.currentCamera.GetComponent<UnityEngine.Experimental.Rendering.HDPipeline.HDAdditionalCameraData>().backgroundColorHDR = Color.red;
	}

	public void LongHeads()
    {
        foreach(NonPlayableCharacter npc in Object.FindObjectsOfType<NonPlayableCharacter>())
        {
            npc.skeleton.GetSocketBySlot(Clothes.ESlot.HEAD).bone.localPosition += Vector3.up * Random.Range(.2f, 1f);
        }
    }

    public void MauvaisesTetes()
    {
        foreach(NonPlayableCharacter npc in Object.FindObjectsOfType<NonPlayableCharacter>())
        {
            npc.skeleton.GetSocketBySlot(Clothes.ESlot.HEAD).bone.localScale = Vector3.one * Random.Range(3f, 6f);
            npc.skeleton.GetSocketBySlot(Clothes.ESlot.HEAD).bone.localPosition += Vector3.up * Random.Range(.2f, 1f);
        }
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetAllHunderToHundred()
    {
        foreach(NonPlayableCharacter npc in Object.FindObjectsOfType<NonPlayableCharacter>())
            npc.SetStat(NonPlayableCharacter.EStat.HUNGER, 100f);
    }

    public void SpawnNPC()
    {
        var o = Object.Instantiate(Game.i.library.npcLibrary.NPCSamplePrefab);
        o.transform.position = Game.i.player.GetCurrentController().transform.position + Game.i.player.GetCurrentController().transform.forward * 3f;
    }

    public void SpawnPoof()
    {
        Pyromancer.PlayGameEffect("gfx_step", Game.i.player.GetCurrentController().transform.position);
    }

    public void SpawnFire()
    {
        Pyromancer.PlayVFXAttached("vfx_fire", Game.i.player.GetCurrentController().transform);
    }
}
