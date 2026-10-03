using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeSettings", menuName = "Scriptable Objects/TimeSettings")]
public class TimeSettings : ScriptableObject
{
    [Tooltip("Time (in seconds) before spawning an order")]
    public float orderSpawnInterval;

    [Tooltip("Time (in seconds) for the angry slider to go from full to empty")]
    public float angrySliderEmptyDuration = 120f;

    [Tooltip("Value added to the angry slider when RaiseAngrySlider is triggered")]
    public float angrySliderRaiseAmount;
}
