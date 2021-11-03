using UnityEngine;

public class GPUGraph : MonoBehaviour {
    static readonly int POINTS = Shader.PropertyToID("Points");
    static readonly int RESOLUTION = Shader.PropertyToID("Resolution");
    static readonly int STEP = Shader.PropertyToID("Step");
    static readonly int TIME = Shader.PropertyToID("Time");

    [SerializeField] ComputeShader functionsShader;
    [SerializeField] Material material;
    [SerializeField] Mesh mesh;
    [SerializeField, Range(10, 200)] int resolution = 10;
    [SerializeField] Functions.Name functionName = Functions.Name.Sine;
    [SerializeField] float transitionDuration = 1;

    int Resolution { set; get; }
    float Step => 2f / Resolution;

    ComputeBuffer pointsBuffer;
    Functions.Name previousFunctionName;
    float transitionTime;

    void Awake() => Initialize();

    void Initialize() {
        Resolution = resolution;
        pointsBuffer?.Release();
        pointsBuffer = new ComputeBuffer(Resolution * Resolution, 3 * 4);
    }

    void OnDisable() => pointsBuffer.Release();

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

        UpdateFunctionOnGPU();

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

    void UpdateFunctionOnGPU() {
        functionsShader.SetInt(RESOLUTION, Resolution);
        functionsShader.SetFloat(STEP, Step);
        functionsShader.SetFloat(TIME, Time.time);

        functionsShader.SetBuffer(0, POINTS, pointsBuffer);

        var groups = Mathf.CeilToInt(Resolution / 8f);
        functionsShader.Dispatch(0, groups, groups, 1);

        material.SetBuffer(POINTS, pointsBuffer);
        material.SetFloat(STEP, Step);

        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / Resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, pointsBuffer.count);
    }
}
