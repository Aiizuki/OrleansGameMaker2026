using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "TimeSettings", menuName = "Scriptable Objects/TimeSettings")]
public class TimeSettings : ScriptableObject
{
    [FormerlySerializedAs("orderSpawnInterval")] [Tooltip("Time (in seconds) before spawning an order")]
    public float OrderSpawnInterval;

    [FormerlySerializedAs("angrySliderEmptyDuration")] [Tooltip("Time (in seconds) for the angry slider to go from full to empty")]
    public float AngrySliderEmptyDuration = 120f;
}
