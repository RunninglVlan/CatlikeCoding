using UnityEngine;

public class Fractal : MonoBehaviour {
    [SerializeField] Mesh mesh = default;
    [SerializeField] Material material = default;
    [SerializeField] int maxDepth = 4;

    int depth;

    void Start() {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
        if (depth < maxDepth) {
            new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this);
        }
    }

    void Initialize(Fractal parent) {
        mesh = parent.mesh;
        material = parent.material;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        transform.parent = parent.transform;
    }
}
