using System;
using System.Collections.Generic;
using Core.Enums;
using UnityEngine;

[CreateAssetMenu(fileName = "Plat", menuName = "Scriptable Objects/Plat")]
public class Plat : ScriptableObject
{
    [Header("Common Settings")]
    public string Nom;
    public Sprite Icon;
        
    // Modèles FBX complets (mesh + matériaux + transform d'import) : un simple Mesh perdrait
    // les matériaux, les sous-objets et la rotation d'import du FBX
    [SerializeField] private GameObject _modelRaw;
    [SerializeField] private GameObject _modelWashed;
        [SerializeField] private GameObject _modelButchered;
    [SerializeField] private GameObject _modelCooked;
    [SerializeField] private GameObject _modelOvercooked;
    [SerializeField] private GameObject _modelDressed;
    [SerializeField] private GameObject _modelRuined;
    [SerializeField] private GameObject _modelFailed;

    [Header("Dish Settings (not used for single ingredients)")]
    public List<Plat> Ingredients;

    public GameObject GetModelFromState(bool failedDish, EnumDishStatus status, bool overFailed = false)
    {
        if (overFailed)
            return _modelFailed;

        switch (status)
        {
            case EnumDishStatus.Raw:
                return _modelRaw;
            case EnumDishStatus.Cutted:
                return failedDish ? _modelButchered : _modelWashed;
            case EnumDishStatus.Cooked:
                return failedDish ? _modelOvercooked : _modelCooked;
            case EnumDishStatus.Dressed:
                return failedDish ? _modelRuined : _modelDressed;
            default:
                return _modelFailed;
        }
    }
}
