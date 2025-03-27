using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PushOutSphereCollider : MonoBehaviour
{

    public float scaleDuration = 1f; // time in seconds to reach final size

    private float originalRadius;
    private float currentTime;
    private SphereCollider sphereCollider;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        originalRadius = sphereCollider.radius;
        sphereCollider.radius = 0f;
        currentTime = 0f;
    }

    void Update()
    {
        if (sphereCollider == null) return;

        // Gradually scale up radius
        if (currentTime < scaleDuration)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.Clamp01(currentTime / scaleDuration);
            sphereCollider.radius = Mathf.Lerp(0f, originalRadius, t);
        }
    }

    void OnDisable()
    {
        // Reset to original on disable
        if (sphereCollider != null)
            sphereCollider.radius = originalRadius;
    }

    void OnValidate()
    {
        if (sphereCollider == null)
            sphereCollider = GetComponent<SphereCollider>();
    }
}
