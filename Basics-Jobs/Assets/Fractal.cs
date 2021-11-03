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

    [SerializeField, Range(1, 8)] int depth = 4;

    Child[][] children;

    void Awake() {
        children = new Child[depth][];
        for (int index = 0, length = 1; index < children.Length; index++, length *= CHILDREN.Length) {
            children[index] = new Child[length];
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

    void Update() {
        var deltaRotation = Quaternion.Euler(0, ROTATION_SPEED * Time.deltaTime, 0);
        var level = 0;
        var root = children[level][0];
        root.rotation *= deltaRotation;
        root.worldRotation = root.rotation;

        level++;
        float scale = 1;
        for (; level < children.Length; level++) {
            scale *= CHILD_SCALE;
            var parents = children[level - 1];
            var levelChildren = children[level];
            for (var index = 0; index < levelChildren.Length; index++) {
                var parent = parents[index / CHILDREN.Length];
                var child = levelChildren[index];
                child.rotation *= deltaRotation;
                child.worldRotation = parent.worldRotation * child.rotation;
                child.worldPosition = parent.worldPosition +
                                      parent.worldRotation * child.direction * (scale * CHILD_OFFSET);
            }
        }
    }

    class Child {
        public Vector3 direction, worldPosition;
        public Quaternion rotation, worldRotation;
    }
}
