using System;
using System.Collections.Generic;
using Core.Enums;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Scripts
{
    /// <summary>
    /// Composant de debug : affiche dans l'inspecteur un bouton par valeur de EnumUnityEventName
    /// pour déclencher l'event correspondant via UnityEventManager (en Play mode uniquement).
    /// </summary>
    public class UnityEventDebugger : MonoBehaviour
    {
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UnityEventDebugger))]
    public class UnityEventDebuggerEditor : Editor
    {
        // Paramètre GameObject optionnel par event (ex : TakeCommande)
        private readonly Dictionary<string, GameObject> _params = new Dictionary<string, GameObject>();

        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Les events ne peuvent être déclenchés qu'en Play mode.", MessageType.Info);
            }
            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                foreach (var eventName in Enum.GetNames(typeof(EnumUnityEventName)))
                {
                    DrawEvent(eventName);
                }
            }
        }

        private void DrawEvent(string eventName)
        {
            EditorGUILayout.BeginHorizontal();

            _params.TryGetValue(eventName, out var param);
            _params[eventName] = (GameObject)EditorGUILayout.ObjectField(param, typeof(GameObject), true, GUILayout.Width(150));

            if (GUILayout.Button(eventName))
            {
                // Sans paramètre : on déclenche l'event classique ET la version GameObject avec null,
                // pour que les listeners paramétrés puissent gérer leur propre cas de debug.
                if (_params[eventName] == null)
                {
                    UnityEventManager.TriggerEvent(eventName);
                    UnityEventManager.TriggerEvent<GameObject>(eventName, null);
                }
                else
                {
                    UnityEventManager.TriggerEvent(eventName, _params[eventName]);
                }

                Debug.Log("[UnityEventDebugger] Event déclenché : " + eventName
                          + (_params[eventName] != null ? " (" + _params[eventName].name + ")" : string.Empty));
            }

            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}
