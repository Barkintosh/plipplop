
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GolfCar : Controller
{
    [Header("Specific properties")]
    public float acceleration = 5f;
    public float maxSpeed = 100f;
    public float steeringSpeed = 2f;
    public float maxSteering = 0.7f;
    public float steeringForce = 400f;
    public float autoBrakeSpeed = 1f;
    public float antiSpinSpeed = 1f;
    public float tiltAmount = 15f;
    public float antiFlipForce = 5f;

    public Transform[] wheels;
    public Transform visual;

    float currentTilt = 0f;
    float steering = 0f;

    public override void OnEject()
    {
        base.OnEject();
        // Code here
    }

    public override void OnPossess()
    {
        base.OnPossess();
        // Code here
    }

    internal override void SpecificMove(Vector3 direction)  
    {
        if (!IsGrounded()) direction = Vector3.zero;

        // Thrust
        rigidbody.AddForce(transform.forward * direction.z * acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);

        var localSpeed = transform.InverseTransformDirection(rigidbody.velocity);
        var thrustObjective = localSpeed.z;

        if (Mathf.Abs(localSpeed.z) > maxSpeed) {
            thrustObjective = Mathf.Sign(localSpeed.z) * maxSpeed;
        }
        
        // Auto brake
        thrustObjective = Mathf.Lerp(thrustObjective, thrustObjective*(Mathf.Abs(direction.z)), autoBrakeSpeed * Time.fixedDeltaTime);
        localSpeed.x = 0f; // No strafing, good tires

        // Steering + anti spin
        steering = Mathf.Lerp(steering, direction.x, steeringSpeed * Time.fixedDeltaTime) * maxSteering;
        var localSpin = transform.TransformDirection(rigidbody.angularVelocity);
        localSpin.y = steering * steeringForce * (thrustObjective / maxSpeed) * Time.deltaTime;
        localSpin.y = Mathf.Lerp(localSpin.y, localSpin.y * Mathf.Abs(direction.x), antiSpinSpeed * Time.fixedDeltaTime);

        // Apply
        rigidbody.velocity = transform.TransformDirection(new Vector3(localSpeed.x, localSpeed.y, thrustObjective));
        rigidbody.angularVelocity = transform.InverseTransformDirection(localSpin);


        // Tilt animation
        currentTilt = Mathf.Lerp(currentTilt, (steering / maxSteering) * tiltAmount * (thrustObjective/maxSpeed), 4f * Time.fixedDeltaTime);
        visual.localEulerAngles = new Vector3(visual.localEulerAngles.x, visual.localEulerAngles.y, currentTilt);

    }

    internal override void Start()
    {
        base.Start();
        // Code here
    }

    internal override void Update()
    {
        base.Update();
        // Code here
    }

    internal override void FixedUpdate()
    {
        base.FixedUpdate();

        // Reorient the car to optimal rotation to avoid flipping
        // (Anti spin)
        if (!IsGrounded()) {
            var y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(
                    0f, y, 0f
                ),
                antiFlipForce * Time.fixedDeltaTime
            );
        }

        // Wheel animation
        var localSpeed = transform.InverseTransformDirection(rigidbody.velocity);
        var factor = 1;
        foreach (var wheel in wheels) {
            wheel.Rotate(new Vector3(0f, 1f, 0f) * localSpeed.z * factor);
        }

        // Even when not controlled, the car shouldn't drift away
        if (IsPossessed()) {
            rigidbody.drag = 0f;
            rigidbody.angularDrag = 0f;
        }
        else {
            rigidbody.drag = 4f;
            rigidbody.angularDrag = 2f;

            // No tilting
            visual.localEulerAngles = new Vector3(visual.localEulerAngles.x, visual.localEulerAngles.y, 0f);
        }

    }

    internal override void OnLegsRetracted()
    {
        // Code here
    }

    internal override void OnLegsExtended()
    {
        // Code here
    }
}
