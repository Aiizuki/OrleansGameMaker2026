using System;
using Core.Enums;
using Core.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BinHandler : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent _onPlayerEnter;
    [SerializeField] private UnityEvent _onPlayerExit;
    
    void Awake()
    {
        UnityEventManager.AddListener(nameof(EnumUnityEventName.HighlightBin), GetComponent<Outliner>().ShowOutline);
    }

    private void OnDestroy()
    {
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.HighlightBin), GetComponent<Outliner>().ShowOutline);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        other.gameObject.GetComponent<PlayerInteractionHandler>().Interactible = this.gameObject;
        _onPlayerEnter.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        other.gameObject.GetComponent<PlayerInteractionHandler>().Interactible = null;
        _onPlayerExit.Invoke();
    }
}