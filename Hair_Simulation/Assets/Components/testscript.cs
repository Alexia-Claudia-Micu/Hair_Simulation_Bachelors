using UnityEngine;

public class SurfaceLineDrawer : MonoBehaviour
{
    public GameObject targetObject; // Assign the object you want to get a surface point from
    public float lineLength = 2.0f; // Length of the line
    public Material lineMaterial; // Optional: Assign a material for the line

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("Target object is not assigned!");
            return;
        }

        Collider collider = targetObject.GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("Target object does not have a collider!");
            return;
        }

        // Get a random surface point
        Vector3 surfacePoint = GetRandomSurfacePoint(collider);

        // Approximate the outward normal direction
        Vector3 normalDirection = (surfacePoint - collider.bounds.center).normalized;

        // Draw the line
        DrawLine(surfacePoint, surfacePoint + normalDirection * lineLength);
    }

    Vector3 GetRandomSurfacePoint(Collider collider)
    {
        Bounds bounds = collider.bounds;
        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 startPoint = bounds.center + randomDirection * bounds.extents.magnitude; // Outside the collider

        RaycastHit hit;
        if (Physics.Raycast(startPoint, -randomDirection, out hit, bounds.extents.magnitude * 2))
        {
            return hit.point; // Returns the first hit point on the surface
        }

        return collider.transform.position; // Fallback: Return object center if no hit
    }

    void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject lineObject = new GameObject("SurfaceLine");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Set line properties
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;

        // Assign material if provided
        if (lineMaterial != null)
        {
            lineRenderer.material = lineMaterial;
        }
    }
}
