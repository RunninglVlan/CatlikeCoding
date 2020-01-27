using System;
using UnityEngine;

public class Clock : MonoBehaviour {

    private const float HOUR_FACTOR = 1 / 120f;
    private const float MINUTE_FACTOR = 1 / 10f;
    private const float SECOND_FACTOR = 6;

    [SerializeField] Transform hoursTransform = default;
    [SerializeField] Transform minutesTransform = default;
    [SerializeField] Transform secondsTransform = default;

    void Update() {
        var currentTime = DateTime.Now;
        hoursTransform.localRotation = Quaternion.Euler(0,
            (HourSeconds() + MinuteSeconds() + currentTime.Second) * HOUR_FACTOR, 0);
        minutesTransform.localRotation = Quaternion.Euler(0,
            (MinuteSeconds() + currentTime.Second) * MINUTE_FACTOR, 0);
        secondsTransform.localRotation = Quaternion.Euler(0,
            currentTime.Second * SECOND_FACTOR, 0);

        int HourSeconds() => currentTime.Hour * 60 * 60;
        int MinuteSeconds() => currentTime.Minute * 60;
    }
}
