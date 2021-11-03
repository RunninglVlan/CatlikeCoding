using UnityEngine;

public class Fractal : MonoBehaviour {
    [SerializeField, Range(1, 8)] int depth = 4;

    void Start() {
        name = $"Fractal {depth}";
        if (depth <= 1) {
            return;
        }

        var child1 = CreateChild(Vector3.right);
        var child2 = CreateChild(Vector3.up);
        child1.transform.SetParent(transform, false);
        child2.transform.SetParent(transform, false);

        Fractal CreateChild(Vector3 direction) {
            var child = Instantiate(this);
            child.depth = depth - 1;
            var childTransform = child.transform;
            childTransform.localPosition = direction * .75f;
            childTransform.localScale = Vector3.one * .5f;
            return child;
        }
    }
}
