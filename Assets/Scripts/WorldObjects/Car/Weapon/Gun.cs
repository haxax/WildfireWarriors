using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Weapon
{
	[Tooltip("Amount of Shoots per second.")]
	[SerializeField] protected float _rateOfFire = 1f;

	[Tooltip("If true, allows continuous shooting whithout the need to reuse.")]
	[SerializeField] protected bool _continuousFire = false;


	// Timer to track when new 'Shoot' should occur.
	private float _shootDelayTimer = 0f;

	/// <summary> If true, shooting will occur when _shootDelayTimer hits 0.</summary>
	private bool ShootOn { get; set; } = false;


	public override void OnSpawn()
	{
		// Reset shooting trackers.
		ShootOn = false;
		_shootDelayTimer = 0f;

		this.enabled = true;
	}

	public override void OnDespawn()
	{ this.enabled = false; }


	private void FixedUpdate()
	{
		// Shoot if ShootOn is enabled and _shootDelayTimer reaches 0.
		if (_shootDelayTimer > 0f)
		{ _shootDelayTimer -= Time.fixedDeltaTime; }

		if (ShootOn && _shootDelayTimer <= 0f)
		{
			_shootDelayTimer += _rateOfFire;
			if (!_continuousFire) { ShootOn = false; }
			Shoot();
		}
	}

	/// <summary> Enables/Disables shooting based on state.</summary>
	public override void Use(bool state)
	{ ShootOn = state; }

	// Define Shoot behaviour in upper class.
	protected abstract void Shoot();
}