using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Damage
{
	public Damage(int damageAmount, DamageTeam team = DamageTeam.none, DamageType type = DamageType.general)
	{
		DamageAmount = damageAmount;
		Team = team;
		Type = type;
	}

	[Tooltip("Default amount of damage dealt to target before applying any effects or stats.")]
	[SerializeField] private float _damageAmount = 0f;

	[Tooltip("DamageTeam flag defines the team of the damage source. Same team wont take damage. DamageTeam.none deals damage to all teams.")]
	[SerializeField] private DamageTeam _team = DamageTeam.none;

	[Tooltip("DamageType is used to alter DamageAmount based on possible effects or stats.")]
	[SerializeField] private DamageType _type = DamageType.general;

	public float DamageAmount { get => _damageAmount; set => _damageAmount = value; }
	public DamageTeam Team { get => _team; set => _team = value; }
	public DamageType Type { get => _type; set => _type = value; }
}

public enum DamageType
{
	general = 0,
	collision = 1,
	fire = 2,
	water = 3,
}

[Flags]
public enum DamageTeam
{
	none = 0,
	danger = 1,
	fire = 2,
	water = 4,

	blueTeam = 16,
	redTeam = 32,
	yellowTeam = 64,
	greenTeam = 128,
}