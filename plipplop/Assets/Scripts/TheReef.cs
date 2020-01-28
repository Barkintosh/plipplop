﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TheReef : NonPlayableCharacter
{
	private Controller controller;
	private Controller player;
	private NonPlayableCharacter target;

	public float throwForce = 10000f;
	public override void Kick(Controller c)
	{
		controller = c;
		animator.SetTrigger("Eject");
		agentMovement.Stop();
		StartCoroutine(WaitAndKick());
	}
	IEnumerator WaitAndKick()
	{
		yield return new WaitForSeconds(0.5f);

		controller.Kick();
		player = Game.i.player.GetCurrentController();
		player.rigidbody.isKinematic = true;
		Drop();
		skeleton.Attach(controller.transform, Clothes.ESlot.LEFT_HAND, true);
		skeleton.Attach(player.transform, Clothes.ESlot.RIGHT_HAND, true);
		StartCoroutine(WaitAndThrow());
	}
	IEnumerator WaitAndThrow()
	{
		yield return new WaitForSeconds(0.85f);

		if (controller.TryGetComponent(out ICarryable result)) Carry(result);
		else skeleton.Attach(controller.transform, Clothes.ESlot.RIGHT_HAND, true);
		player.rigidbody.isKinematic = false;
		player.transform.position -= skeleton.GetSocketBySlot(Clothes.ESlot.RIGHT_HAND).bone.up * 2f;
		player.Throw(-skeleton.GetSocketBySlot(Clothes.ESlot.RIGHT_HAND).bone.up, throwForce);
		player.transform.up = Vector3.up;
		controller = null;
		player = null;
	}

	public float reassureTime = 1f;
	public void Reassure(NonPlayableCharacter npc)
	{
		target = npc;
		target.agentMovement.Stop();
		agentMovement.Stop();
		Vector3 dir = (target.transform.position - transform.position).normalized;
		target.transform.forward = -dir;
		transform.forward = dir;
		StartCoroutine(WaitAndEndReassure());
	}
	IEnumerator WaitAndEndReassure()
	{
		yield return new WaitForSeconds(reassureTime);
		target.graph.Move(6406);
		target = null;
	}
}
