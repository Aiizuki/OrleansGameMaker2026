using System.Collections;
using Core.Enums;
using UnityEngine;

namespace Core.Scripts
{
    public class TimeManager : MonoBehaviour
    {
        [SerializeField] private TimeSettings _settings;

        private Coroutine _spawnRoutine;

        void Awake()
        {
            InitEvents();
        }

        private void OnDestroy()
        {
            CancelEvents();
        }

        void Start()
        {
            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.StartTime));
        }

        private void StopTime()
        {
            StopOrderSpawn();
        }

        private void StartOrderSpawn()
        {
            _spawnRoutine ??= StartCoroutine(HandleSpawnTimeCycle());
        }

        private void StopOrderSpawn()
        {
            if (_spawnRoutine == null)
            {
                return;
            }

            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }

        private IEnumerator HandleSpawnTimeCycle()
        {
            while (true)
            {
                UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.SpawnCommande));
                yield return new WaitForSeconds(_settings.OrderSpawnInterval);
            }
        }

        #region UnityEvents

        void InitEvents()
        {
            UnityEventManager.AddListener(nameof(EnumUnityEventName.StartTime), StartOrderSpawn);
            UnityEventManager.AddListener(nameof(EnumUnityEventName.StartOrderSpawn), StartOrderSpawn);
            UnityEventManager.AddListener(nameof(EnumUnityEventName.StopOrderSpawn), StopOrderSpawn);
            UnityEventManager.AddListener(nameof(EnumUnityEventName.GameOver), StopTime);
        }

        void CancelEvents()
        {
            UnityEventManager.RemoveListener(nameof(EnumUnityEventName.StartTime), StartOrderSpawn);
            UnityEventManager.RemoveListener(nameof(EnumUnityEventName.StartOrderSpawn), StartOrderSpawn);
            UnityEventManager.RemoveListener(nameof(EnumUnityEventName.StopOrderSpawn), StopOrderSpawn);
            UnityEventManager.RemoveListener(nameof(EnumUnityEventName.GameOver), StopTime);
        }

        #endregion UnityEvents
    }
}
