using System;
using Core.Scripts;
using UnityEngine;
using Core.Enums;
using UnityEngine.SceneManagement;

public class SceneTransitionHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Awake()
    {
        UnityEventManager.AddListener(nameof(EnumUnityEventName.GameStart), TransitionToGameScene);
    }

    private void TransitionToGameScene()
    {
        SceneManager.LoadScene("GameScene");
        SceneManager.UnloadSceneAsync("Main menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
