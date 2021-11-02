using NaughtyAttributes;
using UnityEngine;

public class GPUGraph : MonoBehaviour {
    [SerializeField] Transform pointPrefab;
    [SerializeField, Range(10, 200)] int resolution = 10;
    [SerializeField] Functions.Name functionName = Functions.Name.Sine;
    [SerializeField] float transitionDuration = 1;

    int Resolution { set; get; }
    float Step => 2f / Resolution;

    Transform[] points;
    Functions.Name previousFunctionName;
    float transitionTime;

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

    public void NextFunction() {
        previousFunctionName = functionName;
        functionName = Functions.NextName(functionName);
        transitionTime = transitionDuration;
    }

    public void RandomFunction() {
        previousFunctionName = functionName;
        functionName = Functions.RandomNameOtherThan(functionName);
        transitionTime = transitionDuration;
    }

    public void ChangeResolution(int delta) {
        var newResolution = Mathf.Clamp(resolution + delta, 10, 200);
        if (resolution == newResolution) {
            return;
        }
        resolution = newResolution;
        Initialize();
    }

    void Update() => Animate();

    void Animate() {
        var time = Time.time;
        var function = Functions.Get(functionName);
        var transitioning = Transitioning(out var previousFunction, out var transitionProgress);
        var step = Step;
        var index = 0;
        for (var z = -Resolution / 2; z < Resolution / 2; z++) {
            var v = (z + .5f) * step;
            for (var x = -Resolution / 2; x < Resolution / 2; x++) {
                var u = (x + .5f) * step;
                points[index++].localPosition = !transitioning
                    ? function(u, v, time)
                    : Functions.Morph(u, v, time, previousFunction, function, transitionProgress);
            }
        }

        bool Transitioning(out Functions.Function from, out float progress) {
            from = null;
            var result = transitionTime > 0;
            if (result) {
                from = Functions.Get(previousFunctionName);
                transitionTime -= Time.deltaTime;
            }
            progress = (transitionDuration - transitionTime) / transitionDuration;
            return result;
        }
    }
}
