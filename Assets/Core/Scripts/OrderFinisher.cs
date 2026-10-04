using System;
using Core.Enums;
using Core.Scripts;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Suit les ingrédients ramassés pour la commande en cours et, à OrderPrepared,
/// instancie le plat préparé en succès ou en échec.
/// </summary>
public class OrderFinisher : MonoBehaviour
{
    public UnityEvent ReactToGoodDish;
    public UnityEvent ReactToShitDish;
    
    public void Deposit(PreparedDish dish)
    {
        UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.DishServed), dish);

        if (dish.IsFailed || dish.DishStatus != EnumDishStatus.Dressed)
        {
            ReactToShitDish.Invoke();
            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.ShitDishDeposit));
        }
        else
        {
            ReactToGoodDish.Invoke();
            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.GoodDishDeposit));
        }

        GetComponent<Outliner>().HideOutline();
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
}