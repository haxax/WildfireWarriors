using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
	[SerializeField] private Collider _healthBox;

	[SerializeField] private float _maxHealth = 100f;

	[Tooltip("Damage from same teams are ignored. If set to none, take damage from any team.")]
	[SerializeField] private DamageTeam _team = DamageTeam.blueTeam;

	[Space(10)]
	[Tooltip("Invoked whenever health state changes")]
	[SerializeField] public UnityEvent<Health> OnUpdateHealth = new UnityEvent<Health>();

	[Tooltip("Invoked whenever damage is dealt. Health = this component, Damage = damage amount from the source.")]
	[SerializeField] public UnityEvent<Health, Damage> OnDamage = new UnityEvent<Health, Damage>();

	[Tooltip("Invoked whenever health goes to zero or less.")]
	[SerializeField] public UnityEvent OnDeath = new UnityEvent();

	private float _currentHealth = -1f;

	public float MaxHealth { get { return _maxHealth; } }
	public DamageTeam Team { get { return _team; } }
	public float CurrentHealth
	{
		get { return _currentHealth; }
		private set
		{
			if (_currentHealth == value) { return; }
			_currentHealth = Mathf.Clamp(value, 0f, MaxHealth);
			OnUpdateHealth.Invoke(this);
		}
	}


	void Start()
	{
		// Health Should never be trigger. DamageSource is trigger.
		_healthBox.isTrigger = false;

		// Health should always be on layer 20 (Health). Health layer (20) only collides with Damage layer (21).
		gameObject.layer = 20;
	}

	public void OnSpawn()
	{
		CurrentHealth = MaxHealth;
		_healthBox.enabled = true;
	}

	public void OnDespawn()
	{ _healthBox.enabled = false; }

	/// <summary>
	/// Deals damage based on given Damage. Returns the dealt amount of damage.
	/// </summary>
	public float DealDamage(Damage damage)
	{
		// If team is set to none, always ignore team settings.
		// If health has same team as damage, ignore damage.
		if (Team.HasFlag(DamageTeam.none) && (Team & damage.Team) > 0) { return 0f; }

		// Calculate, how much damage is dealt.
		float dealtDamage = Mathf.Clamp(damage.DamageAmount, 0f, CurrentHealth);

		// Apply damage, invokes health amount update event.
		CurrentHealth -= dealtDamage;

		// Invoke damage amount event, mainly for effects.
		OnDamage.Invoke(this, damage);

		// Check if dead.
		if (CurrentHealth <= 0f) { OnDeath.Invoke(); }

		return dealtDamage;
	}
}
