using System.ComponentModel;

namespace Core.Enums
{
    public enum EnumUnityEventName
    {
        GameStart,
        GameOver,

        [Description("Starts everything driven by time (order spawn, angry slider decrease)")]
        StartTime,
        
        #region InGameEvents
        
        [Description("Spawns an order ticket on the board")]
        SpawnCommande,

        [Description("The player took an order ticket from the board (param: the order GameObject)")]
        TakeOrder,

        [Description("The player now holds an order ticket (param: the ordered Plat)")]
        OrderTaken,

        [Description("The player prepared the current order: frees the order ticket so a new one can be taken")]
        OrderPrepared,

        [Description("The player picked an ingredient that is part of the current order (param: the ingredient Plat)")]
        IngredientCollected,

        [Description("The player picked an ingredient that is not (or no longer) expected by the current order (param: the ingredient Plat)")]
        WrongIngredient,

        [Description("Sets the prepared dish(es) to success (green material)")]
        DishSucceeded,

        [Description("Sets the prepared dish(es) to failure (red material)")]
        DishFailed,

        [Description("Puts every ingredient picked by the player back into the storage")]
        FlushPlayerInventory,

        [Description("The player's inventory has been put back into the storage (param: the current order Plat)")]
        InventoryFlushed,
        
        [Description("The player has collected all correct ingredients")]
        IngredientsCollected,

        [Description("Raises the angry slider by the amount set in TimeSettings")]
        RaiseAngrySlider,

        #endregion InGameEvents
        
        #region BackgroundEvents
        
        [Description("Event to start/restart order spawning")]
        StartOrderSpawn,
        
        [Description("When the board is full, no more order is created")]
        StopOrderSpawn,
        
        [Description("When the user goes to storage, switch the camera position")]
        SwitchCameraPosition,
        
        [Description("Spawns a storage box in the storage room")]
        SpawnStorage,

        [Description("Puts one ingredient back into the storage (param: the ingredient Plat)")]
        ReturnToStorage,
        
        [Description("Highlight the bin when the player has selected a wrong ingredient")]
        HighlightBin,
        
        #endregion BackgroundEvents

    }
}