using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> To debug if UI element is able to call Test.</summary>
public class UiDebug : MonoBehaviour
{
	[SerializeField] private string _debugTxt = "";

	public void Test() { Debug.Log($"{gameObject.name} {_debugTxt}"); }
}