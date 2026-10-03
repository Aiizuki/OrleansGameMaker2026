using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CoreGameSettings", menuName = "Scriptable Objects/CoreGameSettings")]
public class CoreGameSettings : ScriptableObject
{
    [Tooltip("Define all foods which can be ordered")]
    public List<Plat> LstAvailablePlats;
    
    [Header("Player Config")]
    [Tooltip("Define the player speed")]
    public float Speed = 10f;
    
    [Tooltip("Define the player spawn position")]
    public Vector3 SpawnPosition;
}
