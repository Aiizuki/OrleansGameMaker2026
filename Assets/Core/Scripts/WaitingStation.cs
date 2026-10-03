using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public enum IOAction
{
    Pull,
    Push
}

public class WaitingStation : MonoBehaviour
{
    public IOAction stationMode;
    public List<GameObject> objectsWaiting = new List<GameObject>();
    public List<GameObject> objectsPlaces = new List<GameObject>();

    public GameObject linkedStation;

    // Update is called once per frame
    void Update()
    {
        switch (stationMode)
        {
            case IOAction.Pull:
                Deposit();
                break;
            case IOAction.Push:
                PushToLinkedStation();
                break;
        }
    }

    public void Deposit()
    {
        if (!CheckToPull())
            return;

        objectsWaiting.Add(linkedStation.GetComponent<StationScript>().objectAtInput);
        linkedStation.GetComponent<StationScript>().objectAtInput = null;
        UpdateVisuals();
    }

    public bool HasObjectWaiting => objectsWaiting.Count > 0;

    /// <summary>
    /// Retire le premier objet de la file et le renvoie (null si la file est vide).
    /// Ne le pose pas sur la station liée : en mode Pull, Deposit() le reprendrait à la frame suivante.
    /// </summary>
    [CanBeNull]
    public GameObject Take()
    {
        if (!HasObjectWaiting)
            return null;

        GameObject takenObject = objectsWaiting[0];
        // RemoveAt décale déjà le reste de la file
        objectsWaiting.RemoveAt(0);

        UpdateVisuals();
        return takenObject;
    }

    /// <summary>
    /// Mode Push : passe le premier objet de la file à l'entrée de la station liée, si elle est libre.
    /// </summary>
    private void PushToLinkedStation()
    {
        if (!CheckToPush())
            return;

        StationScript station = linkedStation.GetComponent<StationScript>();
        GameObject pushedObject = Take();
        station.objectAtInput = pushedObject;
        pushedObject.transform.parent = station.placeAtInput.transform;
        pushedObject.transform.localPosition = Vector3.zero;
    }

    public void AddObjectToWaiting(GameObject _objectToWaiting)
    {
        if (objectsWaiting.Count < objectsPlaces.Count)
        {
            objectsWaiting.Add(_objectToWaiting);
            UpdateVisuals();
        }
        else
        {
            Destroy(_objectToWaiting);
        }
    }

    public bool CheckToPull()
    {
        if (linkedStation.GetComponent<StationScript>().objectAtInput != null &&
            objectsWaiting.Count < objectsPlaces.Count)
        {
            return true;
        }

        return false;
    }

    public bool CheckToPush()
    {
        if (!(linkedStation.GetComponent<StationScript>().objectAtInput != null) && objectsWaiting.Count > 0)
        {
            return true;
        }

        return false;
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < (objectsWaiting.Count); i++)
        {
            objectsWaiting[i].transform.parent = objectsPlaces[i].transform;
            objectsWaiting[i].transform.localPosition = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PreparedOrder") &&
            objectsWaiting.Count < objectsPlaces.Count && stationMode == IOAction.Pull)
        {
            objectsWaiting.Add(other.gameObject);
            UpdateVisuals();
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerInteractionHandler>().Interactible = gameObject;
        }
    }
}