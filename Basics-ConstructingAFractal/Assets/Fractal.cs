using UnityEngine;

public class Fractal : MonoBehaviour {
    [SerializeField] Mesh mesh = default;
    [SerializeField] Material material = default;

    void Start() {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
    }
}
