using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class FrameRateCounter : MonoBehaviour {
    [SerializeField] TMP_Text display;
    [SerializeField, Range(0.1f, 2f)] float sampleDuration = 1f;
    [SerializeField] DisplayMode displayMode = DisplayMode.FPS;

    int frames;
    float duration, bestDuration = float.MaxValue, worstDuration;

    void Update() {
        var frameDuration = Time.unscaledDeltaTime;
        frames++;
        duration += frameDuration;
        bestDuration = math.min(frameDuration, bestDuration);
        worstDuration = math.max(frameDuration, worstDuration);

        if (!(duration > sampleDuration)) {
            return;
        }
        display.SetText(displayMode == DisplayMode.FPS
            ? @$"FPS
{1 / bestDuration:0}
{frames / duration:0}
{1 / worstDuration:0}"
            : @$"MS
{1000 * bestDuration:F1}
{1000 * duration / frames:F1}
{1000 * worstDuration:F1}");
        frames = 0;
        duration = 0;
        bestDuration = float.MaxValue;
        worstDuration = 0;
    }

    enum DisplayMode {
        FPS,
        MS
    }
}
