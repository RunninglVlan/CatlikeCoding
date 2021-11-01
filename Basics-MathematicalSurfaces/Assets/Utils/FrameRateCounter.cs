using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class FrameRateCounter : MonoBehaviour {
    [SerializeField] TMP_Text display;
    [SerializeField, Range(0.1f, 2f)] float sampleDuration = 1f;

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
        display.SetText($"FPS\n{1 / bestDuration:0}\n{frames / duration:0}\n{1 / worstDuration:0}");
        frames = 0;
        duration = 0;
        bestDuration = float.MaxValue;
        worstDuration = 0;
    }
}
