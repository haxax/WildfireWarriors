using System.Collections.Generic;
using UnityEngine;

// Simple nested list to be inspectable in Unity's inspector.

[System.Serializable]
public class NestedList<T>
{
    [SerializeField] public List<T> _list = new List<T>();
    public List<T> List { get => _list; set => _list = value; }
}