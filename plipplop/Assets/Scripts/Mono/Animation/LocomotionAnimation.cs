﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionAnimation
{
    public float legsHeight;
    public Vector3 legsOffset;
    public bool isJumping;
    public bool isWalking;

    Transform parentTransform;
    public Rigidbody rigidbody;
    BoxCollider legsCollider;
    MeshAnimator legs;
    Transform visualsTransform;

    public LocomotionAnimation(Rigidbody rb, BoxCollider legsCollider, Transform visualsTransform)
    {
        this.rigidbody = rb;
        this.legsCollider = legsCollider;
        parentTransform = legsCollider.transform;
        this.visualsTransform = visualsTransform;
        GrowLegs();
    }

    public void Update()
    {

        legs.transform.localPosition = legsOffset - Vector3.up*(legsHeight);
        SetLegHeight();


        if (isWalking) legs.PlayOnce("Walk");
        else legs.PlayOnce("Idle");
    }

    public bool AreLegsRetracted()
    {
        return legs == null || !legs.gameObject.activeSelf;
    }

    public void RetractLegs()
    {
        legs.gameObject.SetActive(false);
        legsCollider.enabled = false;
        ResetVisualRotation(); // TODO : Remove
    }

    public void ExtendLegs()
    {
        legs.gameObject.SetActive(true);
        legsCollider.enabled = true;
        ResetVisualRotation(); // TODO : Remove
    }

    void ResetVisualRotation()
    {
        if(visualsTransform != null) visualsTransform.localEulerAngles = Vector3.zero;
    }

    void GrowLegs()
    {
        legs = Object.Instantiate(Game.i.library.legsPrefab, parentTransform)
        .GetComponent<MeshAnimator>();
     //   legs.body = parentTransform;
        legs.transform.localPosition = legsOffset;
     //   foreach (Leg l in legs.legs) l.maxFootDistance = legsHeight + 2f;
    }

    void SetLegHeight()
    {
        legsCollider.size = new Vector3(1f, legsHeight, 1f);
        legsCollider.center = legsOffset + new Vector3(0f, -legsHeight / 2, 0f);
        legs.transform.localScale = (Vector3.one - Vector3.up) + Vector3.up * legsHeight;
    }
}
