using Unity.Mathematics;
using UnityEngine;

public class GPUGraph : MonoBehaviour {
    static readonly int POINTS = Shader.PropertyToID("Points");
    static readonly int RESOLUTION = Shader.PropertyToID("Resolution");
    static readonly int STEP = Shader.PropertyToID("Step");
    static readonly int TIME = Shader.PropertyToID("Time");
    static readonly int TRANSITION_PROGRESS = Shader.PropertyToID("TransitionProgress");

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
        Initialize();
    }

    void Update() => UpdateFunctionOnGPU();

    void UpdateFunctionOnGPU() {
        var transitioning = Transitioning(out var transitionProgress);
        functionsShader.SetInt(RESOLUTION, Resolution);
        functionsShader.SetFloat(STEP, Step);
        functionsShader.SetFloat(TIME, Time.time);
        if (transitioning) {
            functionsShader.SetFloat(TRANSITION_PROGRESS, math.smoothstep(0, 1, transitionProgress));
        }

        var kernelIndex = (int)functionName +
                          (int)(transitioning ? previousFunctionName : functionName) * Functions.Length;
        functionsShader.SetBuffer(kernelIndex, POINTS, pointsBuffer);

        var groups = Mathf.CeilToInt(Resolution / 8f);
        functionsShader.Dispatch(kernelIndex, groups, groups, 1);

        material.SetBuffer(POINTS, pointsBuffer);
        material.SetFloat(STEP, Step);

        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / Resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, pointsBuffer.count);

        bool Transitioning(out float progress) {
            var result = transitionTime > 0;
            if (result) {
                transitionTime -= Time.deltaTime;
            }

            progress = (transitionDuration - transitionTime) / transitionDuration;
            return result;
        }
    }
}
