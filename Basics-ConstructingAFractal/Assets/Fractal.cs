using UnityEngine;

public class Fractal : MonoBehaviour {
    [SerializeField] Mesh mesh = default;
    [SerializeField] Material material = default;
    [SerializeField] int maxDepth = 4;
    [SerializeField] float childScale = .5f;

    int depth;

    void Start() {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
        if (depth < maxDepth) {
            new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, Vector3.up);
            new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, Vector3.right);
        }
    }

    void Initialize(Fractal parent, Vector3 direction) {
        mesh = parent.mesh;
        material = parent.material;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        var childTransform = transform;
        childTransform.parent = parent.transform;
        childTransform.localScale = Vector3.one * childScale;
        childTransform.localPosition = direction * (.5f + .5f * childScale);
    }
}
