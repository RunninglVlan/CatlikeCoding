using UnityEngine;

public class GPUGraph : MonoBehaviour {
    [SerializeField, Range(10, 200)] int resolution = 10;
    [SerializeField] Functions.Name functionName = Functions.Name.Sine;
    [SerializeField] float transitionDuration = 1;

    int Resolution { set; get; }
    float Step => 2f / Resolution;

    Functions.Name previousFunctionName;
    float transitionTime;

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
