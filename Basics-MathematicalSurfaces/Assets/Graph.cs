using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

public class Graph : MonoBehaviour {

    private static readonly Func<float, float, float, Vector3>[] functions = {
        Sine, Sine2D, MultiSine, MultiSine2D, Ripple, Cylinder, Sphere, Torus
    };

    [SerializeField] Transform pointPrefab = default;
    [Range(10, 100)]
    [SerializeField] int resolution = 10;

    private int Resolution { set; get; }
    private float Step => 2f / Resolution;

    private int currentFunction = 0;
    private Dictionary<(float u, float v), Transform> points = new Dictionary<(float, float), Transform>();

    void Awake() => Initialize();

    [Button]
    void Initialize() {
        Resolution = resolution;
        Clear();
        var scale = Vector3.one * Step;
        IteratePoints((u, v) => {
            var point = Instantiate(pointPrefab, transform);
            point.localScale = scale;
            points[(u, v)] = point;
        });

        void Clear() {
            foreach (Transform point in transform) {
                Destroy(point.gameObject);
            }
        }
    }

    public void bChangeFunction() {
        currentFunction = currentFunction + 1 < functions.Length ? currentFunction + 1 : 0;
    }

    public void bChangeResolution(int delta) {
        var newResolution = Mathf.Clamp(resolution + delta, 10, 100);
        if (resolution == newResolution) {
            return;
        }
        resolution = newResolution;
        Initialize();
    }

    void Update() => Animate();

    private void Animate() {
        var time = Time.time;
        var function = functions[currentFunction];
        IteratePoints((u, v) => {
            points[(u, v)].localPosition = function(u, v, time);
        });
    }

    private void IteratePoints(Action<float, float> action) {
        var step = Step;
        foreach (var z in Enumerable.Range(-Resolution / 2, Resolution)) {
            var v = (z + .5f) * step;
            foreach (var x in Enumerable.Range(-Resolution / 2, Resolution)) {
                var u = (x + .5f) * step;
                action(u, v);
            }
        }
    }

    private static Vector3 Sine(float x, float z, float time) {
        var y = Sine(x, time);
        return new Vector3(x, y, z);
    }

    private static Vector3 Sine2D(float x, float z, float time) {
        var y = (Sine(x, time) + Sine(z, time)) * .5f;
        return new Vector3(x, y, z);
    }

    private static Vector3 MultiSine(float x, float z, float time) {
        var y = Sine(x, time);
        y += Mathf.Sin(2 * PI * (x + 2 * time)) / 2;
        y *= 2f / 3;
        return new Vector3(x, y, z);
    }

    private static Vector3 MultiSine2D(float x, float z, float time) {
        var y = 4f * Mathf.Sin(PI * (x + z + time * .5f));
        y += Sine(x, time);
        y += Mathf.Sin(2f * PI * (z + 2f * time)) * .5f;
        y *= 1f / 5.5f;
        return new Vector3(x, y, z);
    }

    private static Vector3 Ripple(float x, float z, float time) {
        const float multiplier = 1.3f;
        var distance = Mathf.Sqrt(x * x + z * z);
        var y = Mathf.Sin(4 * (PI * distance - time));
        y /= 1 + 10 * distance;
        return new Vector3(x * multiplier, y, z * multiplier);
    }

    private static Vector3 Cylinder(float u, float v, float time) {
        const float multiplier = 1.3f;
        var radius = .8f + Sine(6 * u + 2 * v, time) * .2f;
        return new Vector3(radius * Sine(u) * multiplier, v, radius * Cos(u) * multiplier);
    }

    private static Vector3 Sphere(float u, float v, float time) {
        const float multiplier = 1.3f;
        var radius = .8f + Sine(6 * u, time) * .1f;
        radius += Sine(4 * v, time) * .1f;
        var s = radius * Cos(.5f * v);
        return new Vector3(s * Sine(u) * multiplier, radius * Sine(.5f * v) * multiplier, s * Cos(u) * multiplier);
    }

    private static Vector3 Torus(float u, float v, float time) {
        const float multiplier = 1.5f;
        var radius1 = .65f + Sine(6 * u, time) * .1f;
        var radius2 = .2f + Sine(4 * v, time) * .05f;
        var s = radius2 * Cos(v) + radius1;
        return new Vector3(s * Sine(u) * multiplier, radius2 * Sine(v) * multiplier, s * Cos(u) * multiplier);
    }

    private static float Sine(float angle, float time = 0) => Mathf.Sin(PI * (angle + time));
    private static float Cos(float angle, float time = 0) => Mathf.Cos(PI * (angle + time));
}
