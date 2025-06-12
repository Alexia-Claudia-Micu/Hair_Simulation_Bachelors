using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class VideoSliderController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider scrubSlider;

    private bool isScrubbing = false;

    void Start()
    {
        scrubSlider.onValueChanged.AddListener(OnScrubSliderChanged);

        EventTrigger trigger = scrubSlider.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry downEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        downEntry.callback.AddListener((_) => isScrubbing = true);
        trigger.triggers.Add(downEntry);

        EventTrigger.Entry upEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        upEntry.callback.AddListener((_) => isScrubbing = false);
        trigger.triggers.Add(upEntry);

        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();
    }

    void Update()
    {
        if (videoPlayer.isPrepared && videoPlayer.length > 0 && !isScrubbing)
        {
            scrubSlider.value = (float)(videoPlayer.time / videoPlayer.length);
        }
    }

    private void OnScrubSliderChanged(float value)
    {
        if (isScrubbing && videoPlayer.isPrepared && videoPlayer.length > 0)
        {
            videoPlayer.time = value * videoPlayer.length;
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        scrubSlider.minValue = 0f;
        scrubSlider.maxValue = 1f;
        scrubSlider.value = 0f;
    }
}
