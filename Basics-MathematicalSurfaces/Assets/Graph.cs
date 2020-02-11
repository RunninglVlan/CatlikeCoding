using NaughtyAttributes;
using System;
using System.Linq;
using UnityEngine;

public class Graph : MonoBehaviour {

    [SerializeField] Transform pointPrefab = default;
    [Range(10, 100)]
    [SerializeField] int resolution = 10;

    private Func<float, float, float>[] functions = { Sine, MultiSine };
    private int currentFunction = 0;

    void Awake() => Visualize();

    [Button]
    void Visualize() {
        Clear();
        var step = 2f / resolution;
        var scale = Vector3.one * step;
        var position = Vector3.zero;
        foreach (var index in Enumerable.Range(-resolution / 2, resolution)) {
            var point = Instantiate(pointPrefab, transform);
            point.localScale = scale;
            position.x = (index + .5f) * step;
            point.localPosition = position;
        }

        void Clear() {
            foreach (Transform point in transform) {
                Destroy(point.gameObject);
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            currentFunction = currentFunction + 1 < functions.Length ? currentFunction + 1 : 0;
        }
        Animate();
    }

    private void Animate() {
        var time = Time.time;
        var function = functions[currentFunction];
        foreach (Transform point in transform) {
            var position = point.position;
            position.y = function(position.x, time);
            point.localPosition = position;
        }
    }

    private static float Sine(float x, float time) => Mathf.Sin(Mathf.PI * (x + time));

    private static float MultiSine(float x, float time) {
        var y = Sine(x, time);
        y += Mathf.Sin(2 * Mathf.PI * (x + 2 * time)) / 2;
        y *= 2f / 3;
        return y;
    }
}
