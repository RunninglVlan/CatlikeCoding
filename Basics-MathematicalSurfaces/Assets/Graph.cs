using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph : MonoBehaviour {
    [SerializeField] Transform pointPrefab = default;
    [SerializeField, Range(10, 100)] int resolution = 10;
    [SerializeField] Functions.Name functionName = Functions.Name.Sine;

    private int Resolution { set; get; }
    private float Step => 2f / Resolution;

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
        functionName = Functions.NextName(functionName);
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
        var function = Functions.Get(functionName);
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
}
