using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Intended for quick play once type of particle effects, to automatically return back to pool.
/// </summary>
public class OneTimeEffect : Poolable
{
	[SerializeField] private ParticleSystem _particleSystem;

	private void Start()
	{
		// Make sure particle system is reset when returning to pool.
		OnReturnToPool.AddListener(x => _particleSystem.Stop());
	}

	protected override void Spawn()
	{
		_particleSystem.Play();
		base.Spawn();
	}

	private void FixedUpdate()
	{
		// Automatically despawn the effect if stops playing.
		if (!_particleSystem.isPlaying)
		{ Despawn(); }
	}
}
