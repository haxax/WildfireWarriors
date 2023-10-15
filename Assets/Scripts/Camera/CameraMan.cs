using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMan : Poolable
{
	[SerializeField] private Camera _camera;
	[SerializeField] private float _movementSpeed = 1f;
	[SerializeField] private float _rotationSpeed = 45f;


	/// <summary> Followed object.</summary>
	public Transform TargetObject { get; set; }

	/// <summary> Followed point in world IF TargetObject is null.</summary>
	public Vector3 TargetPosition { private get; set; } = Vector3.zero;

	/// <summary> Follows this state of rotation.</summary>
	public float TargetRotation { private get; set; } = 0f;


	/// <summary> Returns the position in world where the camera is going. Returns TargetObject.Position if not null, else TargetPosition.</summary>
	public Vector3 GetTargetPosition => TargetObject ? TargetObject.position : TargetPosition;

	/// <summary> Returns the rotation in Y-axis where the camera is going. Returns TargetObject's rotation if not null, else TargetRotation.</summary>
	public float GetTargetRotation => TargetObject ? TargetObject.localEulerAngles.y : TargetRotation;


	// Remaining time until camera starts to follow _nextTarget.
	private float _delayTimer = 0f;
	// The object which will be followed after DelayTimer has reached 0.
	private Transform _nextTarget;


	/// <summary> Sets the given object as a target which the camera will start to follow. If delay is set, camera waits that before the new target is set.</summary>
	public void SetTargetObject(Transform target, float delay = 0f)
	{
		// If previous delay hasn't passed, directly set previous _nextTarget as target and don't update delay.
		// To reduce following a dead target in edge cases. Until better solution is found.
		if (_delayTimer > 0.01f)
		{
			TargetObject = _nextTarget;
			delay = _delayTimer;
		}

		_nextTarget = target;
		_delayTimer = delay;
	}

	protected override void Spawn()
	{
		// Reset target settings.
		TargetObject = null;
		TargetPosition = transform.position;
		TargetRotation = 0f;

		_nextTarget = null;
		_delayTimer = 0f;

		// Reset calculation variables.
		_rotationCalc = 0f;
		_rotationTracker = TargetRotation;
	}


	private void Update()
	{
		UpdateDelayedTarget();
		UpdatePosition();
		UpdateRotation();
	}

	/// <summary> Sets _nextTarget as TargetObject when _delayTimer reaches 0.</summary>
	private void UpdateDelayedTarget()
	{
		if (_delayTimer > 0f)
		{
			_delayTimer -= Time.deltaTime;
			if (_delayTimer <= 0f) { TargetObject = _nextTarget; }
		}
	}

	private void UpdatePosition()
	{ transform.position += (GetTargetPosition - transform.position) * Time.deltaTime * _movementSpeed; }


	// Tracks rotation between update frames.
	private float _rotationTracker = 0f;
	// To store values for rotation calculations.
	float _rotationCalc = 0f;

	private void UpdateRotation()
	{
		// Target's rotation within -360 to 360 scale.
		_rotationCalc = GetTargetRotation % 360f;

		// Calculate the shortest angle difference.
		_rotationCalc = Mathf.DeltaAngle(_rotationTracker, _rotationCalc);

		// Add rotation to rotationTracker and keep it within 360 degrees.
		_rotationTracker = Mathf.Repeat(_rotationTracker + (_rotationCalc * Time.deltaTime * _rotationSpeed), 360f);

		// Set to localEulerAngles.
		transform.localEulerAngles = Vector3.up * _rotationTracker;
	}


	/// <summary> Changes camera's screen size and position to given CameraProfile, which should be based on player amount.</summary>
	public void SetCameraProfile(CameraSplitProfile profile)
	{
		_camera.rect = profile.ViewportRect;
		_camera.fieldOfView = profile.FieldOfView;
	}
}
