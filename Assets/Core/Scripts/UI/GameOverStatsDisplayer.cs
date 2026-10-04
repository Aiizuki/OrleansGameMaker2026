using Core.Scripts;
using TMPro;
using UnityEngine;

/// <summary>
/// Affiche dans la GameOverScene les stats de la partie enregistrées par le GameMaster au GameOver.
/// </summary>
public class GameOverStatsDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _chronoText;
    [SerializeField] private TextMeshProUGUI _statsText;
    [SerializeField] private TextMeshProUGUI _hitText;
    
    [SerializeField] private StatDetailer _statDetailer;

    void Start()
    {
        if (!GameStatsFile.TryLoad(out GameStats stats))
        {
            _statsText.text = "Aucune stat";
            return;
        }

        int minutes = Mathf.FloorToInt(stats.ElapsedTime / 60f);
        int seconds = Mathf.FloorToInt(stats.ElapsedTime % 60f);
        _chronoText.text = $"{minutes:00}:{seconds:00}";
        _hitText.text = $"{stats.NpcWhackCount}";

        // Le StatDetailer de la scène n'est pas forcément assigné dans l'inspector
        if (_statDetailer == null)
            _statDetailer = FindFirstObjectByType<StatDetailer>();
        if (_statDetailer == null)
            Debug.LogWarning($"[GameOverStatsDisplayer] Aucun StatDetailer trouvé : le détail par plat ne sera pas affiché !");

        int totalServed = 0;
        foreach (DishStat dish in stats.Dishes)
        {
            totalServed += dish.SuccessCount + dish.FailCount;
            if (_statDetailer != null)
                _statDetailer.GenerateDetail(dish.DishName, $"{dish.SuccessCount} réussi(s), {dish.FailCount} raté(s)");
        }
        _statsText.text = $"{totalServed}";
    }
}
