using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple component to continuously rotate an object.
/// </summary>
public class Rotator : MonoBehaviour
{
	[SerializeField] private float _speed = 1.0f;
	[SerializeField] private Vector3 _rotateVector = Vector3.zero;
	[SerializeField] private AnimationCurve _curve = new AnimationCurve();

	private float _timer = 0f;

	void Update()
	{
		_timer += Time.deltaTime * _speed;
		if (_timer >= 1f) { _timer -= 1f; }

		transform.localEulerAngles = _rotateVector * _curve.Evaluate(_timer);
	}
}
