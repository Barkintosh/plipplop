﻿using UnityEngine;

public class Ball : Controller
{
	[Header("Specific properties")]
	public float jumpComboWindow = 0.5f;
	public float jumpComboHForceBonus = 1000f;
	public float jumpComboVForceBonus = 1000f;
	public int maxCombo = 5;
	public float rotationSpeed = 0.5f;
	public float velocityDamplerOnImpact = 0.75f;
	public float horizontalForce;
	public float verticalForce;

	int combo = 1;
	float comboTimer = 0f;
	bool hopped = false;
    bool canJumpAgain = true;
    Vector3 lastOrientation;

	public override void OnEject()
	{
		base.OnEject();
		Initialize();
	}

	public override void OnPossess()
	{
		base.OnPossess();
		Initialize();
	}

	public void Initialize()
	{
		comboTimer = 0f;
		combo = 1;
		hopped = false;
		lastOrientation = Vector3.zero;
	}

	public void OnCollisionEnter(Collision collision)
	{
        canJumpAgain = true;

        if (hopped)
		{
            Jump(collision.GetContact(0).point);
            hopped = false;
        }
	}

	internal override void Update()
	{
		base.Update();
		if (comboTimer > 0f) comboTimer -= Time.deltaTime;
	}

	internal override void SpecificMove(Vector3 direction)
    {
		if(movingStick)
		{
			Rotate(direction);
			if (!hopped)
			{
				Bump(direction);
				hopped = true;
			}
		}

		if(direction.magnitude > 0.25f)
		{
			lastOrientation = (Game.i.aperture.Right() * direction.x + Game.i.aperture.Forward() * direction.z);
		}
	}

	void Rotate(Vector3 direction)
	{
		Vector3 dir = (Game.i.aperture.Forward() * -direction.x + Game.i.aperture.Right() * direction.z);
		rigidbody.angularVelocity += dir * rotationSpeed;
	}

	void Bump(Vector3 direction)
	{
		Vector3 dir = (Game.i.aperture.Right() * direction.x + Game.i.aperture.Forward() * direction.z);
		rigidbody.AddForce(dir * (horizontalForce + ((combo - 1) * jumpComboHForceBonus)) * Time.deltaTime);
		rigidbody.AddForce(Vector3.up * (verticalForce + ((combo - 1) * jumpComboVForceBonus)) * Time.deltaTime);
	}

    void Jump(Vector3 pointOfContact)
    {
        rigidbody.velocity *= velocityDamplerOnImpact;
        JumpFX(pointOfContact);


        if (comboTimer > 0) {
            if (combo < maxCombo)
                combo++;
        }
        else {
            combo = 1;
        }
    }

    void JumpFX(Vector3 pointOfContact)
    {

        Pyromancer.PlayGameEffect("gfx_bounce", pointOfContact);
        SoundPlayer.PlayAtPosition("sfx_beachball_jump_" + combo, pointOfContact, 1f, true);
    }

    internal override void SpecificJump()
    {
		if(comboTimer <= 0) comboTimer += jumpComboWindow;
        if (!movingStick && !hopped && IsGrounded() && canJumpAgain) {
            Bump(Vector3.up);
            canJumpAgain = false;
            combo = 1;
            JumpFX(transform.position - 0.5f * Vector3.up);
        }
	}

    internal override void OnLegsRetracted()
    {
		//rigidbody.constraints = RigidbodyConstraints.None;
	}

    internal override void OnLegsExtended()
    {
		AlignPropOnHeadDummy();
		transform.forward = lastOrientation;
		//rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}
}
