using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Affiche / masque un contour autour d'un renderer.
/// Le contour est un GameObject enfant qui redessine le même mesh avec le matériau d'outline
/// (inverted hull), plutôt qu'un matériau ajouté au renderer : ça couvre tous les sous-meshes
/// et ne touche pas aux matériaux de l'objet.
/// Appelé depuis les UnityEvent de l'Inspector (ShowOutline / HideOutline), chaque objet
/// peut donc avoir son propre matériau et ses propres déclencheurs.
/// </summary>
public class Outliner : MonoBehaviour
{
    [SerializeField] private Material _outlineMaterial;
    [SerializeField] private Renderer _rendererToOutline;

    private static readonly Dictionary<(Mesh, Vector3), Mesh> _smoothedMeshes = new Dictionary<(Mesh, Vector3), Mesh>();

    private GameObject _outlineObject;

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
        if (outlined && _outlineObject == null)
            CreateOutlineObject();

        if (_outlineObject != null)
            _outlineObject.SetActive(outlined);
    }

    private void CreateOutlineObject()
    {
        if (_outlineMaterial == null || _rendererToOutline == null)
        {
            Debug.LogError($"[Outliner] Matériau ou renderer non assigné sur {name} !");
            return;
        }

        MeshFilter sourceFilter = _rendererToOutline.GetComponent<MeshFilter>();
        if (sourceFilter == null || sourceFilter.sharedMesh == null)
        {
            Debug.LogError($"[Outliner] Pas de MeshFilter sur {_rendererToOutline.name} !");
            return;
        }

        // Enfant du renderer pour suivre exactement sa position / rotation / scale
        _outlineObject = new GameObject("Outline");
        _outlineObject.transform.SetParent(_rendererToOutline.transform, false);

        Mesh mesh = GetSmoothedMesh(sourceFilter.sharedMesh, _rendererToOutline.transform.lossyScale);
        _outlineObject.AddComponent<MeshFilter>().sharedMesh = mesh;

        MeshRenderer outlineRenderer = _outlineObject.AddComponent<MeshRenderer>();
        // Un matériau par sous-mesh pour que tout le mesh soit contouré
        var materials = new Material[mesh.subMeshCount];
        for (int i = 0; i < materials.Length; i++)
            materials[i] = _outlineMaterial;
        outlineRenderer.sharedMaterials = materials;
        outlineRenderer.shadowCastingMode = ShadowCastingMode.Off;
        outlineRenderer.receiveShadows = false;
    }

    /// <summary>
    /// Copie du mesh avec des normales lissées : sur un mesh à arêtes vives, les sommets d'un coin
    /// ont chacun la normale de leur face, le mesh gonflé s'ouvre aux coins et le contour est cassé.
    /// En moyennant les normales des sommets à la même position, le gonflement reste continu.
    /// La moyenne se fait avec le scale appliqué : sur un objet aplati (scale non uniforme),
    /// une moyenne en espace objet donnerait des normales presque verticales une fois transformées.
    /// Mise en cache : un seul mesh lissé par couple mesh source / scale.
    /// </summary>
    private static Mesh GetSmoothedMesh(Mesh source, Vector3 scale)
    {
        // Le cache statique survit entre deux Play si le domain reload est désactivé,
        // mais les meshes créés pendant le Play précédent ont été détruits : on les recrée
        var key = (source, scale);
        if (_smoothedMeshes.TryGetValue(key, out Mesh smoothed) && smoothed != null)
            return smoothed;

        if (!source.isReadable)
        {
            Debug.LogWarning($"[Outliner] Le mesh {source.name} n'est pas Read/Write : contour sans lissage des normales.");
            return source;
        }

        smoothed = Instantiate(source);
        smoothed.name = source.name + "_Outline";

        Vector3[] vertices = smoothed.vertices;
        Vector3[] normals = smoothed.normals;

        var normalsByPosition = new Dictionary<Vector3, Vector3>();
        for (int i = 0; i < vertices.Length; i++)
        {
            // Normale telle qu'elle sera après le scale (une normale se transforme par l'inverse du scale)
            Vector3 scaledNormal = new Vector3(normals[i].x / scale.x, normals[i].y / scale.y, normals[i].z / scale.z).normalized;
            normalsByPosition.TryGetValue(vertices[i], out Vector3 sum);
            normalsByPosition[vertices[i]] = sum + scaledNormal;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            // Retour en espace objet : le shader réappliquera l'inverse du scale
            Vector3 average = normalsByPosition[vertices[i]].normalized;
            normals[i] = Vector3.Scale(average, scale).normalized;
        }

        smoothed.normals = normals;
        _smoothedMeshes[key] = smoothed;
        return smoothed;
    }

    private void OnDestroy()
    {
        if (_outlineObject != null)
            Destroy(_outlineObject);
    }
}
