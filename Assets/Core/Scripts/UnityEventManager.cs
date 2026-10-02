using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Scripts
{
    public class UnityEventManager : MonoBehaviour {
        private static EventManager _instance;
        public static UnityEventManager Instance { get; private set; }

        private Dictionary<string, UnityAction> eventDictionary;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                eventDictionary = new Dictionary<string, UnityAction>();
            } else {
                Destroy(gameObject);
            }
        }

        public static void AddListener(string eventName, UnityAction listener) {
            if (Instance.eventDictionary.TryGetValue(eventName, out UnityAction thisEvent)) {
                thisEvent += listener;
            } else {
                thisEvent = listener;
                Instance.eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void TriggerEvent(string eventName) {
            if (Instance.eventDictionary.TryGetValue(eventName, out UnityAction thisEvent)) {
                thisEvent?.Invoke();
            }
        }

        public static void RemoveListener(string eventName, UnityAction listener)
        {
            if (Instance.eventDictionary.TryGetValue(eventName, out UnityAction action))
            {
                action -= listener;
            }
        }   
    }   
}