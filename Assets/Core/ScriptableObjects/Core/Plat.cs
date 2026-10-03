using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plat", menuName = "Scriptable Objects/Plat")]
public class Plat : ScriptableObject
{
    public string Nom;
    
    public List<Plat> Ingredients;
}
