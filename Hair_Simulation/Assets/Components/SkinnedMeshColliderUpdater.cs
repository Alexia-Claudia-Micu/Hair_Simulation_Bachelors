using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class SkinnedMeshColliderUpdater : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedRenderer;
    private MeshCollider meshCollider;
    private Mesh bakedMesh;

    void Awake()
    {
        skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        bakedMesh = new Mesh();
        bakedMesh.name = "BakedSkinnedMesh";
        bakedMesh.MarkDynamic();
    }

    void LateUpdate()
    {
        if (skinnedRenderer == null || meshCollider == null)
            return;

        bakedMesh.Clear();
        skinnedRenderer.BakeMesh(bakedMesh);

        if (bakedMesh.vertexCount == 0)
        {
            Debug.LogWarning($"[{name}] Baked mesh is empty.");
            return;
        }

        // Apply transform correction to baked mesh
        var vertices = bakedMesh.vertices;
        var worldToLocal = transform.worldToLocalMatrix;
        var skinnedToWorld = skinnedRenderer.transform.localToWorldMatrix;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPos = skinnedToWorld.MultiplyPoint3x4(vertices[i]);
            vertices[i] = worldToLocal.MultiplyPoint3x4(worldPos);
        }

        bakedMesh.vertices = vertices;
        bakedMesh.RecalculateBounds();
        bakedMesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = bakedMesh;

        Physics.SyncTransforms();
    }


    void OnDestroy()
    {
        if (bakedMesh != null)
        {
            Destroy(bakedMesh);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (bakedMesh == null || bakedMesh.vertexCount == 0) return;

        Gizmos.color = Color.cyan;
        Vector3[] vertices = bakedMesh.vertices;
        Vector3[] normals = bakedMesh.normals;

        Transform t = transform;
        int step = Mathf.Max(1, vertices.Length / 200); // avoid overdraw

        for (int i = 0; i < vertices.Length; i += step)
        {
            Vector3 worldPos = t.TransformPoint(vertices[i]);
            Vector3 normalDir = t.TransformDirection(normals[i]);
            Gizmos.DrawRay(worldPos, normalDir * 0.02f);
            Gizmos.DrawSphere(worldPos, 0.001f);
        }
    }
#endif
}
