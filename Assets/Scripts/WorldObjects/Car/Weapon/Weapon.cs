using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
	/// <summary> State is to track if using should go On or Off.</summary>
	public abstract void Use(bool state);
	public abstract void OnSpawn();
	public abstract void OnDespawn();
}