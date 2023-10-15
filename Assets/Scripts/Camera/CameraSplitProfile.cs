using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraSplit", menuName = "Custom/CameraSplit", order = 200)]
public class CameraSplitProfile : ScriptableObject
{
	[Range(0.00001f, 179f)][SerializeField] private float _fieldOfView = 60f;
	[SerializeField] private Rect _viewportRect = new Rect(0f, 0f, 1f, 1f);

	public float FieldOfView => _fieldOfView;
	public Rect ViewportRect => _viewportRect;
}