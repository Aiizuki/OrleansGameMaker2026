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
        
    [SerializeField] private Mesh _modelRaw;
    [SerializeField] private Mesh _modelCut;
    [SerializeField] private Mesh _modelButchered;
    [SerializeField] private Mesh _modelCooked;
    [SerializeField] private Mesh _modelOvercooked;
    [SerializeField] private Mesh _modelDressed;
    [SerializeField] private Mesh _modelRuined;
    [SerializeField] private Mesh _modelFailed;
    
    [Header("Dish Settings (not used for single ingredients)")]
    public List<Plat> Ingredients;

    public Mesh GetMeshFromState(bool failedDish, EnumDishStatus status, bool overFailed = false)
    {
        if (overFailed)
            return _modelFailed;
        
        switch (status)
        {
            case EnumDishStatus.Raw:
                return _modelRaw;
            case EnumDishStatus.Cutted:
                return failedDish ? _modelCut : _modelButchered;
            case EnumDishStatus.Cooked:
                return failedDish ? _modelCooked : _modelOvercooked;
            case EnumDishStatus.Dressed:
                return failedDish ? _modelDressed : _modelRuined;
            default:
                return _modelFailed;
        }
    }
}
