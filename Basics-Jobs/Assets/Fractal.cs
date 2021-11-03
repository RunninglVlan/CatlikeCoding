using UnityEngine;

public class Fractal : MonoBehaviour {
    static readonly (Vector3 direction, Quaternion orientation)[] CHILDREN = {
        (Vector3.up, Quaternion.identity),
        (Vector3.right, Quaternion.Euler(0, 0, -90)),
        (Vector3.left, Quaternion.Euler(0, 0, 90)),
        (Vector3.forward, Quaternion.Euler(90, 0, 0)),
        (Vector3.back, Quaternion.Euler(-90, 0, 0))
    };

    const float CHILD_OFFSET = .75f;
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

        float currentScale = 1;
        CreateChild(0, 0, currentScale);
        for (var level = 1; level < children.Length; level++) {
            currentScale *= CHILD_SCALE;
            var levelParts = children[level];
            for (var part = 0; part < levelParts.Length; part += CHILDREN.Length) {
                for (var child = 0; child < CHILDREN.Length; child++) {
                    CreateChild(level, child, currentScale);
                }
            }
        }

        void CreateChild(int level, int index, float scale /*Vector3 direction, Quaternion orientation*/) {
            var child = Instantiate(childPrefab, transform);
            child.name = $"Fractal [{level}, {index}]";
            var childTransform = child.transform;
            // childTransform.localPosition = direction * CHILD_OFFSET;
            childTransform.localScale = Vector3.one * scale;
            // childTransform.localRotation = orientation;
        }
    }

    // void Update() => transform.Rotate(0, ROTATION_SPEED * Time.deltaTime, 0);

    struct Child {
        public Vector3 direction;
        public Quaternion orientation;
        public Transform transform;
    }
}
