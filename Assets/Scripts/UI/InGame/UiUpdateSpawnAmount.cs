using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary> Tracks wanted poolable object count and updates to a UI text element.</summary>
public class UiUpdateSpawnAmount : SpawnCountTracker
{
	[SerializeField] private TMP_Text _txt;
	public override void OnUpdateAmount(int amount)
	{ _txt.text = amount.ToString(); }
}
