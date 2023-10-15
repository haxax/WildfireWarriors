using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To load the correct game state and menu at the start of application.
/// </summary>
public class InitialMenuLoad : MonoBehaviour
{
	void Start()
	{
		GameManager.Instance.GoToMainMenu();
		Destroy(this);
	}
}
