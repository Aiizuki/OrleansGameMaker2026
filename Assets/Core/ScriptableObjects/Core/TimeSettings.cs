using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeSettings", menuName = "Scriptable Objects/TimeSettings")]
public class TimeSettings : ScriptableObject
{
    [Tooltip("Time (in seconds) before spawning an order")]
    public float orderSpawnInterval;
}
