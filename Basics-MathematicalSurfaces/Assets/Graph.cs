using NaughtyAttributes;
using System;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

public class Graph : MonoBehaviour {

    private static readonly Func<float, float, float, float>[] functions = { Sine, Sine2D, MultiSine, MultiSine2D };

    [SerializeField] Transform pointPrefab = default;
    [Range(10, 100)]
    [SerializeField] int resolution = 10;

    private int currentFunction = 0;

    void Awake() => Visualize();

    [Button]
    void Visualize() {
        Clear();
        var step = 2f / resolution;
        var scale = Vector3.one * step;
        var position = Vector3.zero;
        foreach (var z in Enumerable.Range(-resolution / 2, resolution)) {
            position.z = (z + .5f) * step;
            foreach (var x in Enumerable.Range(-resolution / 2, resolution)) {
                var point = Instantiate(pointPrefab, transform);
                point.localScale = scale;
                position.x = (x + .5f) * step;
                point.localPosition = position;
            }
        }

        void Clear() {
            foreach (Transform point in transform) {
                Destroy(point.gameObject);
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            currentFunction = currentFunction + 1 < functions.Length ? currentFunction + 1 : 0;
        }
        Animate();
    }

    private void Animate() {
        var time = Time.time;
        var function = functions[currentFunction];
        foreach (Transform point in transform) {
            var position = point.position;
            position.y = function(position.x, position.z, time);
            point.localPosition = position;
        }
    }

    private static float Sine(float x, float _, float time) => Mathf.Sin(PI * (x + time));

    private static float Sine2D(float x, float z, float time) => (Sine(x, 0, time) + Sine(z, 0, time)) * .5f;

    private static float MultiSine(float x, float _, float time) {
        var y = Sine(x, _, time);
        y += Mathf.Sin(2 * PI * (x + 2 * time)) / 2;
        y *= 2f / 3;
        return y;
    }

    private static float MultiSine2D(float x, float z, float time) {
        var y = 4f * Mathf.Sin(PI * (x + z + time * .5f));
        y += Sine(x, 0, time);
        y += Mathf.Sin(2f * PI * (z + 2f * time)) * .5f;
        y *= 1f / 5.5f;
        return y;
    }
}
