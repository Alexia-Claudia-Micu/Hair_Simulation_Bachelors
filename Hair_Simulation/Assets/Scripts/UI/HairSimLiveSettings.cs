using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HairSimSettingsUI : MonoBehaviour
{
    [Header("References")]
    public HairSimCore hairSim;

    [Header("Input Fields")]
    public TMP_InputField followerCountInput;
    public TMP_InputField spawnRadiusInput;
    public TMP_InputField rootThicknessInput;
    public TMP_InputField tipThicknessInput;

    [Header("Slider")]
    public Slider taperSlider;
    public TMP_Text taperValueLabel; // Optional: shows slider value

    void Start()
    {
        if (hairSim == null)
        {
            Debug.LogError("HairSim reference not assigned.");
            return;
        }

        SyncWithSim();

        // Initialize UI with current HairSim values
        followerCountInput.text = hairSim.followerCount.ToString();
        spawnRadiusInput.text = hairSim.spawnRadius.ToString("F4");
        rootThicknessInput.text = hairSim.rootThickness.ToString("F4");
        tipThicknessInput.text = hairSim.tipThickness.ToString("F4");

        taperSlider.value = hairSim.taperAmount;
        if (taperValueLabel != null)
            taperValueLabel.text = hairSim.taperAmount.ToString("F2");
        // Add listeners
        followerCountInput.onEndEdit.AddListener(OnFollowerCountChanged);
        spawnRadiusInput.onEndEdit.AddListener(OnSpawnRadiusChanged);
        rootThicknessInput.onEndEdit.AddListener(OnRootThicknessChanged);
        tipThicknessInput.onEndEdit.AddListener(OnTipThicknessChanged);
        taperSlider.onValueChanged.AddListener(OnTaperChanged);
    }

    public void SyncWithSim()
    {
        if (hairSim == null) return;

        followerCountInput.text = hairSim.followerCount.ToString();
        spawnRadiusInput.text = hairSim.spawnRadius.ToString("F4");
        rootThicknessInput.text = hairSim.rootThickness.ToString("F4");
        tipThicknessInput.text = hairSim.tipThickness.ToString("F4");

        taperSlider.value = hairSim.taperAmount;
        if (taperValueLabel != null)
            taperValueLabel.text = hairSim.taperAmount.ToString("F2");
    }


    void OnFollowerCountChanged(string value)
    {
        if (int.TryParse(value, out int result))
        {
            hairSim.followerCount = Mathf.Max(0, result);
        }
    }

    void OnSpawnRadiusChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            hairSim.spawnRadius = Mathf.Max(0f, result);
        }
    }

    void OnRootThicknessChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            hairSim.rootThickness = Mathf.Max(0f, result);
        }
    }

    void OnTipThicknessChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            hairSim.tipThickness = Mathf.Max(0f, result);
        }
    }

    void OnTaperChanged(float value)
    {
        hairSim.taperAmount = Mathf.Clamp01(value);
        if (taperValueLabel != null)
            taperValueLabel.text = value.ToString("F2");
    }
}
