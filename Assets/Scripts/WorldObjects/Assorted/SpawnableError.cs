using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Error object if spawning something else fails.</summary>
public class SpawnableError : Poolable
{
	[SerializeField] private Transform _model;

	[Space(10)]
	[SerializeField] private float _rotationSpeed = 1.0f;
	[SerializeField] private float _heightMultiplier = 1.0f;
	[SerializeField] private AnimationCurve _heightCurve = new AnimationCurve();

	private float _timer = 0.0f;

	void Update()
	{
		// Rotate and float.

		_timer += Time.deltaTime * _rotationSpeed;
		if (_timer >= 1.0f) { _timer -= 1.0f; }

		_model.localEulerAngles = new Vector3(0f, 360f * _timer, 0f);
		_model.localPosition = new Vector3(0f, _heightCurve.Evaluate(_timer) * _heightMultiplier);
	}
}
