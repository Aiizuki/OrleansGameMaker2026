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

        [Description("An order ticket has been removed from the board (param: the ordered Plat)")]
        RefreshOrderUI,

        #endregion InGameEvents
        
        #region BackgroundEvents
        
        [Description("Event to start/restart order spawning")]
        StartOrderSpawn,
        
        [Description("When the board is full, no more order is created")]
        StopOrderSpawn,
        
        [Description("When the user goes to storage, switch the camera position")]
        SwitchCameraPosition,
        
        #endregion BackgroundEvents
    }
}