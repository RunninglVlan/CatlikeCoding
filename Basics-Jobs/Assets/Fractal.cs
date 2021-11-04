using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Fractal : MonoBehaviour {
    static readonly (Vector3 direction, Quaternion orientation)[] CHILDREN = {
        (Vector3.up, Quaternion.identity),
        (Vector3.right, Quaternion.Euler(0, 0, -90)),
        (Vector3.left, Quaternion.Euler(0, 0, 90)),
        (Vector3.forward, Quaternion.Euler(90, 0, 0)),
        (Vector3.back, Quaternion.Euler(-90, 0, 0))
    };

    const float CHILD_OFFSET = 1.5f;
    const float CHILD_SCALE = .5f;
    const float ROTATION_SPEED = 22.5f;

    static readonly int MATRICES = Shader.PropertyToID("Matrices");

    [SerializeField, Range(1, 8)] int depth = 4;
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;

    NativeArray<Child>[] children;
    NativeArray<Matrix4x4>[] matrices;
    ComputeBuffer[] matricesBuffers;
    static MaterialPropertyBlock propertyBlock;

    void Awake() {
        children = new NativeArray<Child>[depth];
        matrices = new NativeArray<Matrix4x4>[depth];
        matricesBuffers = new ComputeBuffer[depth];
        propertyBlock = new MaterialPropertyBlock();
        const int matrixSize = sizeof(float) * 16;
        for (int index = 0, length = 1; index < children.Length; index++, length *= CHILDREN.Length) {
            children[index] = new NativeArray<Child>(length, Allocator.Persistent);
            matrices[index] = new NativeArray<Matrix4x4>(length, Allocator.Persistent);
            matricesBuffers[index] = new ComputeBuffer(length, matrixSize);
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
            var (direction, orientation) = CHILDREN[index];
            return new Child {
                direction = direction,
                rotation = orientation
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
        root.worldRotation = rootTransform.rotation * (root.rotation * Quaternion.Euler(0, root.spinAngle, 0));
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
            jobHandle = job.Schedule(children[level].Length, jobHandle);
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
        public Vector3 direction, worldPosition;
        public Quaternion rotation, worldRotation;
        public float spinAngle;
    }

    [BurstCompile]
    struct UpdateFractalLevelJob : IJobFor {
        public int childCount;
        public float spinAngleDelta;
        public float scale;

        [ReadOnly] public NativeArray<Child> parents;
        public NativeArray<Child> children;

        [WriteOnly] public NativeArray<Matrix4x4> matrices;

        void IJobFor.Execute(int index) {
            var parent = parents[index / childCount];
            var child = children[index];
            child.spinAngle += spinAngleDelta;
            child.worldRotation = parent.worldRotation * (child.rotation * Quaternion.Euler(0, child.spinAngle, 0));
            child.worldPosition = parent.worldPosition +
                                  parent.worldRotation * child.direction * (scale * CHILD_OFFSET);
            children[index] = child;
            matrices[index] = Matrix(child, scale);
        }

        public static Matrix4x4 Matrix(Child child, float scale) {
            return Matrix4x4.TRS(child.worldPosition, child.worldRotation, Vector3.one * scale);
        }
    }
}
