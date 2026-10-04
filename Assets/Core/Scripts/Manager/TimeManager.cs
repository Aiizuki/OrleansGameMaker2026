using System.Collections;
using Core.Enums;
using UnityEngine;

namespace Core.Scripts
{
    public class TimeManager : MonoBehaviour
    {
        [SerializeField] private TimeSettings _settings;
        public GameObject CommandeManagerInScene;

        private Coroutine _spawnRoutine;
        private bool _isTimeRunning;

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
            StartCoroutine(StartGameRoutine());
        }

        /// <summary>
        /// Attend que tous les Start de la scène soient passés, lance la partie (GameStart),
        /// joue le décompte puis démarre le temps (StartTime).
        /// </summary>
        private IEnumerator StartGameRoutine()
        {
            // L'ordre des Start entre objets n'est pas garanti : on laisse passer une frame
            yield return null;
            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.GameStart));

            for (int remaining = _settings.StartCountdownDuration; remaining > 0; remaining--)
            {
                UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.CountdownTick), remaining);
                yield return new WaitForSeconds(1f);
            }

            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.CountdownTick), 0);
            _isTimeRunning = true;
            // Lance en même temps le spawn des commandes (ici) et la baisse du slider (AngrySliderHandler)
            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.StartTime));
        }

        private void StopTime()
        {
            _isTimeRunning = false;
            StopOrderSpawn();
        }

        private void StartOrderSpawn()
        {
            // StartOrderSpawn peut être déclenché d'ailleurs (prise de commande) : rien avant la fin du décompte
            if (!_isTimeRunning)
                return;

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
                yield return new WaitForSeconds(GetSpawnInterval());
            }
        }

        private float GetSpawnInterval()
        {
            Debug.Log(_settings.OrderSpawnInterval + (_settings.OrderSpawnInterval * CommandeManagerInScene.GetComponent<CommandeSpawner>().GetNbrOfCommande()));
            return _settings.OrderSpawnInterval + (_settings.OrderSpawnInterval * CommandeManagerInScene.GetComponent<CommandeSpawner>().GetNbrOfCommande());
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
