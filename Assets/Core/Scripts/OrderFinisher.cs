using System;
using Core.Enums;
using Core.Scripts;
using UnityEngine;

/// <summary>
/// Suit les ingrédients ramassés pour la commande en cours et, à OrderPrepared,
/// instancie le plat préparé en succès ou en échec.
/// </summary>
public class OrderFinisher : MonoBehaviour
{
    public void Deposit(PreparedDish dish)
    {
        UnityEventManager.TriggerEvent(dish.IsFailed || dish.DishStatus != EnumDishStatus.Dressed
            ? nameof(EnumUnityEventName.ShitDishDeposit)
            : nameof(EnumUnityEventName.GoodDishDeposit));

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