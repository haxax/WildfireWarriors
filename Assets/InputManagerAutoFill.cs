using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// This script is made to fasten the setup of InputManager and doesn't work in builds.
// Add this component to a empty GameObject, set inputs in inspector and call "Update InputManager" via context menu.
// The GameObject can be deleted from a build.

public class InputManagerAutoFill : MonoBehaviour
{
	[Tooltip("The amount of joysticks added to the InputManager.")]
	[SerializeField] private int _presetInputsAmount = 8;

	[Space(20)]
	[SerializeField] private int _joystickSpeedAxis = 10;
	[SerializeField] private int _joystickBrakeAxis = 9;
	[SerializeField] private int _joystickSteerAxis = 1;
	[Space(10)]
	[SerializeField] private int _joystickShootButton = 7;
	[SerializeField] private int _joystickRollButton = 3;
	[SerializeField] private int _joystickSpecialButton = 6;

	[Space(20)]
	[Tooltip("Player specific controls. Should setup as many as the client can support players.")]
	[SerializeField] private List<KeyboardControls> _keyboardControls = new List<KeyboardControls>();

	[Space(20)]
	[Tooltip("Generic, non-player specific, buttons which are read from all controllers and or keyboard.")]
	[SerializeField] private List<InputButton> _uniqueButtons = new List<InputButton>();

	[System.Serializable]
	public class KeyboardControls
	{
		[SerializeField] private KeyCode _forward = KeyCode.W;
		[SerializeField] private KeyCode _backward = KeyCode.S;
		[SerializeField] private KeyCode _steerLeft = KeyCode.A;
		[SerializeField] private KeyCode _steerRight = KeyCode.D;
		[SerializeField] private KeyCode _shoot = KeyCode.E;
		[SerializeField] private KeyCode _roll = KeyCode.Q;
		[SerializeField] private KeyCode _special = KeyCode.R;

		public string Forward => Convert(_forward);
		public string Backward => Convert(_backward);
		public string SteerLeft => Convert(_steerLeft);
		public string SteerRight => Convert(_steerRight);
		public string Shoot => Convert(_shoot);
		public string Roll => Convert(_roll);
		public string Special => Convert(_special);

		public string Convert(KeyCode code)
		{
			// Convert Keycode format into format which InputManager understands.
			string result = code.ToString();

			// "Up Arrow" -> "Up"
			if (result.Contains("Arrow")) { result = result.Replace("Arrow", ""); }
			// "Keypad 0" -> "[0]"
			if (result.Contains("Keypad")) { result = result.Replace("Keypad", "[") + "]"; }
			// "Alpha 0" -> 0
			if (result.Contains("Alpha")) { result = result.Replace("Alpha", ""); }

			// InputManager is case sensitive and requires lower.
			return result.ToLower();
		}
	}



	[System.Serializable]
	public class InputButton
	{
		[SerializeField] private string _name = "";
		[SerializeField] private string _positiveButton = "";
		[SerializeField] private string _altPositiveButton = "";
		[SerializeField] private string _negativeButton = "";
		[SerializeField] private string _altNegativeButton = "";
		[SerializeField] private int _type = 0;
		[SerializeField] private int _joyAxis = 0;
		[SerializeField] private int _joyNum = 0;
		[SerializeField] private float _gravity = 1000f;
		[SerializeField] private float _dead = 0.001f;
		[SerializeField] private float _sensitivity = 1000f;
		[SerializeField] private bool _snap = false;
		[SerializeField] private bool _invert = false;


		public string Name => _name;
		public string PositiveButton => _positiveButton;
		public string AltPositiveButton => _altPositiveButton;
		public string NegativeButton => _negativeButton;
		public string AltNegativeButton => _altNegativeButton;
		public int Type => _type;
		public int JoyAxis => _joyAxis;
		public int JoyNum => _joyNum;
		public float Gravity => _gravity;
		public float Dead => _dead;
		public float Sensitivity => _sensitivity;
		public bool Snap => _snap;
		public bool Invert => _invert;
	}

#if UNITY_EDITOR

