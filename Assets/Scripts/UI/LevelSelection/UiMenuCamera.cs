using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Switched on when entering main menu, and off in game.</summary>
public class UiMenuCamera : MonoBehaviour
{
	public void Awake()
	{
		GameManager.Instance.OnMainMenu += (() => gameObject.SetActive(true));
		GameManager.Instance.OnStartGame += (() => gameObject.SetActive(false));
	}
}
