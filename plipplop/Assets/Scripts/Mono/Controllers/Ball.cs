﻿using UnityEngine;

public class Ball : Controller
{
    [Header("Specific properties")]
    public float maxSpeed = 200f;
    public float acceleration = 1000f;
    public float jumpForce = 7f;
    public float airRollFactor = 4f;
    public float moveLerp = 1;
    public float drag = 1f;
    public float speedBeforeRoll = 12f;
    public float timeBeforeRoll = 1f;
    public float speedBeforeStandingUp = 2f;

    new Renderer renderer;
    new SphereCollider collider;
    Transform childBall;
    float timeStartedRolling = 0f;
    bool isWaitingToRoll = false;

    public override void OnPossess()
    {
        base.OnPossess();
        ExtendLegs();
    }

    public override void OnEject()
    {
        base.OnEject();
    }

    internal override void SpecificMove(Vector3 direction)
    {
        var perimeter = 2 * Mathf.PI * (1f); //radius
        Vector3 _velocity = (Game.i.aperture.Right() * direction.x + Game.i.aperture.Forward() * direction.z) * Time.deltaTime;
        _velocity *= maxSpeed;
        _velocity = Vector3.ClampMagnitude(_velocity, maxSpeed);
        _velocity.y = rigidbody.velocity.y;
        rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, _velocity, Time.deltaTime * moveLerp);

        if(rigidbody.velocity.magnitude > 0.2f) transform.forward = rigidbody.velocity.normalized;


        var factor = 1;
        var amount = new Vector3(
            (rigidbody.velocity.z / perimeter) * 10f,
            0f,
            -(rigidbody.velocity.x / perimeter) * 10f
        ) * factor;

        childBall.Rotate(amount, Space.World);
    }

    internal override void SpecificJump() //niveau son c'est clunky car tu peux quand même appuyer sur A quand tu es en l'air, donc ça joue le "specific jump" global
    {
        if(WasGrounded()) 
        {
            SoundPlayer.Play("sfx_beachball_jump");
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpForce, rigidbody.velocity.z);
        }
    }

    internal override void OnLegsRetracted()
    {
        collider.enabled = true;
        rigidbody.drag = drag;
		ResetVisuals();
	}

    internal override void OnLegsExtended()
    {
        collider.enabled = false;
		ResetVisuals();
	}

    internal override void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponentInChildren<MeshRenderer>();
        collider = GetComponent<SphereCollider>();
        renderer.material = Instantiate<Material>(renderer.material);
        childBall = transform.GetChild(0);
    }

    internal override void Start()
    {
        base.Start();
    }
    
    internal override void Update()
    {
        base.Update();

        if (IsPossessed())
        {
            if (rigidbody.velocity.magnitude > speedBeforeRoll) {

                if (!isWaitingToRoll) {
                    timeStartedRolling = Time.time;
                    isWaitingToRoll = true;
                }
                if (isWaitingToRoll && Time.time > timeStartedRolling + timeBeforeRoll && !AreLegsRetracted()) {
                    RetractLegs();
                    isWaitingToRoll = false;
                }
            }
            else if (IsGrounded() && rigidbody.velocity.magnitude < speedBeforeStandingUp) {
                isWaitingToRoll = false;
                if (AreLegsRetracted()) ExtendLegs();
            }
        }
    }
}
