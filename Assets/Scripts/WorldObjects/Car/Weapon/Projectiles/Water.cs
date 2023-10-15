using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : Poolable, IProjectile
{
	[SerializeField] private DamageSource _damageSource;
	[SerializeField] private Rigidbody _rb;
	[SerializeField] private Collider _collider;
	[SerializeField] private ParticleSystem _particleSystem;

	public Rigidbody Rigidbody { get => _rb; private set => _rb = value; }

	private void Awake()
	{
		// Update stuff if deals damage.
		_damageSource.OnDamage.AddListener(OnDamage);
	}

	protected override void Spawn()
	{
		// Restore reduced damages.
		_damageSource.Damage.DamageAmount += _totalDealtDamage;
		_totalDealtDamage = 0f;

		// Reset Rigidbody.
		_rb.isKinematic = false;
		_rb.velocity = Vector3.zero;
		_rb.angularVelocity = Vector3.zero;
		_rb.useGravity = true;

		// Enable colliers.
		_damageSource.Collider.enabled = true;
		_collider.enabled = true;

		// Play particles.
		_particleSystem.Play();

		base.Spawn();
	}

	public override void Despawn()
	{
		// Disable colliders.
		_damageSource.Collider.enabled = false;
		_collider.enabled = false;

		// Disable Rigidbody.
		_rb.isKinematic = true;
		_rb.useGravity = false;

		// Stop Particles.
		_particleSystem.Stop();

		base.Despawn();
	}

	// Total amount of damage during life.
	private float _totalDealtDamage = 0f;
	private void OnDamage(float dealtDamage)
	{
		// Count total damage and reduce dealtDamage from DamageSource. Allow possible remaining damage to be dealt to other targets.
		_totalDealtDamage += dealtDamage;
		_damageSource.Damage.DamageAmount -= dealtDamage;

		// Despawn if all damage is consumed.
		if (_damageSource.Damage.DamageAmount <= 0f) { Despawn(); }
	}


	public void Shoot(Vector3 force)
	{
		Rigidbody.AddForce(force, ForceMode.Impulse);
	}


	private void OnCollisionEnter(Collision collision)
	{
		// Despawn if collider (not damage collider) hits ground.
		if (_collider != null) { Despawn(); }
	}
}