using System.Collections.Generic;
using Core.Enums;
using Core.Scripts;
using JetBrains.Annotations;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _productCartonPrefab;
    [SerializeField] private List<Transform> _lstSpawnPoints;

    [ItemCanBeNull] private Dictionary<Transform, ProductStockHandler> _spawnPoints;

    void Awake()
    {
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.SpawnStorage), SpawnStorage);
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.ReturnToStorage), ReturnToStorage);

        _spawnPoints = new Dictionary<Transform, ProductStockHandler>();
        foreach (var spawnPoint in _lstSpawnPoints)
        {
            _spawnPoints.Add(spawnPoint, null);
        }
    }

    private void SpawnStorage(Plat plat)
    {
        foreach (var ingredient in plat.Ingredients)
        {
            if (!AddToStorage(ingredient))
                return;
        }
    }

    private void ReturnToStorage(Plat ingredient)
    {
        AddToStorage(ingredient);
    }

    // Ajoute 1 produit au stock : +1 sur le carton existant, sinon nouveau carton sur un emplacement libre
    private bool AddToStorage(Plat ingredient)
    {
        var existingStock = GetStock(ingredient);
        if (existingStock != null)
        {
            existingStock.AddProduct();
            return true;
        }

        var freeSpawnPoint = GetFreeSpawnPoint();
        if (freeSpawnPoint == null)
        {
            Debug.LogError("Plus aucun point de spawn disponible !");
            return false;
        }

        _spawnPoints[freeSpawnPoint] = _productCartonPrefab.GetComponent<ProductStockHandler>()
            .Setup(ingredient, freeSpawnPoint, this);
        return true;
    }

    public void ReleaseSpawnPoint(Transform spawnPoint)
    {
        _spawnPoints[spawnPoint] = null;
    }

    [CanBeNull]
    private ProductStockHandler GetStock(Plat ingredient)
    {
        foreach (var stock in _spawnPoints.Values)
        {
            if (stock != null && stock.Plat == ingredient)
                return stock;
        }

        return null;
    }

    [CanBeNull]
    private Transform GetFreeSpawnPoint()
    {
        var freeSpawnPoints = new List<Transform>();
        foreach (var spawnPoint in _lstSpawnPoints)
        {
            if (_spawnPoints[spawnPoint] == null)
                freeSpawnPoints.Add(spawnPoint);
        }

        if (freeSpawnPoints.Count == 0)
            return null;

        return freeSpawnPoints[Random.Range(0, freeSpawnPoints.Count)];
    }
}