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
        TakeCommande,

        #endregion InGameEvents
        
        #region BackgroundEvents
        
        [Description("Event to start/restart order spawning")]
        StartOrderSpawn,
        
        [Description("When the board is full, no more order is created")]
        StopOrderSpawn,
        
        #endregion BackgroundEvents
    }
}