using System;
using System.Collections.Generic;
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
                if (CheckToPull())
                {
                    objectsWaiting.Add(linkedStation.GetComponent<StationScript>().objectAtInput);
                    linkedStation.GetComponent<StationScript>().objectAtInput = null;
                    UpdateVisuals();
                }
                break;
            case IOAction.Push:
                if (CheckToPush())
                {
                    linkedStation.GetComponent<StationScript>().objectAtInput = objectsWaiting[0];
                    objectsWaiting[0].transform.parent = linkedStation.GetComponent<StationScript>().objectAtInput.transform;
                    objectsPlaces[0].transform.localPosition = Vector3.zero;
                    if (objectsWaiting.Count > 1)
                    {
                        for (int i = 1; i < (objectsWaiting.Count - 1); i++)
                        {
                            objectsWaiting[i-1] = objectsWaiting[i];
                        }
                    }
                    UpdateVisuals();
                }
                break;
        }
    }

    private bool CheckToPull()
    {
        if (linkedStation.GetComponent<StationScript>().objectAtInput != null && objectsWaiting.Count < objectsPlaces.Count)
        {
            return true;
        }
        return false;
    }

    private bool CheckToPush()
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
            objectsPlaces[i].transform.localPosition = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Mlem");
        if (other.gameObject.layer == LayerMask.NameToLayer("PreparedOrder") && objectsWaiting.Count < objectsPlaces.Count && stationMode ==  IOAction.Pull)
        {
            objectsWaiting.Add(other.gameObject);
            UpdateVisuals();
        }
    }
}
