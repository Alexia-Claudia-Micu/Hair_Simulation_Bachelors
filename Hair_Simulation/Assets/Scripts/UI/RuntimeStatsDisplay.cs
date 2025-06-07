using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeStatsDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text textPrefab;
    public Transform layoutGroupTransform;

    private Dictionary<string, TMP_Text> statTexts = new();
    private float smoothedDeltaTime = 0.016f;
    private float lastFps = 0f;
    private float lastCpu = 0f;
    private float statUpdateTimer = 0f;

    private const float updateInterval = 0.5f;
    private const float fpsChangeThreshold = 10f;
    private const float cpuChangeThreshold = 2f;

    void Start()
    {
        AddStat("FPS");
        AddStat("CPU (ms)");
        AddStat("Tris");
        AddStat("Verts");
    }

    void AddStat(string statName)
    {
        TMP_Text statText = Instantiate(textPrefab, layoutGroupTransform);
        statText.text = $"{statName}: ...";
        statTexts[statName] = statText;
    }

    void Update()
    {
        // --- Smoothing for FPS/CPU ---
        smoothedDeltaTime = Mathf.Lerp(smoothedDeltaTime, Time.unscaledDeltaTime, 0.05f);

        float currentFps = Mathf.Clamp(1f / smoothedDeltaTime, 1f, 999f);
        float currentCpu = Mathf.Clamp(smoothedDeltaTime * 1000f, 0.01f, 200f);
        statUpdateTimer += Time.unscaledDeltaTime;

        // --- Update FPS if needed ---
        if ((Mathf.Abs(currentFps - lastFps) >= fpsChangeThreshold) || statUpdateTimer >= updateInterval)
        {
            if (statTexts.TryGetValue("FPS", out var fpsText))
                fpsText.text = $"FPS: {Mathf.RoundToInt(currentFps)}";
            lastFps = currentFps;
        }

        // --- Update CPU if needed ---
        if ((Mathf.Abs(currentCpu - lastCpu) >= cpuChangeThreshold) || statUpdateTimer >= updateInterval)
        {
            if (statTexts.TryGetValue("CPU (ms)", out var cpuText))
                cpuText.text = $"CPU (ms): {currentCpu:F1}";
            lastCpu = currentCpu;
        }

        // Tris & Verts (Editor only)
#if UNITY_EDITOR
        int tris = UnityEditor.UnityStats.triangles;
        int verts = UnityEditor.UnityStats.vertices;

        if (statTexts.TryGetValue("Tris", out var trisText))
            trisText.text = $"Tris: {FormatCount(tris)}";

        if (statTexts.TryGetValue("Verts", out var vertsText))
            vertsText.text = $"Verts: {FormatCount(verts)}";
#else
        // If you want runtime approximations, insert logic here
        if (statTexts.TryGetValue("Tris", out var trisText))
            trisText.text = $"Tris: N/A";
        if (statTexts.TryGetValue("Verts", out var vertsText))
            vertsText.text = $"Verts: N/A";
#endif

        if (statUpdateTimer >= updateInterval)
            statUpdateTimer = 0f;
    }

    string FormatCount(int count)
    {
        if (count >= 1_000_000)
            return $"{(count / 1_000_000f):F1}M";
        else if (count >= 1_000)
            return $"{(count / 1_000f):F1}K";
        else
            return count.ToString();
    }
}