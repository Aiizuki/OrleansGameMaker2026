using TMPro;
using UnityEngine;

namespace Core.Scripts
{
    public class StatDetailer : MonoBehaviour
    {
        [SerializeField] private GameObject _datailStatPrefab;

        public void GenerateDetail(string platName, string PlatDetail)
        {
            GameObject go =  Instantiate(_datailStatPrefab, transform);
            go.GetComponent<TextPlacer>().NameZone.text = platName;
            go.GetComponent<TextPlacer>().DetailZone.text = PlatDetail;
        }
    }
}