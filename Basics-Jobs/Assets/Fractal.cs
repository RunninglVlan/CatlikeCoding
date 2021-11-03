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
    [SerializeField] Transform childPrefab;

    Child[][] children;

    void Awake() {
        children = new Child[depth][];
        for (int index = 0, length = 1; index < children.Length; index++, length *= CHILDREN.Length) {
            children[index] = new Child[length];
        }

        var level = 0;
        float currentScale = 1;
        children[level][0] = CreateChild(0, currentScale);
        level++;
        for (; level < children.Length; level++) {
            currentScale *= CHILD_SCALE;
            var levelParts = children[level];
            for (var part = 0; part < levelParts.Length; part += CHILDREN.Length) {
                for (var child = 0; child < CHILDREN.Length; child++) {
                    levelParts[part + child] = CreateChild(child, currentScale);
                }
            }
        }

        Child CreateChild(int index, float scale) {
            var child = Instantiate(childPrefab, transform);
            child.name = $"Fractal [{level}, {index}]";
            var childTransform = child.transform;
            childTransform.localScale = Vector3.one * scale;
            var (direction, orientation) = CHILDREN[index];
            return new Child {
                direction = direction,
                rotation = orientation,
                transform = child
            };
        }
    }

    void Update() {
        var deltaRotation = Quaternion.Euler(0, ROTATION_SPEED * Time.deltaTime, 0);
        var level = 0;
        var root = children[level][0];
        root.rotation *= deltaRotation;
        root.transform.localRotation = root.rotation;

        level++;
        for (; level < children.Length; level++) {
            var parents = children[level - 1];
            var levelChildren = children[level];
            for (var index = 0; index < levelChildren.Length; index++) {
                var parentTransform = parents[index / CHILDREN.Length].transform;
                var child = levelChildren[index];
                var childTransform = child.transform;
                var parentRotation = parentTransform.localRotation;
                child.rotation *= deltaRotation;
                childTransform.localRotation = parentRotation * child.rotation;
                childTransform.localPosition =
                    parentTransform.localPosition +
                    parentRotation * child.direction * (childTransform.localScale.x * CHILD_OFFSET);
            }
        }
    }

    class Child {
        public Vector3 direction;
        public Quaternion rotation;
        public Transform transform;
    }
}
