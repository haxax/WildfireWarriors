using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class InputSetup
{
	[Tooltip("Maximum amount of players the game can handle.")]
	[SerializeField] private int _supportedPlayerCount = 4;
	public int SupportedPlayerCount { get => _supportedPlayerCount; private set => _supportedPlayerCount = value; }

	[Space(10)]
	[Tooltip("Each control should be named: ¤ + Function Name + $.\nFor shooting: ¤Shoot$")]
	[SerializeField] private PlayerControls _controlsTemplate;


	/// <summary> Loads the controllers at the start of application.</summary>
	public void SetupControls()
	{
		List<PlayerControls> controlProfiles = new List<PlayerControls>();

		SetupJoysticks(controlProfiles);

		SetupKeyboards(controlProfiles);

		// Adjust SupportedPlayerCount if setup fails to set required amount of controlProfiles.
		if (controlProfiles.Count < SupportedPlayerCount)
		{ SupportedPlayerCount = controlProfiles.Count; }

		// Create InputController for each controlProfile.
		InputManager.Instance.Controllers = new List<PlayerInputController>();
		for (int i = 0; i < controlProfiles.Count; i++)
		{
			InputManager.Instance.Controllers.Add(new PlayerInputController(controlProfiles[i]));
		}
	}

	/// <summary> Gets connected joysticks and creates control profiles for them.</summary>
	private void SetupJoysticks(List<PlayerControls> controlProfiles)
	{
		// Get all connected joysticks.
		List<string> joysticks = Input.GetJoystickNames().ToList();

		for (int i = 0; i < joysticks.Count; i++)
		{
			// Ignore invalid connections.
			if (joysticks[i] == "") { continue; }

			// Register valid joystick.
			controlProfiles.Add(new PlayerControls(_controlsTemplate));
			controlProfiles[controlProfiles.Count - 1].AssignToJoystick(i + 1);

			// Register only as many joysticks as the game supports players.
			if (controlProfiles.Count == SupportedPlayerCount) { break; }
		}
	}

	/// <summary> Creates control profiles for missing players.</summary>
	private void SetupKeyboards(List<PlayerControls> controlProfiles)
	{
		// Fill the missing ControlProfile slots with keyboard controls.
		for (int i = 0; controlProfiles.Count < SupportedPlayerCount; i++)
		{
			controlProfiles.Add(new PlayerControls(_controlsTemplate));
			controlProfiles[controlProfiles.Count - 1].AssignToKeyboard(i + 1);
		}
	}
}