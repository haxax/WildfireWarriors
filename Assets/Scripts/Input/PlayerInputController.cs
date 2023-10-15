using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : InputController
{
	public PlayerInputController(PlayerControls controls)
	{ Controls = controls; }

	/// <summary> Inputs this InputController listens.</summary>
	public PlayerControls Controls { get; private set; }

	// To track previous input states.
	private float _prevSpeedState = 0f;
	private float _prevBrakeState = 0f;
	private float _prevSteerState = 0f;

	public override void UpdateInputs()
	{
		// Sends event every time button goes down and up, not between.
		if (Input.GetButtonDown(Controls.Shoot)) { Shoot(true); }
		else if (Input.GetButtonUp(Controls.Shoot)) { Shoot(false); }

		if (Input.GetButtonDown(Controls.Roll)) { Roll(true); }
		else if (Input.GetButtonUp(Controls.Roll)) { Roll(false); }

		if (Input.GetButtonDown(Controls.Special)) { Special(true); }
		else if (Input.GetButtonUp(Controls.Special)) { Special(false); }

		// Sends event every time axis state changes, not if same as previously.
		if (Input.GetAxis(Controls.SpeedAxis) != _prevSpeedState)
		{
			_prevSpeedState = Input.GetAxis(Controls.SpeedAxis);
			Speed(_prevSpeedState);
		}

		if (Input.GetAxis(Controls.BrakeAxis) != _prevBrakeState)
		{
			_prevBrakeState = Input.GetAxis(Controls.BrakeAxis);
			Brake(_prevBrakeState);
		}

		if (Input.GetAxis(Controls.SteerAxis) != _prevSteerState)
		{
			_prevSteerState = Input.GetAxis(Controls.SteerAxis);
			Steer(_prevSteerState);
		}
	}
}

/// <summary> Set of Controls defined in editor's InputManager. To be listened from Input.</summary>
[System.Serializable]
public class PlayerControls
{
	public PlayerControls(PlayerControls c)
	{
		_shoot = c._shoot;
		_roll = c._roll;
		_special = c._special;
		_speedAxis = c._speedAxis;
		_brakeAxis = c._brakeAxis;
		_steerAxis = c._steerAxis;
	}

	// Controls this PlayerControls would be used to listen. Same ones should be defined in editor's InputManager.

	[SerializeField] private string _shoot = "¤Shoot$";
	[SerializeField] private string _roll = "¤Roll$";
	[SerializeField] private string _special = "¤Special$";

	[Space(10)]
	[SerializeField] private string _speedAxis = "¤Speed$";
	[SerializeField] private string _brakeAxis = "¤Brake$";
	[SerializeField] private string _steerAxis = "¤Steer$";

	public string Shoot => _shoot;
	public string Roll => _roll;
	public string Special => _special;
	public string SpeedAxis => _speedAxis;
	public string BrakeAxis => _brakeAxis;
	public string SteerAxis => _steerAxis;




	/// <summary> Replaces $ symbols with joystickId and ¤ symbols with J.</summary>
	public void AssignToJoystick(int joystickId)
	{ AssignControls("J", joystickId); }

	/// <summary> Replaces $ symbols with keyboardId and ¤ symbols with K.</summary>
	public void AssignToKeyboard(int keyboardId)
	{ AssignControls("K", keyboardId); }

	/// <summary> Changes generic format to a id and source specific format. K = keyboard, J = joystick, id = number of joystick or keyboard.</summary>
	private void AssignControls(string inputChar, int id)
	{
		_shoot = _shoot.Replace("¤", $"{inputChar}").Replace("$", $"{id}");
		_roll = _roll.Replace("¤", $"{inputChar}").Replace("$", $"{id}");
		_special = _special.Replace("¤", $"{inputChar}").Replace("$", $"{id}");
		_speedAxis = _speedAxis.Replace("¤", $"{inputChar}").Replace("$", $"{id}");
		_brakeAxis = _brakeAxis.Replace("¤", $"{inputChar}").Replace("$", $"{id}");
		_steerAxis = _steerAxis.Replace("¤", $"{inputChar}").Replace("$", $"{id}");
	}
}