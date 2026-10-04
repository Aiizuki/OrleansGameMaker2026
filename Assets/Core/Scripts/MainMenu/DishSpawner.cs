using System.Collections;
using UnityEngine;

public class DishSpawner : MonoBehaviour
{
    private Coroutine _spawnRoutine;
    private Coroutine _killRoutine;
    
    [SerializeField] private WaitingStation _orderSpawn;
    [SerializeField] private WaitingStation _orderKill;
    [SerializeField] private CoreGameSettings _settings;
    [SerializeField] private GameObject _preparedDishPrefab;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spawnRoutine ??= StartCoroutine(OrderLifeRoutine());
        _killRoutine ??= StartCoroutine(OrderKillRoutine());
    }

    void OnDestroy()
    {
        StopCoroutine(_spawnRoutine);
        StopCoroutine(_killRoutine);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator OrderLifeRoutine()
    {
        while (true)
        {
            _preparedDishPrefab.GetComponent<PreparedDish>().Plat = _settings.LstAvailablePlats[Random.Range(0, _settings.LstAvailablePlats.Count)];
            
            GameObject go = Instantiate(_preparedDishPrefab);
            _orderSpawn.Deposit(go);
            
            yield return new WaitForSeconds(15f);
        }
    }
    
    private IEnumerator OrderKillRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);
            var plat = _orderKill.Take();
            if(plat)
                Destroy(plat);
        }
    }
}
