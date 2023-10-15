using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiMainMenuActions : MonoBehaviour
{
	public void Play()
	{ GameManager.Instance.StartLevel(); }

	public void Info()
	{ MenuManager.Instance.SetMenuState(MenuStateType.info); }

	public void CustomLevel()
	{ MenuManager.Instance.SetMenuState(MenuStateType.customLevel); }

	public void Exit()
	{ Application.Quit(); }
}
