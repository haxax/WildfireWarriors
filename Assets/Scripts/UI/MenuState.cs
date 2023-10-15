using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary> Core component of a menu state. UI should have one MenuState for each MenuStateType</summary>
public class MenuState : MonoBehaviour
{
	[Tooltip("The type of this MenuState. Should be unique in the scene.")]
	[SerializeField] MenuStateType _stateType = MenuStateType.none;

	[Tooltip("If set, this Selectable is selected when this UI becomes active.")]
	[SerializeField] private Selectable _selectByDefault;

	[Space(10)]
	[SerializeField] private UnityEvent onActivate = new UnityEvent();
	[SerializeField] private UnityEvent onDeactivate = new UnityEvent();

	public MenuStateType StateType => _stateType;


	private void Awake()
	{ gameObject.SetActive(false); }

	public void Activate()
	{
		gameObject.SetActive(true);
		_selectByDefault?.Select();
		onActivate.Invoke();
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
		onDeactivate.Invoke();
	}

}

public enum MenuStateType
{
	none = 0,
	mainMenu = 1,
	gameUI = 2,
	gameEndScreen = 3,
	info = 4,
	customLevel = 5,
	backToGameScreen = 6,
}