using TMPro;
using UnityEngine;

public class FrameRateCounter : MonoBehaviour {
    [SerializeField] TMP_Text display;
    [SerializeField, Range(0.1f, 2f)] float sampleDuration = 1f;

    int frames;
    float duration;

    void Update() {
        var frameDuration = Time.unscaledDeltaTime;
        frames++;
        duration += frameDuration;

        if (!(duration > sampleDuration)) {
            return;
        }
        display.SetText($"FPS\n{frames / duration:0}\n000\n000");
        frames = 0;
        duration = 0;
    }
}
