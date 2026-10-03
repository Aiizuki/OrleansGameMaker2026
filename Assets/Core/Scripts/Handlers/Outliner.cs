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

    private static readonly Dictionary<Mesh, Mesh> _smoothedMeshes = new Dictionary<Mesh, Mesh>();

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

        Mesh mesh = GetSmoothedMesh(sourceFilter.sharedMesh);
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
    /// Mise en cache : un seul mesh lissé par mesh source.
    /// </summary>
    private static Mesh GetSmoothedMesh(Mesh source)
    {
        // Le cache statique survit entre deux Play si le domain reload est désactivé,
        // mais les meshes créés pendant le Play précédent ont été détruits : on les recrée
        if (_smoothedMeshes.TryGetValue(source, out Mesh smoothed) && smoothed != null)
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
            normalsByPosition.TryGetValue(vertices[i], out Vector3 sum);
            normalsByPosition[vertices[i]] = sum + normals[i];
        }

        for (int i = 0; i < vertices.Length; i++)
            normals[i] = normalsByPosition[vertices[i]].normalized;

        smoothed.normals = normals;
        _smoothedMeshes[source] = smoothed;
        return smoothed;
    }

    private void OnDestroy()
    {
        if (_outlineObject != null)
            Destroy(_outlineObject);
    }
}
