using UnityEngine;
using UnityEngine.Serialization;

public class IaManager : MonoBehaviour
{
    [Header("IAs")]
    public GameObject cookingIA;
    public GameObject cleaningIA;
    public GameObject dressingIA;
    
    [FormerlySerializedAs("CookingStation")]
    [Space(10)]
    [Header("Stations")]
    public GameObject cookingStation;
    public GameObject cleaningStation;
    public GameObject dressingStation;
    public GameObject pickupStations;
}

