using System;
using UnityEngine;

public class BoardCollisionHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerInteractionHandler>().Interactible = this.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerInteractionHandler>().Interactible = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
