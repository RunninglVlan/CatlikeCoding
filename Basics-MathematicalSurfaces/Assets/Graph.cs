using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

public class Graph : MonoBehaviour {
    [SerializeField] Transform pointPrefab = default;
    [SerializeField, Range(10, 100)] int resolution = 10;
    [SerializeField] Functions.Name functionName = Functions.Name.Sine;

    private int Resolution { set; get; }
    private float Step => 2f / Resolution;

    private Transform[] points;

    void Awake() => Initialize();

    [Button]
    void Initialize() {
        Resolution = resolution;
        Clear();
        var scale = Vector3.one * Step;
        points = new Transform[Resolution * Resolution];
        for (var index = 0; index < points.Length; index++) {
            var point = Instantiate(pointPrefab, transform);
            point.localScale = scale;
            points[index] = point;
        }

        void Clear() {
            foreach (Transform point in transform) {
                Destroy(point.gameObject);
            }
        }
    }

    [UsedImplicitly]
    public void NextFunction() {
        functionName = Functions.NextName(functionName);
    }

    [UsedImplicitly]
    public void RandomFunction() {
        functionName = Functions.RandomNameOtherThan(functionName);
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
        var step = Step;
        var index = 0;
        for (var z = -Resolution / 2; z < Resolution / 2; z++) {
            var v = (z + .5f) * step;
            for (var x = -Resolution / 2; x < Resolution / 2; x++) {
                var u = (x + .5f) * step;
                points[index++].localPosition = function(u, v, time);
            }
        }
    }
}
