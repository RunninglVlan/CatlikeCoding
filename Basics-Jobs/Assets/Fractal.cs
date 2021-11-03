using UnityEngine;

public class Fractal : MonoBehaviour {
    [SerializeField, Range(1, 8)] int depth = 4;

    void Start() {
        name = $"Fractal {depth}";
        if (depth <= 1) {
            return;
        }

        var child = Instantiate(this, transform);
        child.depth = depth - 1;
        var childTransform = child.transform;
        childTransform.localPosition = Vector3.right * .75f;
        childTransform.localScale = Vector3.one * .5f;
    }
}
