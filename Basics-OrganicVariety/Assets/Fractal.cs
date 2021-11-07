using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fractal : MonoBehaviour {
    static readonly quaternion[] CHILD_ROTATION = {
        quaternion.identity,
        quaternion.RotateZ(-.5f * math.PI),
        quaternion.RotateZ(.5f * math.PI),
        quaternion.RotateX(.5f * math.PI),
        quaternion.RotateX(-.5f * math.PI)
    };

    const int MIN_DEPTH = 3, MAX_DEPTH = 8;
    const float CHILD_OFFSET = 1.5f;
    const float CHILD_SCALE = .5f;
    const float ROTATION_SPEED = .125f * math.PI;

    static readonly int MATRICES = Shader.PropertyToID("Matrices");
    static readonly int COLOR_1 = Shader.PropertyToID("Color1");
    static readonly int COLOR_2 = Shader.PropertyToID("Color2");
    static readonly int SEQUENCE_NUMBERS = Shader.PropertyToID("SequenceNumbers");

    [SerializeField, Range(MIN_DEPTH, MAX_DEPTH)] int depth = 4;
    [SerializeField] Mesh mesh, leafMesh;
    [SerializeField] Material material;
    [SerializeField] Gradient gradient1, gradient2;
    [SerializeField] Color leafColor1, leafColor2;

    NativeArray<Child>[] children;
    NativeArray<float3x4>[] matrices;
    ComputeBuffer[] matricesBuffers;
    Vector4[] sequenceNumbers;
    static MaterialPropertyBlock propertyBlock;

    void Awake() => Initialize();

    void Initialize() {
        children = new NativeArray<Child>[depth];
        matrices = new NativeArray<float3x4>[depth];
        matricesBuffers = new ComputeBuffer[depth];
        sequenceNumbers = new Vector4[depth];
        propertyBlock = new MaterialPropertyBlock();
        const int stride = sizeof(float) * 12;
        for (int index = 0, length = 1; index < children.Length; index++, length *= CHILD_ROTATION.Length) {
            children[index] = new NativeArray<Child>(length, Allocator.Persistent);
            matrices[index] = new NativeArray<float3x4>(length, Allocator.Persistent);
            matricesBuffers[index] = new ComputeBuffer(length, stride);
            sequenceNumbers[index] = new Vector4(Random.value, Random.value, Random.value, Random.value);
        }

        var level = 0;
        children[level][0] = CreateChild(0);
        level++;
        for (; level < children.Length; level++) {
            var levelParts = children[level];
            for (var part = 0; part < levelParts.Length; part += CHILD_ROTATION.Length) {
                for (var child = 0; child < CHILD_ROTATION.Length; child++) {
                    levelParts[part + child] = CreateChild(child);
                }
            }
        }

        Child CreateChild(int index) {
            return new Child {
                rotation = CHILD_ROTATION[index]
            };
        }
    }

    public void ChangeDepth(int delta) {
        var newDepth = Mathf.Clamp(depth + delta, MIN_DEPTH, MAX_DEPTH);
        if (depth == newDepth) {
            return;
        }

        depth = newDepth;
        ClearGarbage();
        Initialize();
    }

    void OnDisable() => ClearGarbage();

    void ClearGarbage() {
        for (var index = 0; index < matricesBuffers.Length; index++) {
            matricesBuffers[index].Release();
            children[index].Dispose();
            matrices[index].Dispose();
        }
    }

    void Update() {
        var spinAngleDelta = ROTATION_SPEED * Time.deltaTime;
        var level = 0;
        var root = children[level][0];
        root.spinAngle += spinAngleDelta;
        var rootTransform = transform;
        root.worldRotation =
            math.mul(rootTransform.rotation, math.mul(root.rotation, quaternion.RotateY(root.spinAngle)));
        root.worldPosition = rootTransform.position;
        children[level][0] = root;
        var scale = rootTransform.lossyScale.x;
        matrices[level][0] = UpdateFractalLevelJob.Matrix(root, scale);

        level++;
        JobHandle jobHandle = default;
        for (; level < children.Length; level++) {
            scale *= CHILD_SCALE;
            var job = new UpdateFractalLevelJob {
                childCount = CHILD_ROTATION.Length,
                spinAngleDelta = spinAngleDelta,
                scale = scale,
                parents = children[level - 1],
                children = children[level],
                matrices = matrices[level]
            };
            jobHandle = job.ScheduleParallel(children[level].Length, 5, jobHandle);
        }
        jobHandle.Complete();

        var bounds = new Bounds(root.worldPosition, 3 * scale * Vector3.one);
        var leafIndex = matricesBuffers.Length - 1;
        for (var index = 0; index < matricesBuffers.Length; index++) {
            var buffer = matricesBuffers[index];
            buffer.SetData(matrices[index]);
            propertyBlock.SetBuffer(MATRICES, buffer);

            Mesh instanceMesh;
            Color color1, color2;
            if (index == leafIndex) {
                color1 = leafColor1;
                color2 = leafColor2;
                instanceMesh = leafMesh;
            } else {
                var gradientInterpolator = index / (matricesBuffers.Length - 2f);
                color1 = gradient1.Evaluate(gradientInterpolator);
                color2 = gradient2.Evaluate(gradientInterpolator);
                instanceMesh = mesh;
            }

            propertyBlock.SetColor(COLOR_1, color1);
            propertyBlock.SetColor(COLOR_2, color2);
            propertyBlock.SetVector(SEQUENCE_NUMBERS, sequenceNumbers[index]);
            Graphics.DrawMeshInstancedProcedural(instanceMesh, 0, material, bounds, buffer.count, propertyBlock);
        }
    }

    struct Child {
        public float3 worldPosition;
        public quaternion rotation, worldRotation;
        public float spinAngle;
    }

    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast)]
    struct UpdateFractalLevelJob : IJobFor {
        public int childCount;
        public float spinAngleDelta;
        public float scale;

        [ReadOnly] public NativeArray<Child> parents;
        public NativeArray<Child> children;

        [WriteOnly] public NativeArray<float3x4> matrices;

        void IJobFor.Execute(int index) {
            var parent = parents[index / childCount];
            var child = children[index];
            child.spinAngle += spinAngleDelta;

            var upAxis = math.mul(math.mul(parent.worldRotation, child.rotation), math.up());
            var sagAxis = math.cross(math.up(), upAxis);
            var sagMagnitude = math.length(sagAxis);
            quaternion baseRotation;
            if (sagMagnitude > 0) {
                sagAxis /= sagMagnitude;
                var sagRotation = quaternion.AxisAngle(sagAxis, math.PI * .25f * sagMagnitude);
                baseRotation = math.mul(sagRotation, parent.worldRotation);
            } else {
                baseRotation = parent.worldRotation;
            }

            child.worldRotation = math.mul(baseRotation,
                math.mul(child.rotation, quaternion.RotateY(child.spinAngle)));
            child.worldPosition = parent.worldPosition +
                                  math.mul(child.worldRotation, math.float3(0, scale * CHILD_OFFSET, 0));
            children[index] = child;
            matrices[index] = Matrix(child, scale);
        }

        public static float3x4 Matrix(Child child, float scale) {
            var rs = math.float3x3(child.worldRotation) * scale;
            return math.float3x4(rs.c0, rs.c1, rs.c2, child.worldPosition);
        }
    }
}
