using System;
using UnityEngine;

public class Clock : MonoBehaviour {

    private const float DEGREES_PER_HOUR = 30;
    private const float DEGREES = 6;

    [SerializeField]
    Transform hoursTransform = default,
        minutesTransform = default,
        secondsTransform = default;

    void Update() {
        var currentTime = DateTime.Now.TimeOfDay;
        hoursTransform.localRotation = Quaternion.Euler(0, (float)currentTime.TotalHours * DEGREES_PER_HOUR, 0);
        minutesTransform.localRotation = Quaternion.Euler(0, (float)currentTime.TotalMinutes * DEGREES, 0);
        secondsTransform.localRotation = Quaternion.Euler(0, currentTime.Seconds * DEGREES, 0);
    }
}
