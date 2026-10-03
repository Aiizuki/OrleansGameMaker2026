using System;
using UnityEngine;
using UnityEngine.Events;

public class BoardCollisionHandler : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent _onPlayerEnter;
    [SerializeField] private UnityEvent _onPlayerExit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerInteractionHandler>().Interactible = this.gameObject;
            _onPlayerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerInteractionHandler>().Interactible = null;
            _onPlayerExit.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
