using UnityEngine;

public class StationScript : MonoBehaviour
{
    [Header("IA Waypoints")]
    public GameObject StationInput;
    public GameObject StationInventory;
    public GameObject StationOutput;
    
    [Space(10)]
    [Header("Objects Places")]
    public GameObject placeAtInput;
    public GameObject placeAtInventory;
    
    [Space(10)]
    [Header("Objects Variables")]
    public GameObject objectAtInput;
    public GameObject objectAtInventory;
    public GameObject objectAtOutput;
}
