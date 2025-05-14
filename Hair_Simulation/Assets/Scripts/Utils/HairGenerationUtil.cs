using System.Collections.Generic;
using UnityEngine;

public static class HairGenerationUtil
{
    public static List<StrandVertex> GenerateCurledStrand(
        Vector3 rootPosition,
        float baseSegmentLength,
        int numberOfVertices,
        float curlFrequency,
        float curlDiameter,
        float hairMass
    )
    {
        List<StrandVertex> vertices = new List<StrandVertex>();
        Vector3 currentPosition = rootPosition;
        bool isRoot = true;

        float radius = curlDiameter;
        float fullCircle = Mathf.PI * 2f;
        float totalCurl = curlFrequency * fullCircle;

        float angularStep = totalCurl / (numberOfVertices - 1);
        float dynamicSegmentLength = radius * angularStep;

        if (dynamicSegmentLength < baseSegmentLength * 0.3f)
            dynamicSegmentLength = baseSegmentLength * 0.3f;

        for (int i = 0; i < numberOfVertices; i++)
        {
            float phase = i * curlFrequency * Mathf.PI * 2f;
            float offsetX = Mathf.Sin(phase) * radius;
            float offsetZ = Mathf.Cos(phase) * radius;
            Vector3 offset = new Vector3(offsetX, 0, offsetZ);

            Vector3 finalPosition = currentPosition + offset;

            // Direction to define initial curl rotation
            Vector3 curlDirection = offset.normalized;
            Quaternion initialRotation = curlDirection.sqrMagnitude > 0f
                ? Quaternion.LookRotation(curlDirection, Vector3.up)
                : Quaternion.identity;

            StrandVertex newVertex = new StrandVertex(finalPosition, hairMass, isRoot)
            {
                Rotation = initialRotation,
                RestRotation = initialRotation,
                Torque = Vector3.zero
            };

            vertices.Add(newVertex);
            currentPosition.y -= dynamicSegmentLength;
            isRoot = false;
        }

        return vertices;
    }

    public static List<StrandVertex> GenerateFromImportedVertices(List<Vector3> vertexPositions, float hairMass)
    {
        List<StrandVertex> vertices = new List<StrandVertex>();
        if (vertexPositions.Count < 2)
            return vertices;

        for (int i = 0; i < vertexPositions.Count; i++)
        {
            bool isRoot = (i == 0);
            Quaternion initialRotation = Quaternion.identity;

            // Estimate direction of segment
            if (i < vertexPositions.Count - 1)
            {
                Vector3 dir = vertexPositions[i + 1] - vertexPositions[i];
                if (dir.sqrMagnitude > 0f)
                    initialRotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
            }
            else if (i > 0)
            {
                Vector3 dir = vertexPositions[i] - vertexPositions[i - 1];
                if (dir.sqrMagnitude > 0f)
                    initialRotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
            }

            StrandVertex newVertex = new StrandVertex(vertexPositions[i], hairMass, isRoot)
            {
                Rotation = initialRotation,
                RestRotation = initialRotation,
                Torque = Vector3.zero
            };

            vertices.Add(newVertex);
        }

        return vertices;
    }
}
