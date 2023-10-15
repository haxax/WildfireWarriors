using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageSource : MonoBehaviour
{
	[SerializeField] private Collider _collider;
	[SerializeField] private Damage _damage;

	[Tooltip("Called whenever DamageSource deals damage (more than 0). Comes with the actual dealt damage amount.")]
	[SerializeField] public UnityEvent<float> OnDamage = new UnityEvent<float>();

	public Collider Collider => _collider;
	public Damage Damage { get { return _damage; } }

	private void Start()
	{
		// DamageSource Should alway be trigger. Health is not trigger.
		_collider.isTrigger = true;

		// DamageSource should always be on layer 21 (Damage). Damage layer (21) only collides with Health layer (20).
		gameObject.layer = 21;
	}

	// For trigger calculations.
	private float _previouslyDealtDamage = 0f;

	protected virtual void OnTriggerEnter(Collider other)
	{
		Health hp = other.GetComponent<Health>();
		if (hp == null) { return; }

		// Try to deal damage to the target. Returns the amount of damage dealt.
		_previouslyDealtDamage = hp.DealDamage(Damage);

		if (_previouslyDealtDamage > 0f)
		{ OnDamage.Invoke(_previouslyDealtDamage); }
	}
}
