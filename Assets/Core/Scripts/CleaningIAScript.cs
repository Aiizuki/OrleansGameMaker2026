using System;
using System.Collections.Generic;
using UnityEngine;

public enum AiState {
    Working,
    Idle,
    Moving
}

public enum MoveDestination
{
    Station,
    Output,
    Input
}

public class CleaningIAScript : MonoBehaviour
{ 
    public GameObject hands;
    public float speed;
    public float workTime;
    public List<GameObject> PathToInput;
    public List<GameObject> PathFromInput;
    public List<GameObject> PathToOutput;
    public List<GameObject> PathFromOutput;
    public GameObject MyStation;
    public GameObject NextStation;

    private int _path;
    private GameObject _objectInHands;
    private float _workTimer;
    private AiState _aiState;
    private bool _shouldGoToInput;
    private bool _shouldGoToOutput;
    private GameObject _theStation;
    private int _moveStep;
    private Vector3 _movementDirection;

    private void Start()
    {
        _workTimer = workTime;
        _theStation = MyStation;
        _aiState = AiState.Idle;
    }

    private void Update()
    {
        LookAround();
        if (_workTimer > 0 && _aiState == AiState.Working)
        {
            //working
            _workTimer -= Time.deltaTime;
        }

        if (_workTimer <= 0 && _aiState == AiState.Working)
        {
            //end of work
            _aiState = AiState.Idle;
        }
        
        if (_aiState == AiState.Idle && _shouldGoToOutput && ! (_objectInHands != null))
        {
            if (Vector3.Distance(transform.position, _theStation.GetComponent<StationScript>().StationInventory.transform.position) > 0.2f)
            {
                GoTo(MoveDestination.Station);
            }
            else
            {
                // take food and go to output to place it
                TakeInHands(_theStation.GetComponent<StationScript>().objectAtInventory);
                _theStation.GetComponent<StationScript>().objectAtInventory = null;
                GoTo(MoveDestination.Output);
            }
        }

        if (_aiState == AiState.Idle && _shouldGoToInput && ! (_objectInHands != null))
        {
            if (Vector3.Distance(transform.position, _theStation.GetComponent<StationScript>().StationInput.transform.position) > 0.2f)
            {
                GoTo(MoveDestination.Input);
            }
            else
            {
                TakeInHands(_theStation.GetComponent<StationScript>().objectAtInput);
                _theStation.GetComponent<StationScript>().objectAtInput = null;
                GoTo(MoveDestination.Station);
            }
        }
        
        if (_aiState == AiState.Idle && _objectInHands != null)
        {
            if (Vector3.Distance(transform.position,_theStation.GetComponent<StationScript>().StationOutput.transform.position) <= 0.2f)
            {
                // When IA as object in hands and is at output spot
                NextStation.GetComponent<StationScript>().objectAtInput = _objectInHands;
                _objectInHands.transform.parent = NextStation.GetComponent<StationScript>().placeAtInput.transform;
                _objectInHands.transform.localPosition = Vector3.zero;
                _objectInHands = null;
                // Return to Station
                GoTo(MoveDestination.Station);
            }
            else if (Vector3.Distance(transform.position, _theStation.GetComponent<StationScript>().StationInput.transform.position) <= 0.2f)
            {
                // When IA as objetc in hands and is at input spot
                TakeInHands(_theStation.GetComponent<StationScript>().objectAtInput);
                _theStation.GetComponent<StationScript>().objectAtInput = null;
                // Return to Station
                GoTo(MoveDestination.Station);
            }
            else if (Vector3.Distance(transform.position, _theStation.GetComponent<StationScript>().StationInventory.transform.position) <= 0.2f)
            {
                // Put object on station
                _theStation.GetComponent<StationScript>().objectAtInventory = _objectInHands;
                _objectInHands.transform.parent = _theStation.GetComponent<StationScript>().placeAtInventory.transform;
                _objectInHands.transform.localPosition = Vector3.zero;
                _objectInHands = null;
                // Work
                Work();
            }
        }
    }

    private void FixedUpdate()
    {
        if (_aiState == AiState.Moving)
        {
            _movementDirection = GetPath(_path)[_moveStep].transform.position - transform.position;
            _movementDirection.Normalize();
            _movementDirection = _movementDirection * speed;
            
            if (Vector3.Distance(transform.position, GetPath(_path)[_moveStep].transform.position) < 0.1f)
            {
                if (_moveStep < GetPath(_path).Count - 1 )
                {
                    Debug.Log("move step");
                    _moveStep++;
                }
                else
                {
                    _aiState = AiState.Idle;
                }
            }
            else
            {
                transform.position += _movementDirection;
            }
        }
    }

    private void LookAround()
    {
        _shouldGoToInput = _theStation.GetComponent<StationScript>().objectAtInput != null && !(_theStation.GetComponent<StationScript>().objectAtInventory != null);
        _shouldGoToOutput = _theStation.GetComponent<StationScript>().objectAtInventory != null && !(NextStation.GetComponent<StationScript>().objectAtInput != null);
    }

    private void TakeInHands(GameObject _object)
    {
        _object.transform.parent = hands.transform;
        _object.transform.localPosition = Vector3.zero;
        _objectInHands = _object;
    }

    private List<GameObject> GetPath(int pathID)
    {
        switch (pathID)
        {
            case 0:
                return PathFromInput;
            case 1:
                return PathToInput;
            case 2:
                return PathFromOutput;
            case 3:
                return PathToOutput;
        }
        return null;
    }

    private void GoTo(MoveDestination destination)
    {
        _moveStep = 0;
        switch (destination)
        {
            case MoveDestination.Station:
                if (_path == 1)
                {
                    _path = 0;
                }
                else if (_path == 3)
                {
                    _path = 2;
                }
                break;
            case MoveDestination.Input:
                _path = 1;
                break;
            case MoveDestination.Output:
                _path = 3;
                break;
        }
        _aiState = AiState.Moving;
    }

    private void Work()
    {
        _workTimer = workTime;
        _aiState = AiState.Working;
    }
}