	[ContextMenu("Update InputManager")]
	public void AutoFill()
	{
		// Get InputManager.
		SerializedObject inputManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		SerializedProperty axesArray = inputManager.FindProperty("m_Axes");

		// Remove existing inputs.
		axesArray.ClearArray();

		// Add unique buttons.
		for (int i = 0; i < _uniqueButtons.Count; i++)
		{
			AddInputAxis(axesArray, _uniqueButtons[i].Name, _uniqueButtons[i].PositiveButton, _uniqueButtons[i].NegativeButton,
				_uniqueButtons[i].AltPositiveButton, _uniqueButtons[i].AltNegativeButton,
				_uniqueButtons[i].Type, _uniqueButtons[i].JoyAxis, _uniqueButtons[i].JoyNum,
				_uniqueButtons[i].Gravity, _uniqueButtons[i].Dead, _uniqueButtons[i].Sensitivity,
				_uniqueButtons[i].Snap, _uniqueButtons[i].Invert);
		}

		// Add joysticks to InputManager.
		for (int i = 1; i <= _presetInputsAmount; i++)
		{
			string joystickButton = $"joystick {i} button $"; // The format Unity's InputManager reads joystick buttons. {i} = joystick number, Replace $ with button number.

			AddJoystickAxis(axesArray, $"JSpeed{i}", i, _joystickSpeedAxis - 1); // Joystick axes are named id - 1. 10th axis is 9  for example.
			AddJoystickAxis(axesArray, $"JBrake{i}", i, _joystickBrakeAxis - 1);
			AddJoystickAxis(axesArray, $"JSteer{i}", i, _joystickSteerAxis - 1);

			AddJoystickButton(axesArray, $"JShoot{i}", i, joystickButton.Replace("$", $"{_joystickShootButton}"));
			AddJoystickButton(axesArray, $"JRoll{i}", i, joystickButton.Replace("$", $"{_joystickRollButton}"));
			AddJoystickButton(axesArray, $"JSpecial{i}", i, joystickButton.Replace("$", $"{_joystickSpecialButton}"));
		}

		// Add keyboards to InputManager
		for (int i = 0; i < _keyboardControls.Count; i++)
		{
			AddKeyboardAxis(axesArray, i);
		}

		inputManager.ApplyModifiedProperties();
	}
	private void AddKeyboardAxis(SerializedProperty axesArray, int id)
	{
		AddInputAxis(axesArray, $"KSpeed{id + 1}", _keyboardControls[id].Forward, "");
		AddInputAxis(axesArray, $"KBrake{id + 1}", _keyboardControls[id].Backward, "");
		AddInputAxis(axesArray, $"KSteer{id + 1}", _keyboardControls[id].SteerLeft, _keyboardControls[id].SteerRight, "", "", 0, 0, 0, 1000, 0.001f, 1000, false, true);
		AddInputAxis(axesArray, $"KShoot{id + 1}", _keyboardControls[id].Shoot, "");
		AddInputAxis(axesArray, $"KRoll{id + 1}", _keyboardControls[id].Roll, "");
		AddInputAxis(axesArray, $"KSpecial{id + 1}", _keyboardControls[id].Special, "");
	}

	private void AddJoystickAxis(SerializedProperty axesArray, string name, int joystickId, int axis)
	{
		AddInputAxis(axesArray, name, "", "", "", "", 2, axis, joystickId, 0f, 0.19f, 1f); // 0f, 0.19f and 1f are default values by unity, expose if needed to be changed.
	}
	private void AddJoystickButton(SerializedProperty axesArray, string name, int joystickId, string positive)
	{
		AddInputAxis(axesArray, name, positive, "", "", "", 0, 0, joystickId);
	}

	/// <summary>
	/// Preset float values are for button behaviour (default values by Unity, expose if change needed), int values for keyboard.
	/// </summary>
	private void AddInputAxis(SerializedProperty axesArray, string name, string positive, string negative, string altPositive = "", string altNegative = "",
		int type = 0, int jAxis = 0, int joyNum = 0,
		float gravity = 1000, float dead = 0.001f, float sensitivity = 1000, bool snap = false, bool invert = false)
	{
		axesArray.InsertArrayElementAtIndex(axesArray.arraySize);
		SerializedProperty axis = axesArray.GetArrayElementAtIndex(axesArray.arraySize - 1);

		axis.FindPropertyRelative("m_Name").stringValue = name;
		axis.FindPropertyRelative("descriptiveName").stringValue = name;
		axis.FindPropertyRelative("positiveButton").stringValue = positive;
		axis.FindPropertyRelative("negativeButton").stringValue = negative;
		axis.FindPropertyRelative("altPositiveButton").stringValue = altPositive;
		axis.FindPropertyRelative("altNegativeButton").stringValue = altNegative;

		axis.FindPropertyRelative("gravity").floatValue = gravity;
		axis.FindPropertyRelative("dead").floatValue = dead;
		axis.FindPropertyRelative("sensitivity").floatValue = sensitivity;
		axis.FindPropertyRelative("type").intValue = type;
		axis.FindPropertyRelative("axis").intValue = jAxis;
		axis.FindPropertyRelative("snap").boolValue = snap;
		axis.FindPropertyRelative("invert").boolValue = invert;
		axis.FindPropertyRelative("joyNum").intValue = joyNum;
	}
#endif
}
