using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class HairJsonDropdownLoader : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public HairSimFromImported hairSim;

    private TextAsset[] loadedJsons;

    void Start()
    {
        LoadJsonFiles();
        PopulateDropdown();
    }

    void LoadJsonFiles()
    {
        // Load all TextAssets in Resources/HairJsons
        loadedJsons = Resources.LoadAll<TextAsset>("HairJsons");

        if (loadedJsons.Length == 0)
        {
            Debug.LogWarning("No JSON files found in Resources/HairJsons");
        }
    }

    void PopulateDropdown()
    {
        dropdown.ClearOptions();
        List<string> names = new List<string>();

        foreach (var json in loadedJsons)
        {
            names.Add(json.name);
        }

        dropdown.AddOptions(names);

        // Default selection
        if (loadedJsons.Length > 0)
        {
            SelectJson(0);
        }

        dropdown.onValueChanged.AddListener(SelectJson);
    }

    void SelectJson(int index)
    {
        if (index >= 0 && index < loadedJsons.Length)
        {
            hairSim.importedHairJson = loadedJsons[index];
            Debug.Log($"Assigned JSON: {loadedJsons[index].name}");
        }
    }
}
