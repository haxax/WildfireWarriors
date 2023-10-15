using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
	[Tooltip("Reference list of UI states. Automatically filled at the start of application.")]
	[SerializeField] private List<MenuState> _menuStates = new List<MenuState>();

	/// <summary> Currently open UI.</summary>
	public MenuState CurrentState { get; private set; }

	protected override void Awake()
	{
		FindMissingStates();
		base.Awake();
	}

	/// <summary> Finds and fills missing UI states to _menuStates.</summary>
	private void FindMissingStates()
	{
		// Get all defined UI states.
		MenuStateType[] validStates = (MenuStateType[])Enum.GetValues(typeof(MenuStateType));

		// Get all objects with MenuState component.
		MenuState[] existingMenus = (MenuState[])FindObjectsOfType(typeof(MenuState));

		// Check if found objects or object of same type is already listed in _menuStates, if not, add.
		bool found = false;
		for (int i = 0; i < validStates.Length; i++)
		{
			found = false;
			for (int j = 0; j < _menuStates.Count; j++)
			{
				if (_menuStates[j].StateType == validStates[i])
				{
					found = true;
					break;
				}
			}

			if (found) { continue; }
			else
			{
				for (int k = 0; k < existingMenus.Length; k++)
				{
					if (existingMenus[k].StateType == validStates[i])
					{
						_menuStates.Add(existingMenus[k]);
						break;
					}
				}
			}
		}
	}

	/// <summary> Enables a MenuState of given type. Disables previous one.</summary>
	public void SetMenuState(MenuStateType state)
	{
		// Ignore if same as previous.
		if (CurrentState?.StateType == state) { return; }

		// Disable previous UI.
		CurrentState?.Deactivate();

		// Find and activate the wanted UI.
		for (int i = 0; i < _menuStates.Count; i++)
		{
			if (_menuStates[i].StateType == state)
			{
				CurrentState = _menuStates[i];
				CurrentState.Activate();
				break;
			}
		}
	}
}