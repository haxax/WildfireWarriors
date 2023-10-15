using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : Poolable
{
	[SerializeField] private Health _health;

	[Space(10)]
	[Tooltip("At which point, if HP goes below by fire damage, tree spawns a new fire. Percentage of max HP.")]
	[SerializeField] private float _burnPoint = 0.2f;


	/// <summary> Can spawn fire only once per life. If Isburnt, fire is already spawned. </summary>
	private bool IsBurnt { get; set; } = false;


	private void Awake()
	{
		// Everytime forest takes damage, check if a fire should be spawned.
		_health.OnDamage.AddListener(TryBurn);

		// Call despawn when health goes to 0.
		_health.OnDeath.AddListener(Despawn);
	}

	protected override void Spawn()
	{
		// Reset the ability to spawn fire.
		IsBurnt = false;

		_health.OnSpawn();

		base.Spawn();
	}

	public override void Despawn()
	{
		_health.OnDespawn();
		base.Despawn();
	}

	/// <summary> Checks if fire should be spawned by taken damage.</summary>
	public void TryBurn(Health health, Damage damage)
	{
		// Start new fire only once per life.
		if (IsBurnt) { return; }

		// Damage type must be fire and amount positive.
		if (damage.Team != DamageTeam.fire) { return; }
		if (damage.DamageAmount <= 0f) { return; }

		// Spawn new fire if CurrentHealth is below BurnPoint.
		if (_health.CurrentHealth < _health.MaxHealth * _burnPoint)
		{
			IsBurnt = true;
			ObjectManager.Instance.Spawn(PoolableType.fire, transform.position);
		}
	}
}