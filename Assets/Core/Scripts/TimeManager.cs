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

        void Start()
        {
            StartOrderSpawn();
        }

        private void OnDestroy()
        {
            CancelEvents();
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
                yield return new WaitForSeconds(_settings.orderSpawnInterval);
            }
        }

        #region UnityEvents

        void InitEvents()
        {
            UnityEventManager.AddListener(nameof(EnumUnityEventName.StartOrderSpawn), StartOrderSpawn);
            UnityEventManager.AddListener(nameof(EnumUnityEventName.StopOrderSpawn), StopOrderSpawn);
        }

        void CancelEvents()
        {
            UnityEventManager.RemoveListener(nameof(EnumUnityEventName.StartOrderSpawn), StartOrderSpawn);
            UnityEventManager.RemoveListener(nameof(EnumUnityEventName.StopOrderSpawn), StopOrderSpawn);
        }

        #endregion UnityEvents
    }
}
