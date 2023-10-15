using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
	[Range(0f, 1f)][SerializeField] private float _motorState = 0.0f;
	[Range(0f, 1f)][SerializeField] private float _brakeState = 0.0f;
	[Range(-1f, 1f)][SerializeField] private float _steerState = 0.0f;

	[SerializeField] private Force _motorTorque;
	[SerializeField] private Force _brakeTorque;
	[SerializeField] private Force _brakeMotorTorque;
	[SerializeField] private Force _steerAngle;

	[Space(10)]
	[Tooltip("How much center off mass is offset downwards if car is about to flip, to counter the flip.")]
	[SerializeField] private float _centerOffMassLiftForceZ = 1f;
	[SerializeField] private AnimationCurve _centerOffMassLiftCurveZ;

	[Space(10)]
	[SerializeField] private Rigidbody _rb;
	[SerializeField] private List<WheelCollider> _accelarationWheels = new List<WheelCollider>();
	[SerializeField] private List<WheelCollider> _brakeWheels = new List<WheelCollider>();


	public void SetMotorState(float state)
	{ _motorState = Mathf.Clamp01(state); }

	public void SetBrakeState(float state)
	{
		_brakeState = Mathf.Clamp01(state);

		if (_brakeState > _brakeTorque.ZeroRoundThreshold)
		{ _motorTorque.CurrentState = 0.0f; }
	}
	public void SetSteerState(float state)
	{ _steerState = Mathf.Clamp(state, -1f, 1f); }


	private Vector3 _originalCenterOffMass = Vector3.zero;


	private void Start()
	{
		_originalCenterOffMass = _rb.centerOfMass;
	}

	public void OnSpawn()
	{
		// Reset Rigidbody.
		_rb.isKinematic = false;
		_rb.velocity = Vector3.zero;
		_rb.angularVelocity = Vector3.zero;
		_rb.useGravity = true;

		_rb.centerOfMass = _originalCenterOffMass;

		// Reset values.
		_motorTorque.CurrentState = 0f;
		_brakeTorque.CurrentState = 0f;
		_brakeMotorTorque.CurrentState = 0f;
		_steerAngle.CurrentState = 0f;

		_motorState = 0.0f;
		_brakeState = 0.0f;
		_steerState = 0.0f;

		_accelarationWheels.ForEach(x => x.brakeTorque = Mathf.Infinity);
		_brakeWheels.ForEach(x => x.brakeTorque = Mathf.Infinity);

		this.enabled = true;
	}
	public void OnDespawn()
	{
		// Disable Rigidbody.
		_rb.isKinematic = true;
		_rb.useGravity = false;

		this.enabled = false;
	}

	private void FixedUpdate()
	{
		// Update forces.
		_motorTorque.FixedUpdate(_motorState);
		_brakeTorque.FixedUpdate(_brakeState);
		_brakeMotorTorque.FixedUpdate(_brakeState);
		_steerAngle.FixedUpdate(_steerState);

		// Apply acceleration.
		_accelarationWheels.ForEach(wheel =>
		{
			wheel.motorTorque = _motorTorque.CurrentForce() + _brakeMotorTorque.CurrentForce();
			wheel.steerAngle = _steerAngle.CurrentForce();
		});

		// Apply brake.
		_brakeWheels.ForEach(wheel => wheel.brakeTorque = _brakeTorque.CurrentForce());

		// Reposition center off mass based on Z rotation.
		// To counter flipping over.
		Vector3 currentAngle = _rb.transform.localEulerAngles;
		if (currentAngle.z < 0) { currentAngle.z += 360f; }

		_rb.centerOfMass = _originalCenterOffMass +
			new Vector3(0f,
			_centerOffMassLiftForceZ * _centerOffMassLiftCurveZ.Evaluate(currentAngle.z / 180f),
			0f);
	}
}