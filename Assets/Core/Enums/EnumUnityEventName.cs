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
        
        #endregion InGameEvents
    }
}