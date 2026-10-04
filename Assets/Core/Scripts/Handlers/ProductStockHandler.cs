using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProductStockHandler : MonoBehaviour
{
    [SerializeField] private Image _image;

    [Header("Events")]
    [SerializeField] private UnityEvent _onPlayerEnter;
    [SerializeField] private UnityEvent _onPlayerExit;

    public Plat Plat;
    public int Quantity { get; private set; }

    private IngredientSpawner _spawner;
    private Transform _spawnPoint;

    public ProductStockHandler Setup(Plat plat, Transform spawnPoint, IngredientSpawner spawner)
    {
        var instance = Instantiate(this, spawnPoint.position, spawnPoint.rotation, spawner.transform);
        instance._image.sprite = plat.Icon;
        instance.Plat = plat;
        instance.Quantity = 1;
        instance._spawner = spawner;
        instance._spawnPoint = spawnPoint;
        return instance;
    }

    public void AddProduct()
    {
        Quantity++;
    }

    public Plat TakeProduct()
    {
        Quantity--;
        if (Quantity <= 0)
        {
            _spawner.ReleaseSpawnPoint(_spawnPoint);
            Destroy(gameObject);
        }

        return Plat;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        other.gameObject.GetComponent<PlayerInteractionHandler>().Interactible = this.gameObject;
        _onPlayerEnter.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        other.gameObject.GetComponent<PlayerInteractionHandler>().Interactible = null;
        _onPlayerExit.Invoke();
    }
}