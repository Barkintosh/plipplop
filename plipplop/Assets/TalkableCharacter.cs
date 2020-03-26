﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TalkableCharacter : MonoBehaviour
{
    public float talkRadius = 2f;

    SphereCollider sphereTrigger;

    private void Awake()
    {
        sphereTrigger = gameObject.AddComponent<SphereCollider>();
        sphereTrigger.radius = talkRadius;
        sphereTrigger.isTrigger = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        var ctrl = Game.i.player.GetCurrentController();
        if (other.gameObject != ctrl.gameObject) return;
        if (Vector3.Distance(other.transform.position, ctrl.transform.position) > talkRadius) return;

        if (Game.i.player.currentChatOpportunity != null)
        {
            if (Vector3.Distance(Game.i.player.currentChatOpportunity.transform.position, other.transform.position) > Vector3.Distance(transform.position, other.transform.position))
            {
                Game.i.player.currentChatOpportunity = this;
            }
        }
        else
        {
            Game.i.player.currentChatOpportunity = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var ctrl = Game.i.player.GetCurrentController();
        if (other.gameObject != ctrl.gameObject) return;
        if (Vector3.Distance(other.transform.position, ctrl.transform.position) > talkRadius) return;

        if (Game.i.player.currentChatOpportunity == this)
        {
            Game.i.player.currentChatOpportunity = null;
        }
    }

    public abstract Dialog OnDialogTrigger();

    public void StartDialogue()
    {
        var dial = OnDialogTrigger();
        Game.i.PlayDialogue(dial);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        if (UnityEditor.EditorApplication.isPlaying)
        {
            if (Game.i.player.currentChatOpportunity == this)
                Gizmos.color = Color.green;
        }
        Gizmos.DrawWireSphere(transform.position, talkRadius);
    }
#endif

}
