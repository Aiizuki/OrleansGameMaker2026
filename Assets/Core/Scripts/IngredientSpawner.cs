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
            // Le produit est déjà stocké quelque part : on ajoute 1 au carton existant
            var existingStock = GetStock(ingredient);
            if (existingStock != null)
            {
                existingStock.AddProduct();
                continue;
            }

            var freeSpawnPoint = GetFreeSpawnPoint();
            if (freeSpawnPoint == null)
            {
                Debug.LogError("Plus aucun point de spawn disponible !");
                return;
            }

            _spawnPoints[freeSpawnPoint] = _productCartonPrefab.GetComponent<ProductStockHandler>()
                .Setup(ingredient, freeSpawnPoint, this);
        }
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