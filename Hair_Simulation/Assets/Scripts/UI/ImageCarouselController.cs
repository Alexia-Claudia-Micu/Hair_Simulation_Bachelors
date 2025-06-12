using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class ImageCarouselController : MonoBehaviour
{
    [Header("UI References")]
    public Image imageDisplay;
    public Button leftArrow;
    public Button rightArrow;

    [Header("Video Playback")]
    public GameObject videoPanel;
    public VideoPlayer videoPlayer;
    public Button playButton;
    public Button pauseButton;
    public TMP_Text currentTimeText;
    public TMP_Text totalTimeText;

    [Header("Image Source Folder (Resources/)")]
    public string resourceFolder = "SimulationImages";

    [Header("Video Source Folder (Resources/)")]
    public string videoFolder = "SimulationVideos";

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

        leftArrow.onClick.AddListener(ShowPrevious);
        rightArrow.onClick.AddListener(ShowNext);

        var imageButton = imageDisplay.GetComponent<Button>();
        if (imageButton != null)
        {
            imageButton.onClick.AddListener(OnImageClicked);
        }
        else
        {
            Debug.LogWarning("Image Display should have a Button component.");
        }

        if (videoPanel != null)
        {
            videoPanel.SetActive(false);
        }

        playButton.onClick.AddListener(() => videoPlayer.Play());
        pauseButton.onClick.AddListener(() => videoPlayer.Pause());
    }

    void Update()
    {
        if (videoPlayer.clip == null) return;

        currentTimeText.text = FormatTime(videoPlayer.time);

        bool isPlaying = videoPlayer.isPlaying;
        playButton.gameObject.SetActive(!isPlaying);
        pauseButton.gameObject.SetActive(isPlaying);
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

    void OnImageClicked()
    {
        string videoName = loadedImages[currentIndex].name;
        Debug.Log($"Loading video: {videoName}");

        VideoClip clip = Resources.Load<VideoClip>($"{videoFolder}/{videoName}");

        if (clip != null)
        {
            videoPanel.SetActive(true);
            videoPlayer.clip = clip;
            videoPlayer.time = 0;
            videoPlayer.Play();
            totalTimeText.text = FormatTime(clip.length);
        }
        else
        {
            Debug.LogWarning($"Video '{videoName}' not found in Resources/{videoFolder}");
        }
    }

    public void CloseVideo()
    {
        videoPlayer.Stop();
        videoPanel.SetActive(false);
    }

    string FormatTime(double time)
    {
        int minutes = Mathf.FloorToInt((float)time / 60f);
        int seconds = Mathf.FloorToInt((float)time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
