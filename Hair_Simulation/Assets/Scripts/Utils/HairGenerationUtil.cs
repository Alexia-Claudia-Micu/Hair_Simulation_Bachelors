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
        float fullCircle = Mathf.PI * 2f; // 360 degrees in radians
        float totalCurl = curlFrequency * fullCircle;

        // Dynamically calculate segment length based on curl
        float angularStep = totalCurl / (numberOfVertices - 1); // radians between points
        float dynamicSegmentLength = radius * angularStep; // arc length

        // If diameter is very small (almost straight), fallback to base length
        if (dynamicSegmentLength < baseSegmentLength * 0.3f)
            dynamicSegmentLength = baseSegmentLength * 0.3f;

        for (int i = 0; i < numberOfVertices; i++)
        {
            float phase = i * curlFrequency * Mathf.PI * 2f;
            float offsetX = Mathf.Sin(phase) * radius;
            float offsetZ = Mathf.Cos(phase) * radius;
            Vector3 offset = new Vector3(offsetX, 0, offsetZ);

            float initialAngle = (radius > 0f) ? Mathf.Atan2(offsetZ, offsetX) : 0f;

            Vector3 finalPosition = currentPosition + offset;

            StrandVertex newVertex = new StrandVertex(finalPosition, hairMass, isRoot)
            {
                Angle = initialAngle,
                RestAngle = initialAngle,
                Torque = 0f
            };

            vertices.Add(newVertex);

            currentPosition.y -= dynamicSegmentLength;
            isRoot = false;
        }

        return vertices;
    }

    public static List<StrandVertex> GenerateCurledStrandWithPatternRandomness(
        Vector3 rootPosition,
        float segmentLength,
        int numberOfVertices,
        float baseCurlFrequency,
        float baseCurlDiameter,
        float hairMass,
        float curlVariationFactor = 0.5f // How much variation we want, default to 0 for no randomness
    )
    {
        List<StrandVertex> vertices = new List<StrandVertex>();
        Vector3 currentPosition = rootPosition;
        bool isRoot = true;

        // Random seed for more controlled randomness across all strands (optional)
        System.Random random = new System.Random();

        // Apply subtle variation to frequency and diameter if curlVariationFactor > 0
        for (int i = 0; i < numberOfVertices; i++)
        {
            // If curlVariationFactor is 0, no randomness is applied
            float randomCurlFrequency = baseCurlFrequency;
            float randomCurlDiameter = baseCurlDiameter;
            float initialAngle = 0f;

            if (curlVariationFactor > 0)
            {
                // Apply subtle variation to curl frequency and diameter by a smaller range
                float variationScale = Mathf.Lerp(0.001f, 0.05f, curlVariationFactor); // Small factor based on curlVariationFactor

                randomCurlFrequency = baseCurlFrequency + (float)(random.NextDouble() * variationScale - (variationScale / 2f)); // Subtle variation around base
                randomCurlDiameter = baseCurlDiameter + (float)(random.NextDouble() * variationScale - (variationScale / 2f)); // Subtle variation around base

                // Introduce some randomness in the phase to break uniformity, but in a small range
                float phase = i * randomCurlFrequency * Mathf.PI * 2f + (float)(random.NextDouble() * Mathf.PI * variationScale); // Very small random phase

                float offsetX = Mathf.Sin(phase) * randomCurlDiameter;
                float offsetZ = Mathf.Cos(phase) * randomCurlDiameter;
                Vector3 offset = new Vector3(offsetX, 0, offsetZ);

                // Randomize initial angle for more variation, but within a small range
                initialAngle = Mathf.Atan2(offsetZ, offsetX) + (float)(random.NextDouble() * variationScale - (variationScale / 2f));
            }
            else
            {
                // Without randomness, use the base values directly
                float phase = i * baseCurlFrequency * Mathf.PI * 2f;
                float offsetX = Mathf.Sin(phase) * baseCurlDiameter;
                float offsetZ = Mathf.Cos(phase) * baseCurlDiameter;
                Vector3 offset = new Vector3(offsetX, 0, offsetZ);

                initialAngle = Mathf.Atan2(offsetZ, offsetX);
            }

            Vector3 finalPosition = currentPosition + new Vector3(Mathf.Sin(initialAngle) * randomCurlDiameter, 0, Mathf.Cos(initialAngle) * randomCurlDiameter);

            StrandVertex newVertex = new StrandVertex(finalPosition, hairMass, isRoot)
            {
                Angle = initialAngle,
                RestAngle = initialAngle,
                Torque = 0f
            };

            vertices.Add(newVertex);
            currentPosition.y -= segmentLength;
            isRoot = false;
        }

        return vertices;
    }
    public static List<StrandVertex> GenerateGraduallyCurledStrand(
    Vector3 rootPosition,
    float segmentLength,
    int numberOfVertices,
    float curlFrequency,
    float curlDiameter,
    float hairMass
)
    {
        List<StrandVertex> vertices = new List<StrandVertex>();
        Vector3 currentPosition = rootPosition;
        bool isRoot = true;

        // Start with exaggerated frequency and small diameter
        float startingFrequency = curlFrequency * 0.25f;
        float startingDiameter = curlDiameter * 0.1f;

        for (int i = 0; i < numberOfVertices; i++)
        {
            float t = (float)i / (numberOfVertices - 1); // normalized 0–1

            // Interpolate frequency and diameter
            float currentFrequency = Mathf.Lerp(startingFrequency, curlFrequency, t);
            float currentDiameter = Mathf.Lerp(startingDiameter, curlDiameter, t);

            // Compute phase based on vertex index using current frequency
            float phase = i * currentFrequency * Mathf.PI * 2f;
            float offsetX = Mathf.Sin(phase) * currentDiameter;
            float offsetZ = Mathf.Cos(phase) * currentDiameter;
            Vector3 offset = new Vector3(offsetX, 0, offsetZ);

            float initialAngle = (currentDiameter > 0f) ? Mathf.Atan2(offsetZ, offsetX) : 0f;

            Vector3 finalPosition = currentPosition + offset;

            StrandVertex newVertex = new StrandVertex(finalPosition, hairMass, isRoot)
            {
                Angle = initialAngle,
                RestAngle = initialAngle,
                Torque = 0f
            };

            vertices.Add(newVertex);
            currentPosition.y -= segmentLength;
            isRoot = false;
        }

        return vertices;
    }

    public static List<StrandVertex> GenerateOrganicCurledStrand(
        Vector3 rootPosition,
        float baseSegmentLength,
        int numberOfVertices,
        float baseCurlFrequency,
        float baseCurlDiameter,
        float hairMass,
        float curlVariationFactor = 0.1f // 0 = no randomness, 1 = max variation
    )
    {
        List<StrandVertex> vertices = new List<StrandVertex>();
        Vector3 currentPosition = rootPosition;
        bool isRoot = true;

        System.Random random = new System.Random();

        // Randomly flip curl direction per strand
        int directionMultiplier = (random.NextDouble() > 0.5) ? 1 : -1;

        // Start with smaller, ramp-up curl
        float startFrequency = baseCurlFrequency * 0.25f;
        float startDiameter = baseCurlDiameter * 0.01f;

        float radius = baseCurlDiameter;
        float fullCircle = Mathf.PI * 2f; // 360 degrees
        float totalCurl = baseCurlFrequency * fullCircle;

        // Dynamically calculate segment length based on base curl
        float angularStep = totalCurl / (numberOfVertices - 1); // radians between points
        float dynamicSegmentLength = radius * angularStep; // arc length

        // Fallback: avoid segment being too small
        if (dynamicSegmentLength < baseSegmentLength * 0.3f)
            dynamicSegmentLength = baseSegmentLength * 0.3f;

        for (int i = 0; i < numberOfVertices; i++)
        {
            float t = (float)i / (numberOfVertices - 1);

            // Gradually interpolate curl tightness
            float frequency = Mathf.Lerp(startFrequency, baseCurlFrequency, t);
            float diameter = Mathf.Lerp(startDiameter, baseCurlDiameter, t);

            // Apply random variation if needed
            if (curlVariationFactor > 0f)
            {
                float variationScale = Mathf.Lerp(0.001f, 0.05f, curlVariationFactor);
                frequency += (float)(random.NextDouble() * variationScale - variationScale / 2f);
                diameter += (float)(random.NextDouble() * variationScale - variationScale / 2f);
            }

            // Apply direction and random phase offset
            float phase = i * frequency * Mathf.PI * 2f * directionMultiplier;
            if (curlVariationFactor > 0f)
            {
                phase += (float)(random.NextDouble() * Mathf.PI * 0.05f);
            }

            float offsetX = Mathf.Sin(phase) * diameter;
            float offsetZ = Mathf.Cos(phase) * diameter;
            Vector3 offset = new Vector3(offsetX, 0, offsetZ);

            float angle = (diameter > 0f) ? Mathf.Atan2(offsetZ, offsetX) : 0f;

            Vector3 finalPosition = currentPosition + offset;

            StrandVertex newVertex = new StrandVertex(finalPosition, hairMass, isRoot)
            {
                Angle = angle,
                RestAngle = angle,
                Torque = 0f
            };

            vertices.Add(newVertex);

            currentPosition.y -= dynamicSegmentLength; // Use dynamic segment length
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

            // Directional angle estimation (between this and next point)
            float angle = 0f;
            if (i < vertexPositions.Count - 1)
            {
                Vector3 dir = vertexPositions[i + 1] - vertexPositions[i];
                angle = Mathf.Atan2(dir.z, dir.x); // assumes XZ plane curl
            }
            else if (i > 0)
            {
                Vector3 dir = vertexPositions[i] - vertexPositions[i - 1];
                angle = Mathf.Atan2(dir.z, dir.x);
            }

            StrandVertex newVertex = new StrandVertex(vertexPositions[i], hairMass, isRoot)
            {
                Angle = angle,
                RestAngle = angle,
                Torque = 0f
            };

            vertices.Add(newVertex);
        }

        return vertices;
    }


}