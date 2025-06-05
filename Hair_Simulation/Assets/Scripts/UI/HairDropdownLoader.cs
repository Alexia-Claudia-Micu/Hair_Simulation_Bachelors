using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public enum JsonTarget
{
    HairStrands,
    Settings
}


public class JsonDropdownLoader : MonoBehaviour
{
    [Header("Dropdown UI")]
    public TMP_Dropdown dropdown;
    public Button loadButton;

    [Header("Configuration")]
    public string resourcesFolder = "HairJsons";
    public string sceneToLoad = "";
    public JsonTarget targetVariable;

    private TextAsset[] loadedJsons;

    void Start()
    {
        LoadJsonFiles();
        PopulateDropdown();

        if (loadButton != null)
            loadButton.onClick.AddListener(LoadScene);
    }

    void LoadJsonFiles()
    {
        loadedJsons = Resources.LoadAll<TextAsset>(resourcesFolder);

        if (loadedJsons.Length == 0)
        {
            Debug.LogWarning($"No JSON files found in Resources/{resourcesFolder}");
        }
    }

    void PopulateDropdown()
    {
        dropdown.ClearOptions();
        List<string> names = new List<string>();

        foreach (var json in loadedJsons)
            names.Add(json.name);

        dropdown.AddOptions(names);

        if (loadedJsons.Length > 0)
            SelectJson(0);

        dropdown.onValueChanged.AddListener(SelectJson);
    }

    void SelectJson(int index)
    {
        if (index >= 0 && index < loadedJsons.Length)
        {
            var selected = loadedJsons[index];
            Debug.Log($"Selected JSON: {selected.name}");

            switch (targetVariable)
            {
                case JsonTarget.HairStrands:
                    InterSceneStatics.SelectedHairJson = selected;
                    break;
                case JsonTarget.Settings:
                    InterSceneStatics.SelectedSettingsJson = selected;
                    break;
            }
        }
    }

    void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadScene(sceneToLoad);
    }
}
