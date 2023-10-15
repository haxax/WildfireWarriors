using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Force
{
	[Tooltip("The maximum amount of force at the peak of AccelerationCurve.")]
	[SerializeField] private float _maxForce = 10.0f;

	[Tooltip("Duration in seconds it takes to accelerate from 0 to 1.")]
	[SerializeField] private float _accelerationDuration = 1.0f;

	[Tooltip("Curve of the CurrentForce's amount in each acceleration  state. Timeline should be within 0 to 1.")]
	[SerializeField] private AnimationCurve _accelerationCurve = new AnimationCurve();
	[Tooltip("Curve of the CurrentForce's amount in each deceleration state. Timeline should be within 0 to 1.")]
	[SerializeField] private AnimationCurve _decelerationCurve = new AnimationCurve();

	[Tooltip("If CurrentState can be less than zero, round to zero if within the threshold.")]
	[SerializeField] private float _zeroRoundThreshold = 0.0001f;


	/// <summary>
	/// The maximum amount of force at the peak of AccelerationCurve.
	/// </summary>
	public float MaxForce { get => _maxForce; set => _maxForce = value; }

	/// <summary>
	/// Duration in seconds it takes to accelerate from 0 to 1.
	/// </summary>
	public float AccelerationDuration { get => _accelerationDuration; set => _accelerationDuration = value; }

	/// <summary>
	/// Curve of the CurrentForce's amount in each acceleration  state. Timeline should be within 0 to 1.
	/// </summary>
	public AnimationCurve AccelerationCurve { get => _accelerationCurve; set => _accelerationCurve = value; }

	/// <summary>
	/// Curve of the CurrentForce's amount in each deceleration state. Timeline should be within 0 to 1.
	/// </summary>
	public AnimationCurve DecelerationCurve { get => _decelerationCurve; set => _decelerationCurve = value; }

	/// <summary>
	/// If CurrentState can be less than zero, round to zero if within the threshold.
	/// </summary>
	public float ZeroRoundThreshold { get => _zeroRoundThreshold; set => _zeroRoundThreshold = value; }

	/// <summary>
	/// Current phase of acceleration/deceleratin within the curves.
	/// </summary>
	public float CurrentState { get => _currentState; set => _currentState = value; }
	private float _currentState = 0.0f;

	/// <summary>
	/// Current direction of acceleration/deceleration.
	/// </summary>
	public float CurrentDirection { get => _currentDirection; set => _currentDirection = value; }
	private float _currentDirection = 1f;

	/// <summary>
	/// Current force based on CurrentState and AccelerationCurve/DecelarationCurve.
	/// </summary>
	public float CurrentForce()
	=> CurrentDirection > 0 ? AccelerationCurve.Evaluate(CurrentState) * MaxForce : DecelerationCurve.Evaluate(CurrentState) * MaxForce;


	/// <summary>
	/// Updates the CurrentState of the Force. Call from the FixedUpdate which uses this Force.
	/// </summary>
	/// <param name="inputState">Value between the AccelerationCurve's nodes. Usually either -1 to 1 or 0 to 1.</param>
	public void FixedUpdate(float inputState)
	{
		// Clamp inputState within AccelerationCurve time.
		inputState = Mathf.Clamp(inputState, AccelerationCurve[0].time, AccelerationCurve[AccelerationCurve.length - 1].time);

		// Don't update if inputState is already reached.
		if (inputState == CurrentState) { return; }

		CurrentDirection = Mathf.Sign(inputState - CurrentState);

		// Add or reduce currentState based on whether inputState is higher or lower.
		inputState = inputState - CurrentState;
		CurrentState += (Time.fixedDeltaTime / AccelerationDuration) * Mathf.Sign(inputState);

		// Round to 0 if within threshold to avoid jittering around 0.
		if (Mathf.Abs(CurrentState) < _zeroRoundThreshold)
		{ CurrentState = 0f; }

		// Clamp calculated currentState within AccelerationCurve time.
		CurrentState = Mathf.Clamp(CurrentState, AccelerationCurve[0].time, AccelerationCurve[AccelerationCurve.length - 1].time);
	}
}