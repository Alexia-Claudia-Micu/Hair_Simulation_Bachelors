using UnityEngine;
using UnityEngine.UI;

public class ImageCarouselController : MonoBehaviour
{
    [Header("UI References")]
    public Image imageDisplay;
    public Button leftArrow;
    public Button rightArrow;

    [Header("Image Source Folder (Resources/)")]
    public string resourceFolder = "SimulationImages";

    private Sprite[] loadedImages;
    private int currentIndex = 0;

    void Start()
    {
        loadedImages = Resources.LoadAll<Sprite>(resourceFolder);

        if (loadedImages.Length == 0)
        {
            Debug.LogWarning("No images found in Resources/" + resourceFolder);
            imageDisplay.enabled = false;
            return;
        }

        ShowImage(0);

        leftArrow.onClick.AddListener(() => ShowPrevious());
        rightArrow.onClick.AddListener(() => ShowNext());
    }

    void ShowImage(int index)
    {
        currentIndex = Mathf.Clamp(index, 0, loadedImages.Length - 1);
        imageDisplay.sprite = loadedImages[currentIndex];
        imageDisplay.enabled = true;
    }

    void ShowPrevious()
    {
        int newIndex = (currentIndex - 1 + loadedImages.Length) % loadedImages.Length;
        ShowImage(newIndex);
    }

    void ShowNext()
    {
        int newIndex = (currentIndex + 1) % loadedImages.Length;
        ShowImage(newIndex);
    }
}
