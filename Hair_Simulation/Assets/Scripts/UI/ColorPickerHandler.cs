using UnityEngine;
using Xenia.ColorPicker;

public class ColorPickerHandler : MonoBehaviour
{
    public Material targetMaterial;
    public ColorPicker colorPicker;

    void Start()
    {
        if (colorPicker != null)
        {
            colorPicker.ColorChanged.AddListener(UpdateMaterialColor);
        }
    }

    void UpdateMaterialColor(Color newColor)
    {
        if (targetMaterial != null)
        {
            targetMaterial.color = newColor;
        }
    }
}
