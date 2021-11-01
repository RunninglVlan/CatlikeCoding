using TMPro;
using UnityEngine;

public class FrameRateCounter : MonoBehaviour {
    [SerializeField] TMP_Text display;

    void Update() {
        var frameDuration = Time.unscaledDeltaTime;
        display.SetText($"FPS\n{1 / frameDuration:0}\n000\n000");
    }
}
