using FMOD.Studio;
using FMODUnity;
using System.Collections;
using Core.Enums;
using Core.Scripts;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class ButtonsManager : MonoBehaviour
{
    #region Unity Lifecycle

    protected void OnEnable()
    {
        this.GetComponent<Button>().interactable = true;
    }

    protected void OnDisable()
    {
        StopAllCoroutines();
    }

    #endregion Unity Lifecycle
    
    
    
    
    public void FireReturnToHomeEvent()
    {
        PlayBack();
        UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.ReturnToHome));
    }

    public void FireNewGameEvent()
    {
        PlayForward();
        UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.NewGame));
    }

    public void PlayBack(bool preventSpam = true)
        => PlayButtonClick(preventSpam, true);

    public void PlayForward(bool preventSpam = true)
        => PlayButtonClick(preventSpam, false);

    public void QuitGame()
    {
        PlayForward();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit(); // Quitte l'application compilée
#endif
    }

    private void PlayButtonClick(bool preventSpam, bool isBackButton)
    {
        // EventInstance instance = RuntimeManager.CreateInstance(FMODEvents.Instance.ButtonClick);
        //
        // instance.setParameterByNameWithLabel("ButtonClickType", isBackButton ? "GoBack" : "GoForward");
        // instance.set3DAttributes(RuntimeUtils.To3DAttributes(Vector3.zero));
        // instance.start();
        // instance.release();

        if (preventSpam)
            StartCoroutine(PreventSpamClickRoutine());
    }
    
    private IEnumerator PreventSpamClickRoutine()
    {
        this.GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(1f);
        this.GetComponent<Button>().interactable = true;
    }
}