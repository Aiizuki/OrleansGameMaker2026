using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CoreGameSettings", menuName = "Scriptable Objects/CoreGameSettings")]
public class CoreGameSettings : ScriptableObject
{
    [Tooltip("Define all foods which can be ordered")]
    public List<Plat> LstAvailablePlats;

    [Header("Player Config")] [Tooltip("Define the player speed")]
    public float Speed = 10f;

    [Tooltip("Define the player spawn position")]
    public Vector3 SpawnPosition = new Vector3(0f, -1.224f, -5.88f);

    [Header("Global Game Settings")]
    [Tooltip("Value added to the angry slider when RaiseAngrySlider is triggered")]
    public float AngrySliderRaiseAmount;

    [Tooltip("Value subtracted to the angry slider when DecreaseAngrySlider is triggered")]
    public float AngrySliderDicreaseAmount;

    [Tooltip("How much ingredients a player can pick at max")]
    public int MaxPickableIngredients = 5;
}