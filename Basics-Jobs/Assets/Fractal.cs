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

    static readonly Fractal[] children = new Fractal[CHILDREN.Length];

    void Start() {
        name = $"Fractal {depth}";
        if (depth <= 1) {
            return;
        }

        for (var index = 0; index < CHILDREN.Length; index++) {
            var (direction, orientation) = CHILDREN[index];
            children[index] = CreateChild(direction, orientation);
        }

        foreach (var child in children) {
            child.transform.SetParent(transform, false);
        }

        Fractal CreateChild(Vector3 direction, Quaternion orientation) {
            var child = Instantiate(this);
            child.depth = depth - 1;
            var childTransform = child.transform;
            childTransform.localPosition = direction * CHILD_OFFSET;
            childTransform.localScale = Vector3.one * CHILD_SCALE;
            childTransform.localRotation = orientation;
            return child;
        }
    }

    void Update() => transform.Rotate(0, ROTATION_SPEED * Time.deltaTime, 0);
}
