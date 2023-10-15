using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : Poolable
{
	[SerializeField] private CarInput _input;
	[SerializeField] private CarEngine _engine;
	[SerializeField] private Health _health;
	[SerializeField] private Weapon _weapon;
	[SerializeField] private CarBurnParticles _burnParticles;

	[Space(10)]
	[Tooltip("Prefab, spawned when car despawns.")]
	[SerializeField] private Poolable _carExplosion;

	public CarInput Input { get { return _input; } }
	public CarEngine Engine { get { return _engine; } }
	public Health Health { get { return _health; } }
	public Weapon Weapon { get { return _weapon; } }



	private void Awake()
	{
		// Connect components to InputReceiver.
		Input.OnSpeed += Engine.SetMotorState;
		Input.OnBrake += Engine.SetBrakeState;
		Input.OnSteer += Engine.SetSteerState;
		if (Weapon != null) { Input.OnShoot += Weapon.Use; }

		// Respawn when Roll is pressed.
		Input.OnRoll += (x => { if (x) { Despawn(); } });

		// Set BurnParticles to change based on health amount.
		Health.OnUpdateHealth.AddListener(_burnParticles.UpdateHealthState);

		// Call despawn when health goes to 0.
		Health.OnDeath.AddListener(Despawn);
	}

	protected override void Spawn()
	{
		Health.OnSpawn();
		Engine.OnSpawn();
		Weapon?.OnSpawn();
		_burnParticles.OnSpawn();

		base.Spawn();
	}

	public override void Despawn()
	{
		// Unsubscribe from controllers if connected. Controllers should connect to a new car.
		Input.Unsubscribe();

		Health.OnDespawn();
		Engine.OnDespawn();
		Weapon?.OnDespawn();
		_burnParticles.OnDespawn();

		// Spawn explosion effect.
		_carExplosion.GetFromPool(transform.position, Quaternion.identity);

		base.Despawn();
	}
}