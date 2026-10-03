using System.ComponentModel;

namespace Core.Enums
{
    public enum EnumUnityEventName
    {
        GameStart,
        GameOver,
        
        #region InGameEvents
        
        [Description("Spawns an order ticket on the board")]
        SpawnCommande,

        [Description("The player took an order ticket from the board (param: the order GameObject)")]
        TakeOrder,

        [Description("The player now holds an order ticket (param: the ordered Plat)")]
        OrderTaken,

        [Description("The player prepared the current order: frees the order ticket so a new one can be taken")]
        OrderPrepared,

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
        
        #endregion BackgroundEvents
    }
}