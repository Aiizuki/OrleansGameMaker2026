using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WhackingHandler : MonoBehaviour
{
    [Header("Inputs")] [SerializeField] private InputActionReference _whackAction;
    [SerializeField] private Animator _animatorController;
    public GameObject CurrentVictim = null;

    private void Awake()
    {
        InitEvents();
    }

    private void OnDestroy()
    {
        RevokeEvents();
    }

    private bool _isWhacking = false;

    private void InitEvents()
    {
        _whackAction.action.performed += Whack;
    }

    private void RevokeEvents()
    {
        _whackAction.action.performed -= Whack;
    }

    private void Whack(InputAction.CallbackContext obj)
    {
        _animatorController.SetTrigger("Hit");

        if (CurrentVictim != null)
        {
            Debug.Log("TIENS PRENDS CA BATAR");
            CurrentVictim.gameObject.GetComponent<CleaningIAScript>().Whacked();
        }
        else
        {
            Debug.Log("*se pougne*");
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
