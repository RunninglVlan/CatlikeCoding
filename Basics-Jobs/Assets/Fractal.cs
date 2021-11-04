using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class Fractal : MonoBehaviour {
    static readonly (float3 direction, quaternion rotation)[] CHILDREN = {
        (math.up(), quaternion.identity),
        (math.right(), quaternion.RotateZ(-.5f * math.PI)),
        (math.left(), quaternion.RotateZ(.5f * math.PI)),
        (math.forward(), quaternion.RotateX(.5f * math.PI)),
        (math.back(), quaternion.RotateX(-.5f * math.PI))
    };

    const float CHILD_OFFSET = 1.5f;
    const float CHILD_SCALE = .5f;
    const float ROTATION_SPEED = .125f * math.PI;

    static readonly int MATRICES = Shader.PropertyToID("Matrices");

    [SerializeField, Range(1, 8)] int depth = 4;
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;

    NativeArray<Child>[] children;
    NativeArray<float3x4>[] matrices;
    ComputeBuffer[] matricesBuffers;
    static MaterialPropertyBlock propertyBlock;

    void Awake() {
        children = new NativeArray<Child>[depth];
        matrices = new NativeArray<float3x4>[depth];
        matricesBuffers = new ComputeBuffer[depth];
        propertyBlock = new MaterialPropertyBlock();
        const int stride = sizeof(float) * 12;
        for (int index = 0, length = 1; index < children.Length; index++, length *= CHILDREN.Length) {
            children[index] = new NativeArray<Child>(length, Allocator.Persistent);
            matrices[index] = new NativeArray<float3x4>(length, Allocator.Persistent);
            matricesBuffers[index] = new ComputeBuffer(length, stride);
        }

        var level = 0;
        children[level][0] = CreateChild(0);
        level++;
        for (; level < children.Length; level++) {
            var levelParts = children[level];
            for (var part = 0; part < levelParts.Length; part += CHILDREN.Length) {
                for (var child = 0; child < CHILDREN.Length; child++) {
                    levelParts[part + child] = CreateChild(child);
                }
            }
        }

        Child CreateChild(int index) {
            var (direction, rotation) = CHILDREN[index];
            return new Child {
                direction = direction,
                rotation = rotation
            };
        }
    }

    void OnDisable() {
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
                childCount = CHILDREN.Length,
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
        for (var index = 0; index < matricesBuffers.Length; index++) {
            var buffer = matricesBuffers[index];
            buffer.SetData(matrices[index]);
            propertyBlock.SetBuffer(MATRICES, buffer);
            Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, buffer.count, propertyBlock);
        }
    }

    struct Child {
        public float3 direction, worldPosition;
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
            child.worldRotation = math.mul(parent.worldRotation,
                math.mul(child.rotation, quaternion.RotateY(child.spinAngle)));
            child.worldPosition = parent.worldPosition +
                                  math.mul(parent.worldRotation, child.direction * (scale * CHILD_OFFSET));
            children[index] = child;
            matrices[index] = Matrix(child, scale);
        }

        public static float3x4 Matrix(Child child, float scale) {
            var rs = math.float3x3(child.worldRotation) * scale;
            return math.float3x4(rs.c0, rs.c1, rs.c2, child.worldPosition);
        }
    }
}
