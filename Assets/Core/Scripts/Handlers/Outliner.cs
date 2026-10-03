using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ajoute / retire un matériau de contour sur un renderer.
/// Appelé depuis les UnityEvent de l'Inspector (ShowOutline / HideOutline), chaque objet
/// peut donc avoir son propre matériau et ses propres déclencheurs.
/// </summary>
public class Outliner : MonoBehaviour
{
    [SerializeField] private Material _outlineMaterial;
    [SerializeField] private Renderer _rendererToOutline;

    public void ShowOutline()
    {
        SetOutline(true);
    }

    public void HideOutline()
    {
        SetOutline(false);
    }

    public void SetOutline(bool outlined)
    {
        // sharedMaterials pour ne pas dupliquer les matériaux de l'objet
        var materials = new List<Material>(_rendererToOutline.sharedMaterials);
        bool hasOutline = materials.Contains(_outlineMaterial);

        if (outlined && !hasOutline)
            materials.Add(_outlineMaterial);
        else if (!outlined && hasOutline)
            materials.Remove(_outlineMaterial);
        else
            return;

        // La liste est une copie : il faut la réassigner au renderer pour que ça s'affiche
        _rendererToOutline.sharedMaterials = materials.ToArray();
    }
}
