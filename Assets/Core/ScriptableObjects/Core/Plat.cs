using System.Collections.Generic;
using Core.Enums;
using UnityEngine;

[CreateAssetMenu(fileName = "Plat", menuName = "Scriptable Objects/Plat")]
public class Plat : ScriptableObject
{
    [Header("Common Settings")]
    public string Nom;
    public Sprite Icon;
    
    [Header("Dish Settings (not used for single ingredients)")]
    public List<Plat> Ingredients;
}
