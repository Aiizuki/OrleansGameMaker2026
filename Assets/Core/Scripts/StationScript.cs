using Core.Enums;
using UnityEngine;

public enum StationType
{
    Washing,
    Cooking,
    Dressing
}

public class StationScript : MonoBehaviour
{
    public StationType stationType;
    
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
    
    public EnumDishStatus getStationStatus()
    {
        switch (stationType)
        {
            case StationType.Washing:
                return EnumDishStatus.Washed;
            case StationType.Cooking:
                return EnumDishStatus.Cooked;
            case StationType.Dressing:
                return EnumDishStatus.Dressed;
        }
        return  EnumDishStatus.Raw;
    }
}
