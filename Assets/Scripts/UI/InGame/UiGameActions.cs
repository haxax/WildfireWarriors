using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiGameActions : MonoBehaviour
{
	/// <summary> Opens pause window and allow return to main menu UI state.</summary>
	public void OpenBackToMenuUI()
	{ MenuManager.Instance.SetMenuState(MenuStateType.backToGameScreen); }
}
