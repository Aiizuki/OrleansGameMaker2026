using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Nombre de fois où un plat a été servi, réussi ou raté.
/// </summary>
[Serializable]
public class DishStat
{
    public string DishName;
    public int SuccessCount;
    public int FailCount;
}

/// <summary>
/// Récap d'une partie, écrit au GameOver et relu dans la GameOverScene.
/// List et non Dictionary : JsonUtility ne sérialise pas les Dictionary.
/// </summary>
[Serializable]
public class GameStats
{
    public List<DishStat> Dishes = new List<DishStat>();
    public int NpcWhackCount;
    public float ElapsedTime;
}

/// <summary>
/// Passe les stats de la GameScene à la GameOverScene via un fichier temporaire.
/// </summary>
public static class GameStatsFile
{
    public static string FilePath => Path.Combine(Application.temporaryCachePath, "game_stats.json");

    public static void Save(GameStats stats)
    {
        File.WriteAllText(FilePath, JsonUtility.ToJson(stats, true));
        Debug.Log($"[GameStatsFile] Stats enregistrées dans {FilePath}");
    }

    /// <summary>False si aucune partie n'a été enregistrée (ex : GameOverScene lancée directement en éditeur).</summary>
    public static bool TryLoad(out GameStats stats)
    {
        stats = null;
        if (!File.Exists(FilePath))
            return false;

        stats = JsonUtility.FromJson<GameStats>(File.ReadAllText(FilePath));
        return stats != null;
    }
}
