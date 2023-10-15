using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fire : Poolable
{
	[SerializeField] private DamageSource _damageSource;
	[SerializeField] private Health _health;
	[SerializeField] private FireParticles _particles;

	[Space(10)]
	[Tooltip("Deals damage to objects within range this many times per second.")]
	[SerializeField] private float _hitsPerSecond = 1f;

	[Tooltip("Fire is despawned if BurnDuration is reached.")]
	[Min(0.0001f)][SerializeField] private float _burnDuration = 120f;

	[Tooltip("Fire scales from 1 to 1 + GrowAmount within GrowTime.")]
	[Min(0.0001f)][SerializeField] private float _growTime = 1f;

	[Tooltip("The maximum scale amount added to the fire: final size is 1 + GrowAmount.")]
	[SerializeField] private float _growAmount = 1f;

	[Tooltip("Fire grows based on the curve state. Time scale should be 0 to 1.")]
	[SerializeField] private AnimationCurve _growCurve = new AnimationCurve();


	// Timers to track various states.
	private float _hitTimer = 1f;
	private float _growTimer = 1f;
	private float _burnTimer = 1f;

	private void Awake()
	{
		// Set Particles to change based on health amount.
		_health.OnUpdateHealth.AddListener(_particles.UpdateHealthState);

		// Call despawn when health goes to 0.
		_health.OnDeath.AddListener(Despawn);
	}

	protected override void Spawn()
	{
		transform.localScale = Vector3.one;

		_health.OnSpawn();
		_particles.OnSpawn();

		// Reset timers.
		_hitTimer = 1f;
		_growTimer = _growTime;
		_burnTimer = _burnDuration;

		// Enable damage and growing.
		_damageSource.Collider.enabled = true;
		this.enabled = true;

		base.Spawn();
	}

	public override void Despawn()
	{

		// Disable damage and growing while despawning.
		_damageSource.Collider.enabled = false;
		this.enabled = false;

		_health.OnDespawn();
		_particles.OnDespawn();

		base.Despawn();
	}

	private void FixedUpdate()
	{
		// Blink the collider each time the timer hits 0 to cause new trigger.
		_hitTimer -= Time.fixedDeltaTime * _hitsPerSecond;
		if (_hitTimer <= 0f)
		{
			_hitTimer += 1f;
			_damageSource.Collider.enabled = false;
			_damageSource.Collider.enabled = true;
		}

		// Grow fire size for the given time.
		if (_growTimer > 0f)
		{
			_growTimer -= Time.fixedDeltaTime;
			if (_growTimer < 0f) { _growTimer = 0f; }

			// Scale based on GrowTimer, GrowCurve and GrowAmount.
			transform.localScale = Vector3.one * (1f + (_growCurve.Evaluate(1f - (_growTimer / _growTime)) * _growAmount));

			// Update particles to match the size of fire.
			_particles.UpdateParticles(transform.localScale.x);
		}

		// Despawn by killing is BurnTimer reaches 0.
		_burnTimer -= Time.fixedDeltaTime;
		if (_burnTimer <= 0f)
		{
			_health.DealDamage(new Damage(9999999));
		}
	}
}