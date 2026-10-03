using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductStockHandler : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Material _outlineMaterial;
    [SerializeField] private MeshRenderer _rendererToHighlight;
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
        SetHighlight(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        other.gameObject.GetComponent<PlayerInteractionHandler>().Interactible = null;
        SetHighlight(false);
    }

    private void SetHighlight(bool highlighted)
    {
        // sharedMaterials pour ne pas dupliquer les matériaux de l'objet
        var materials = new List<Material>(_rendererToHighlight.sharedMaterials);
        bool hasOutline = materials.Contains(_outlineMaterial);

        if (highlighted && !hasOutline)
            materials.Add(_outlineMaterial);
        else if (!highlighted && hasOutline)
            materials.Remove(_outlineMaterial);
        else
            return;

        // La liste est une copie : il faut la réassigner au renderer pour que ça s'affiche
        _rendererToHighlight.sharedMaterials = materials.ToArray();
    }
}