using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.Enums;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Core.Scripts
{
    public class CommandeSpawner : MonoBehaviour
    {
        [SerializeField] private CoreGameSettings _settings;
        [SerializeField] private List<GameObject> _lstSpawnPoints;
        [SerializeField] private GameObject _orderPrefab;
        public UnityEvent ReactToNewOrder;
        
        private List<Plat> _lstSpawnablePlats;
        
        // Point de spawn -> commande qui l'occupe (null si libre)
        public Dictionary<GameObject, GameObject> _spawnMatrix;
        
        void Awake()
        {
            _lstSpawnablePlats = _settings.LstAvailablePlats;
            if (_lstSpawnablePlats == null || _lstSpawnablePlats.Count == 0)
            {
                Debug.LogError("Aucun plat dans la liste" + Environment.NewLine + new StackTrace());
                return;
            }

            if (_orderPrefab == null)
            {
                Debug.Log("Aucun order prefab dans la liste" + Environment.NewLine +  new StackTrace());
            }

            InitEvents();
            
            _spawnMatrix = new Dictionary<GameObject, GameObject>();
            foreach (var spawnPoint in _lstSpawnPoints)
            {
                _spawnMatrix.Add(spawnPoint, null);
            }
        }
        
        private void OnDestroy()
        {
            CancelEvents();
        }

        void SpawnCommande()
        {
            var spawnPoint = SelectAvailablePos();
            if (spawnPoint is null)
            {
                Debug.LogError("Tous les emplacements sont pris, revoir le timing de spawn");
                UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.StopOrderSpawn));
                return;
            }

            var plat = _lstSpawnablePlats[Random.Range(0, _lstSpawnablePlats.Count)];
            //Debug.Log("Je spawn le plat" + plat.Nom);
            ReactToNewOrder.Invoke();
            var commande = Instantiate(_orderPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation, transform);
            if (!commande.TryGetComponent(out OrderTicket ticket))
            {
                ticket = commande.AddComponent<OrderTicket>();
            }
            ticket.Init(plat);
            _spawnMatrix[spawnPoint] = commande;
            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.SpawnStorage), plat);

            if (SelectAvailablePos() is null)
            {
                UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.StopOrderSpawn));
            }
        }

        void TakeCommande()
        {
            // En debug (éditeur / development build), sans commande fournie : on prend la première de la matrice

            GameObject commande = _spawnMatrix.Values.FirstOrDefault(x => x is not null);
            if (commande is null)
            {
                Debug.LogWarning("[Debug] Aucune commande dans la matrice à retirer");
                return;
            }

            var orderTicket = _spawnMatrix.FirstOrDefault(x => x.Value == commande);
            if (orderTicket.Key is null)
            {
                Debug.LogError("Commande introuvable dans la matrice : " + commande.name);
                return;
            }

            var plat = commande.GetComponent<OrderTicket>().Plat;
            Debug.Log("J'ai pris la commande de " + plat.Nom);

            _spawnMatrix[orderTicket.Key] = null;
            Destroy(commande); // TODO : faire un PlayerInventoryManager qui réagit à cet event pour récupérer la commande
            
            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.OrderTaken), plat);
            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.StartOrderSpawn));
        }

        private GameObject SelectAvailablePos()
        {
            return _spawnMatrix.FirstOrDefault(x => x.Value is null).Key;
        }

        #region UnityEvents

        void InitEvents()
        {
            UnityEventManager.AddListener(nameof(EnumUnityEventName.SpawnCommande), SpawnCommande);
            UnityEventManager.AddListener(nameof(EnumUnityEventName.TakeOrder), TakeCommande);
        }

        void CancelEvents()
        {
            UnityEventManager.RemoveListener(nameof(EnumUnityEventName.SpawnCommande), SpawnCommande);
            UnityEventManager.RemoveListener(nameof(EnumUnityEventName.TakeOrder), TakeCommande);
        }

        #endregion UnityEvents
    }
}
