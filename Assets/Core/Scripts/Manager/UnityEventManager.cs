using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Scripts
{
    public class UnityEventManager : MonoBehaviour {
        public static UnityEventManager Instance { get; private set; }

        // Statiques : utilisables avant l'Awake du manager, quel que soit l'ordre d'exécution des scripts
        private static readonly Dictionary<string, UnityAction> eventDictionary = new Dictionary<string, UnityAction>();
        private static readonly Dictionary<string, Delegate> paramEventDictionary = new Dictionary<string, Delegate>();

        // Vide les events au lancement du Play mode (nécessaire si le domain reload est désactivé)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetEvents() {
            eventDictionary.Clear();
            paramEventDictionary.Clear();
        }

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }

        public static void AddListener(string eventName, UnityAction listener) {
            eventDictionary.TryGetValue(eventName, out UnityAction thisEvent);
            eventDictionary[eventName] = thisEvent + listener;
        }

        public static void TriggerEvent(string eventName) {
            if (eventDictionary.TryGetValue(eventName, out UnityAction thisEvent)) {
                thisEvent?.Invoke();
            }
        }

        public static void RemoveListener(string eventName, UnityAction listener)
        {
            if (eventDictionary.TryGetValue(eventName, out UnityAction action))
            {
                eventDictionary[eventName] = action - listener;
            }
        }

        #region Events with parameter

        public static void AddListener<T>(string eventName, UnityAction<T> listener) {
            paramEventDictionary.TryGetValue(eventName, out Delegate thisEvent);
            paramEventDictionary[eventName] = Delegate.Combine(thisEvent, listener);
        }

        public static void TriggerEvent<T>(string eventName, T param) {
            if (paramEventDictionary.TryGetValue(eventName, out Delegate thisEvent)) {
                (thisEvent as UnityAction<T>)?.Invoke(param);
            }
        }

        public static void RemoveListener<T>(string eventName, UnityAction<T> listener)
        {
            if (paramEventDictionary.TryGetValue(eventName, out Delegate action))
            {
                paramEventDictionary[eventName] = Delegate.Remove(action, listener);
            }
        }

        #endregion Events with parameter
    }   
}